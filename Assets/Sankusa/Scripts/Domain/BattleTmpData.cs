using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sankusa.unity1week202209.Domain {
    // 途中切断ペナルティ用一時保存データ
    [Serializable]
    public class BattleTmpData
    {
        [SerializeField] private bool playing = false;
        public bool Playing {
            get => playing;
            set => playing = value;
        }

        [SerializeField] private string enemyKey = "";
        public string EnemyKey {
            get => enemyKey;
            set => enemyKey = value;
        }

        [SerializeField] private string playerName = "";
        public string PlayerName {
            get => playerName;
            set => playerName = value;
        }

        [SerializeField] private string enemyName = "";
        public string EnemyName {
            get => enemyName;
            set => enemyName = value;
        }

        [SerializeField] private long loseScoreIncrement = 0;
        public long LoseScoreIncrement {
            get => loseScoreIncrement;
            set => loseScoreIncrement = value;
        }

        public void Clear() {
            playing = false;
            playerName = "";
            enemyKey = "";
            enemyName = "";
            loseScoreIncrement = 0;
        }
    }
}