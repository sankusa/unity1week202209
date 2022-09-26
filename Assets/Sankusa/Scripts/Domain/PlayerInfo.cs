using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sankusa.unity1week202209.Constant;

namespace Sankusa.unity1week202209.Domain {
    [Serializable]
    public class PlayerInfo
    {
        [SerializeField] private string rankingKey = "";
        public string RankingKey {
            get => rankingKey;
            set => rankingKey = value;
        }

        [SerializeField] private BattleTmpData battleTmpData = new BattleTmpData();
        public BattleTmpData BattleTmpData => battleTmpData;

        // 防衛バトルで変動押したレートと比較用
        [SerializeField] private long lastScore = GameConstant.RATE_DEFAULT;
        public long LastScore {
            get => lastScore;
            set => lastScore = value;
        }

        // 再戦判定用
        [SerializeField] private List<string> winHistory = new List<string>();
        public IReadOnlyList<string> WinHistory => winHistory;
        public void AddWinHisory(string rankingKey) {
            winHistory.Add(rankingKey);
            if(winHistory.Count > GameConstant.WIN_HISTORY_NUM) {
                for(int i = winHistory.Count - 1; i >= GameConstant.WIN_HISTORY_NUM; i--) {
                    winHistory.RemoveAt(0);
                }
            }
        }
        public string RemoveOldestWinHistory() {
            string oldest = winHistory[0];
            winHistory.RemoveAt(0);
            return oldest;
        }

        [SerializeField] private int lastUploadedStructureIndex = -1;
        public int LastUploadedStructureIndex {
            get => lastUploadedStructureIndex;
            set => lastUploadedStructureIndex = value;
        }
    }
}