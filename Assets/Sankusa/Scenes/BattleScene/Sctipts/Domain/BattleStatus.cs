using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sankusa.unity1week202209.BattleScene.Domain {
    public class BattleStatus
    {
        private float time = 0;
        public float Time {
            get => time;
            set => time = value < 0 ? 0 : value;
        }
    }
}