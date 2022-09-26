using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sankusa.unity1week202209.Domain {
    public class RankingRecord
    {
        private string key = "";
        public string Key {
            get => key;
            set => key = value;
        }

        private string name = "";
        public string Name {
            get => name;
            set => name = value;
        }

        private long score = 0;
        public long Score {
            get => score;
            set => score = value;
        }

        private SquareStructure structure = null;
        public SquareStructure Structure {
            get => structure;
            set => structure = value;
        }

        public RankingRecord(string key, string name, long score, SquareStructure structure) {
            this.key = key;
            this.name = name;
            this.score = score;
            this.structure = structure;
        }

        // 初回登録用
        public RankingRecord(string name, long score, SquareStructure structure) {
            this.name = name;
            this.score = score;
            this.structure = structure;
        }
    }
}