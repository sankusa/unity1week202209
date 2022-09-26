using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using Zenject;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using SankusaLib;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using SankusaLib.SceneManagementLib;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

namespace Sankusa.unity1week202209.MainScene.View {
    public class RankingView : MonoBehaviour
    {
        [SerializeField] private GameObject rankingRecordViewPrefab;
        [SerializeField] private UIView view;
        [SerializeField] private UIButton backButton;
        [SerializeField] private VerticalLayoutGroup layout;
        [SerializeField] private RankingRecordView ownRecordView;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private GameObject footer;
        [SerializeField] private GameObject dataNoneText;
        [SerializeField] private InputField nameInputField;
        [SerializeField] private UIButton nameUpdateButton;
        [SerializeField] private UIButton battleHisotyButton;
        [SerializeField] private TMP_Text scoreChangeText;
        [SerializeField] private TMP_Text battlePossibleText;
        [SerializeField] private UIButton tweetButton;
        [Inject] private DiContainer container;
        [Inject] private IRankingAccessor rankingAccessor;
        [Inject] private PlayerInfo playerInfo;
        [Inject] private SaveData saveData;
        [Inject] private SceneArgStore sceneArgStore;
        [Inject] private BattleHistoryView battleHistoryView;
        private RankingRecord ownRecord;
        private int ownRank = 0;

        void Start() {
            backButton.AddListenerToPointerClick(() => Hide());
            nameUpdateButton.AddListenerToPointerClick(() => {var _ = UpdateName(this.GetCancellationTokenOnDestroy());});
            battleHisotyButton.AddListenerToPointerClick(() => battleHistoryView.Show());
            tweetButton.AddListenerToPointerClick(() => naichilab.UnityRoomTweet.Tweet(GameConstant.UNITY_ROOM_GAME_ID, "【ランキング " + ownRank + "位 : レート " + ownRecord.Score + "】に到達しました", "unity1week", "8BlockClash"));
        }

        public void Show() {
            view.Show();
            var _ = UpdateRanking(this.GetCancellationTokenOnDestroy());
        }

        public void Hide() {
            view.Hide();
            Clear();
        }

        private void OnRecordButtonClick(RankingRecord enemyRecord) {
            sceneArgStore.Set(new BattleSceneArg(ownRecord, enemyRecord));
            // シーン遷移
            Blackout.Instance.PlayBlackout(0.4f, () => SceneManager.LoadScene(GameConstant.SCENE_NAME_BATTLE));
        }

        private async UniTask UpdateName(CancellationToken cancellationToken) {
            try {
                loadingCanvas.SetActive(true);
                cancellationToken.ThrowIfCancellationRequested();
                await rankingAccessor.UpdateName(playerInfo.RankingKey, nameInputField.text, cancellationToken);
            } catch(OperationCanceledException e) {
                Debug.Log(e);
            } finally {
                loadingCanvas.SetActive(false);
            }
            await UpdateRanking(cancellationToken);
        }

        public async UniTask UpdateRanking(CancellationToken cancellationToken) {
            loadingCanvas.SetActive(true);
            Clear();
            battlePossibleText.text = "自分の順位の前後" + GameConstant.BATTLE_POSSIBLE_RANK_RANGE + "人と戦うことができます";
            try {
                cancellationToken.ThrowIfCancellationRequested();

                if(playerInfo.RankingKey == "") {
                    ownRecordView.SetNone();
                    dataNoneText.SetActive(true);
                    footer.SetActive(false);
                } else {
                    ownRecord = await rankingAccessor.FetchRecord(playerInfo.RankingKey, cancellationToken);
                    if(ownRecord == null) throw new OperationCanceledException();

                    // スコア保存
                    long scoreChange = ownRecord.Score - playerInfo.LastScore;
                    playerInfo.LastScore = ownRecord.Score;
                    saveData.Save(GameConstant.SAVE_KEY);
                    if(scoreChange > 0) {
                        scoreChangeText.text = "+" + scoreChange.ToString();
                        scoreChangeText.color = Color.red;
                    } else if(scoreChange < 0) {
                        scoreChangeText.text = scoreChange.ToString();
                        scoreChangeText.color = new Color(0.5f, 0.5f, 1f);
                    } else {
                        scoreChangeText.text = scoreChange.ToString();
                        scoreChangeText.color = Color.white;
                    }

                    ownRank = await rankingAccessor.FetchRank(ownRecord.Score, cancellationToken);
                    if(ownRank == 0) throw new OperationCanceledException();

                    ownRecordView.SetRecord(ownRank, ownRecord, false, false);
                    nameInputField.text = ownRecord.Name;
                    dataNoneText.SetActive(false);
                    footer.SetActive(true);
                }

                Tuple<int, List<RankingRecord>> result = await rankingAccessor.FetchNeighbors(ownRank, 100, 100, cancellationToken: cancellationToken);
                int currentRank = result.Item1 - 1;
                long scoreOld = long.MinValue;
                int skippedRank = 0;
                int ownIndex = 0;

                if(result.Item2.Count == 0) throw new OperationCanceledException();

                // 自分のインデックスを取得
                if(playerInfo.RankingKey != "") {
                    for(int i = 0; i < result.Item2.Count; i++) {
                        if(result.Item2[i].Key == playerInfo.RankingKey) {
                            ownIndex = i;
                            break;
                        }
                    }
                }

                // 対戦相手がいない(winHistoryに存在する相手しか付近にいない)場合、対戦可能な相手が見つかるまでwinHistoryを古い順に削除
                int battlableCount = 0;
                for(int i = Mathf.Max(0, ownIndex - GameConstant.BATTLE_POSSIBLE_RANK_RANGE); i <= Mathf.Min(ownIndex + GameConstant.BATTLE_POSSIBLE_RANK_RANGE, result.Item2.Count - 1); i++) {
                    if(i == ownIndex) continue;
                    if(playerInfo.WinHistory.ToList().IndexOf(result.Item2[i].Key) == -1) battlableCount++;
                }
                if(battlableCount == 0) {
                    while(playerInfo.WinHistory.Count > 0) {
                        string oldest = playerInfo.RemoveOldestWinHistory();
                        bool battlable = false;
                        for(int i = Mathf.Max(0, ownIndex - GameConstant.BATTLE_POSSIBLE_RANK_RANGE); i <= ownIndex + GameConstant.BATTLE_POSSIBLE_RANK_RANGE; i++) {
                            if(i == ownIndex) continue;
                            if(result.Item2[i].Key == oldest) {
                                battlable = true;
                                break;
                            }
                        }
                        if(battlable) break;
                    }
                }


                // ランキングレコード生成
                int currentIndex = 0;
                foreach(RankingRecord record in result.Item2) {
                    RankingRecordView recordView = container.InstantiatePrefab(rankingRecordViewPrefab, layout.transform).GetComponent<RankingRecordView>();
                    if(record.Score == scoreOld) {
                        skippedRank++;
                    } else {
                        currentRank += 1 + skippedRank;
                        skippedRank = 0;
                    }

                    bool buttonActive = playerInfo.RankingKey != "" && record.Key != playerInfo.RankingKey && Mathf.Abs(ownIndex - currentIndex) <= GameConstant.BATTLE_POSSIBLE_RANK_RANGE;
                    bool buttonInteractive = (playerInfo.WinHistory.ToList().IndexOf(record.Key) == -1);

                    recordView.SetRecord(currentRank, record, buttonActive, buttonInteractive, OnRecordButtonClick);

                    scoreOld = record.Score;
                    currentIndex++;
                }

                await UniTask.Delay(100, cancellationToken: cancellationToken);

                
                float recordHeight = rankingRecordViewPrefab.GetComponent<RectTransform>().sizeDelta.y;
                Vector3 pos = layout.transform.position;
                pos.y += recordHeight * Mathf.Max(0, ownIndex - 5);
                layout.transform.position = pos;
            } catch (OperationCanceledException e) {
                Debug.Log(e);
            } finally {
                loadingCanvas.SetActive(false);
            }
        }

        public void Clear() {
            int recordCount = layout.transform.childCount;
            for(int i = recordCount - 1; i >= 0; i--) {
                Destroy(layout.transform.GetChild(i).gameObject);
            }
        }
    }
}