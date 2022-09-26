using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using Sankusa.unity1week202209.View;
using Sankusa.unity1week202209.BattleScene.Domain;
using Sankusa.unity1week202209.BattleScene.View;
using SankusaLib.SceneManagementLib;
using SankusaLib;
using UnityEngine.SceneManagement;

namespace Sankusa.unity1week202209.BattleScene.GameLoop {
    // エントリーポイント + 進行管理
    public class BattleSceneGameLoop : IInitializable
    {
        private PlayerInfo playerInfo;
        private IRankingAccessor rankingAccessor;
        private SaveData saveData;

        private SquareStructureConverter structureConverter;
        private SceneArgStore sceneArgStore;

        private BattleStatus battleStatus;
        private BattleStatusView battleStatusView;
        private ResultPerformer resultPerformer;

        [Inject]
        public BattleSceneGameLoop(SaveData saveData,
                                   PlayerInfo playerInfo,
                                   IRankingAccessor rankingAccessor,
                                   SquareStructureConverter structureConverter,
                                   SceneArgStore sceneArgStore,
                                   BattleStatus battleStatus,
                                   BattleStatusView battleStatusView,
                                   ResultPerformer resultPerformer) {
            this.saveData = saveData;
            this.playerInfo = playerInfo;
            this.rankingAccessor = rankingAccessor;
            this.structureConverter = structureConverter;
            this.sceneArgStore = sceneArgStore;
            this.battleStatus = battleStatus;
            this.battleStatusView = battleStatusView;
            this.resultPerformer = resultPerformer;
        }

        public void Initialize() {
            var _ = StartAsync();
        }

        private async UniTask StartAsync() {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // シーン間データ取得
            BattleSceneArg arg = sceneArgStore.Pop<BattleSceneArg>();
            if(arg == null) {
                arg = new BattleSceneArg(
                    new RankingRecord("", "Null1", 1000, SquareStructure.Default),
                    new RankingRecord("", "Null2", 1000, SquareStructure.Default)
                );
            }

            SquareStructureView playerView = structureConverter.DataToView(arg.PlayerRecord.Structure, battleStatusView.PlayerGeneratePosition.position);
            playerView.SetLayer(GameConstant.LAYER_SQUARE_UNIT_1);
            SquareStructureView enemyView = structureConverter.DataToView(arg.EnemyRecord.Structure, battleStatusView.EnemyGeneratePosition.position);
            enemyView.transform.localScale = new Vector3(-enemyView.transform.localScale.x, enemyView.transform.localScale.y, enemyView.transform.localScale.z);
            enemyView.SetLayer(GameConstant.LAYER_SQUARE_UNIT_2);

            battleStatusView.Initialize(arg.PlayerRecord.Name, playerView, arg.EnemyRecord.Name, enemyView);

            battleStatus.Time = 60f;
            // 時間更新(投げっぱなし)
            UniTask.Void(async (CancellationToken token) => {
                while(true) {
                    if(token.IsCancellationRequested) break;
                    await UniTask.Yield();
                    battleStatus.Time -= Time.deltaTime;
                    if(battleStatus.Time <= 0) break;
                }
            }, token);

            // 引力(FixedUpdate)
            UniTask.Void(async (CancellationToken token) => {
                while(true) {
                    await UniTask.WaitForFixedUpdate(token);
                    if(playerView == null || enemyView == null) break;

                    Vector2 playerToEnemy = enemyView.transform.position - playerView.transform.position;
                    playerView.AddForcePerMass(playerToEnemy.normalized * 200);
                    enemyView.AddForcePerMass(-playerToEnemy.normalized * 200);
                }
            }, token);

            // 一時データ保存
            playerInfo.BattleTmpData.Playing = true;
            playerInfo.BattleTmpData.PlayerName = arg.PlayerRecord.Name;
            playerInfo.BattleTmpData.EnemyKey = arg.EnemyRecord.Key;
            playerInfo.BattleTmpData.EnemyName = arg.EnemyRecord.Name;
            playerInfo.BattleTmpData.LoseScoreIncrement = RankingUtil.CalculateLoseAdditionalScore(arg.PlayerRecord.Score, arg.EnemyRecord.Score);
            saveData.Save(GameConstant.SAVE_KEY);

            // 不正機体チェック&強制敗北・・・InstantiateしたフレームではCollider2D絡みのバリデーションが正常に動作しないため、遅延
            UniTask.Void(async (CancellationToken token) => {
                await UniTask.Delay(50, cancellationToken: token);
                if(playerView.ValidationCheck() == false) playerView.Hp = 0;
                if(enemyView.ValidationCheck() == false) enemyView.Hp = 0;
            }, token);

            int result = 0;
            // -1 lose, 0 draw, 1 win 
            while(true) {
                await UniTask.Yield();
                if(battleStatus.Time <= 0) {
                    if(playerView.Hp > enemyView.Hp) {
                        result = 1;
                    } else if(playerView.Hp < enemyView.Hp) {
                        result = -1;
                    } else {
                        result = 0;
                    }
                    break;
                }
                if(playerView.Hp == 0 && enemyView.Hp == 0) {
                    result = 0;
                    break;
                }
                if(playerView.Hp == 0) {
                    playerView.gameObject.SetActive(false);
                    result = -1;
                    
                    break;
                }
                if(enemyView.Hp == 0) {
                    enemyView.gameObject.SetActive(false);
                    result = 1;
                    break;
                }
            }


            if(result == -1) {
                long playerScoreIncrement = RankingUtil.CalculateLoseAdditionalScore(arg.PlayerRecord.Score, arg.EnemyRecord.Score);
                playerInfo.LastScore += playerScoreIncrement;

                resultPerformer.PlayLosePerformance(playerScoreIncrement);

                var _ = rankingAccessor.IncrementScoreAndAndAddBattleHistory(arg.PlayerRecord.Key, true, arg.EnemyRecord.Name, -1, playerScoreIncrement, token);
                _ = rankingAccessor.IncrementScoreAndAndAddBattleHistory(arg.EnemyRecord.Key, false, arg.PlayerRecord.Name, 1, -playerScoreIncrement, token);
            } else if(result == 1) {
                playerInfo.AddWinHisory(arg.EnemyRecord.Key);

                long playerScoreIncrement = RankingUtil.CalculateWinAdditionalScore(arg.PlayerRecord.Score, arg.EnemyRecord.Score);
                playerInfo.LastScore += playerScoreIncrement;
                
                resultPerformer.PlayWinPerformance(playerScoreIncrement);

                var _ = rankingAccessor.IncrementScoreAndAndAddBattleHistory(arg.PlayerRecord.Key, true, arg.EnemyRecord.Name, 1, playerScoreIncrement, token);
                _ = rankingAccessor.IncrementScoreAndAndAddBattleHistory(arg.EnemyRecord.Key, false, arg.PlayerRecord.Name, -1, -playerScoreIncrement, token);
            } else {
                resultPerformer.PlayDrawPerformer();
                _ = rankingAccessor.AddBattleHistory(arg.PlayerRecord.Key, true, arg.EnemyRecord.Name, 0, 0, token);
                _ = rankingAccessor.AddBattleHistory(arg.EnemyRecord.Key, false, arg.PlayerRecord.Name, 0, -0, token);
            }
            
            playerInfo.BattleTmpData.Clear();
            saveData.Save(GameConstant.SAVE_KEY);

            await UniTask.WaitUntil(() => resultPerformer.IsPlaying);

            await UniTask.Delay(2000);

            Blackout.Instance.PlayBlackout(0.5f, () => SceneManager.LoadScene(GameConstant.SCENE_NAME_MAIN));
        }
    }
}