using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sankusa.unity1week202209.Constant;

namespace Sankusa.unity1week202209.Domain {
    // 機体復元用情報
    [Serializable]
    public class SquareStructure
    {
        public static SquareStructure Default {
            get {
                SquareStructure structure = new SquareStructure();
                for(int i = 0; i < GameConstant.SQUARE_UNIT_COUNT_MAX; i++) {
                    SquareUnit unit = SquareUnit.Default;
                    if(i == 1) unit.Position = new Vector2(64, 0);
                    if(i == 2) unit.Position = new Vector2(-64, 0);
                    if(i == 3) unit.Position = new Vector2(64, 64);
                    if(i == 4) unit.Position = new Vector2(-64, 64);
                    if(i == 5) unit.Position = new Vector2(64,128);
                    if(i == 6) unit.Position = new Vector2(-64,128);
                    if(i == 7) unit.Position = new Vector2(0,128);
                    structure.SquareUnits.Add(unit);
                }
                
                return structure;
            }
        }

        [SerializeField] private List<SquareUnit> squareUnits = new List<SquareUnit>();
        public List<SquareUnit> SquareUnits => squareUnits;

        [SerializeField] private int hpCost = 0;
        public int HpCost {
            get => hpCost;
            set => hpCost = value;
        }
    }
}