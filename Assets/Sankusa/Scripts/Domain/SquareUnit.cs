using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sankusa.unity1week202209.Domain
{
    // ブロック復元用情報
    [Serializable]
    public class SquareUnit
    {
        public static SquareUnit Default {
            get {
                SquareUnit unit = new SquareUnit();
                unit.Scale = 1f;
                return unit;
            }
        }

        [SerializeField] private Vector2 position = Vector2.zero;
        public Vector2 Position {
            get => position;
            set => position = value;
        }

        [SerializeField] private float rotationZ = 0;
        public float RotationZ {
            get => rotationZ;
            set => rotationZ = value;
        }

        [SerializeField] private float scale = 1;
        public float Scale {
            get => scale;
            set => scale = value;
        }

        [SerializeField] private float scaleX = 1;
        public float ScaleX {
            get => scaleX;
            set => scaleX = value;
        }

        [SerializeField] private float scaleY = 1;
        public float ScaleY {
            get => scaleY;
            set => scaleY = value;
        }

        [SerializeField] private int attackCost = 0;
        public int AttackCost {
            get => attackCost;
            set => attackCost = value;
        }

        [SerializeField] private int defenceCost = 0;
        public int DefenceCost {
            get => defenceCost;
            set => defenceCost = value;
        }

        [SerializeField] private int attackCoolTimeCutCost = 0;
        public int AttackCoolTimeCutCost {
            get => attackCoolTimeCutCost;
            set => attackCoolTimeCutCost = value;
        }
    }
}

