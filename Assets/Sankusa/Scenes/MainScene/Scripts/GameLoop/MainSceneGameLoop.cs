using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using System.Threading;

namespace Sankusa.unity1week202209.MainScene.GameLoop {
    // エントリーポイント
    public class MainSceneGameLoop : IInitializable
    {
        private PlayerInfo playerInfo;
        private SaveData saveData;
        private IRankingAccessor rankingAccessor;

        [Inject]
        public MainSceneGameLoop(PlayerInfo playerInfo, SaveData saveData, IRankingAccessor rankingAccessor) {
            this.playerInfo = playerInfo;
            this.saveData = saveData;
            this.rankingAccessor = rankingAccessor;
        }

        public void Initialize() {
            BattleTmpData battleTmpData = playerInfo.BattleTmpData;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            //　切断ペナルティ処理
            if(battleTmpData.Playing) {
                rankingAccessor.IncrementScoreAndAndAddBattleHistory(playerInfo.RankingKey, true, battleTmpData.EnemyName, -1, battleTmpData.LoseScoreIncrement, token);
                rankingAccessor.IncrementScoreAndAndAddBattleHistory(battleTmpData.EnemyKey, false, battleTmpData.PlayerName, 1, -battleTmpData.LoseScoreIncrement, token);
                playerInfo.LastScore += battleTmpData.LoseScoreIncrement;
                playerInfo.BattleTmpData.Clear();
                saveData.Save(GameConstant.SAVE_KEY);
            }
        }

    }
}