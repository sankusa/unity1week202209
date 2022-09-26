using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sankusa.unity1week202209.Domain {
    public class RankingUtil
    {
        public static long CalculateWinAdditionalScore(long myScore, long otherScore) {
            long difference = otherScore - myScore;
            return (long)((Mathf.Atan(difference / 25f) / (Mathf.PI / 2) + 1) * 15);
        }

        public static long CalculateLoseAdditionalScore(long myScore, long otherScore) {
            long difference = otherScore - myScore;
            return (long)((Mathf.Atan(difference / 25f) / (Mathf.PI / 2) + 1) * 15) - 30;
        }
    }
}