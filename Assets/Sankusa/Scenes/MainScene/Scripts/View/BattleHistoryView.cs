using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using UnityEngine.UI;
using SankusaLib;
using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;
using Sankusa.unity1week202209.Domain;
using System;

namespace Sankusa.unity1week202209.MainScene.View {
    public class BattleHistoryView : MonoBehaviour
    {
        [SerializeField] private GameObject rankingRecordViewPrefab;
        [SerializeField] private UIView view;
        [SerializeField] private UIButton backButton;
        [SerializeField] private VerticalLayoutGroup layout;
        [SerializeField] private GameObject loadingCanvas;
        [Inject] private IRankingAccessor rankingAccessor;
        [Inject] private PlayerInfo playerInfo;

        void Start() {
            backButton.AddListenerToPointerClick(() => Hide());
        }

        public void Show() {
            view.Show();
            var _ = UpdateView(this.GetCancellationTokenOnDestroy());
        }

        public void Hide() {
            view.Hide();
            Clear();
        }

        public void Clear() {
            int recordCount = layout.transform.childCount;
            for(int i = recordCount - 1; i >= 0; i--) {
                Destroy(layout.transform.GetChild(i).gameObject);
            }
        }

        public async UniTask UpdateView(CancellationToken cancellationToken) {
            loadingCanvas.SetActive(true);
            Clear();
            try {
                cancellationToken.ThrowIfCancellationRequested();

                List<Tuple<bool, string, int, long>> histories;

                if(playerInfo.RankingKey == "") {
                    return;
                } else {
                    histories = await rankingAccessor.FetchBattleHistory(playerInfo.RankingKey, cancellationToken);
                }
                histories.Reverse();

                foreach(Tuple<bool, string, int, long> history in histories) {
                    BattleHistoryRecordView recordView = Instantiate(rankingRecordViewPrefab, layout.transform).GetComponent<BattleHistoryRecordView>();

                    recordView.SetValues(history.Item1, history.Item2, history.Item3, history.Item4);
                }
            } catch (OperationCanceledException e) {
                Debug.Log(e);
            } finally {
                loadingCanvas.SetActive(false);
            }
        }
    }
}