using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SankusaLib.SceneManagementLib;
using Sankusa.unity1week202209.Domain;

namespace Sankusa.unity1week202209.Domain {
    // BattleSceneに渡す情報
    public class BattleSceneArg : ISceneArg
    {
        private RankingRecord playerRecord;
        public RankingRecord PlayerRecord => playerRecord;

        private RankingRecord enemyRecord;
        public RankingRecord EnemyRecord => enemyRecord;

        public BattleSceneArg(RankingRecord playerRecord, RankingRecord enemyRecord) {
            this.playerRecord = playerRecord;
            this.enemyRecord = enemyRecord;
        }
    }
}