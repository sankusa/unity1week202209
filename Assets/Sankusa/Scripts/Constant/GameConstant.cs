using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sankusa.unity1week202209.Constant {
    public class GameConstant
    {
        public const string UNITY_ROOM_GAME_ID = "8blockclash";
        
        public const string SCENE_NAME_MAIN = "MainScene";
        public const string SCENE_NAME_BATTLE = "BattleScene";

        public const int LAYER_SQUARE_UNIT_1 = 7;
        public const int LAYER_SQUARE_UNIT_2 = 8;

        public const string SAVE_KEY = "SAVE_KEY";

        public const string PLAYER_NAME_DEFAULT = "No Name";
        public const int RATE_DEFAULT = 1000;

        public const int SQUARE_STRUCTURE_STORAGE_CAPACITY = 3;

        public const int SQUARE_UNIT_COUNT_MAX = 8;

        public const int WIN_HISTORY_NUM = 8;

        public const float ATTACK_COOL_TIME = 0.1f;

        public const int BATTLE_POSSIBLE_RANK_RANGE = 5;

        public const float ALLOWABLE_UNIT_DISTANCE_ERROR = 5f;

        // ステータス関係
        public const int COST_MAX = 200;

        public const int HP_BASE = 100;
        public const float HP_RATE = 4f;
        public const int HP_COST_MAX = 100;

        public const int ATTACK_BASE = 0;
        public const float ATTACK_RATE = 1f;
        public const int ATTACK_COST_MAX = 40;

        public const int DEFENCE_BASE = 0;
        public const float DEFENCE_RATE = 1f;
        public const int DEFENCE_COST_MAX = 40;

        public const float ATTACK_COOL_TIME_CUT_MAX = 0.9f;
        public const float ATTACK_COOL_TIME_CUT_COST_MAX_PER_ATTACK_COST_MAX = 1f;
    }
}