using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sankusa.unity1week202209.Domain;
using SankusaLib;

namespace Sankusa.unity1week202209.View {
    // 復元用データ　←→　実体(View) 相互変換クラス
    public class SquareStructureConverter : MonoBehaviour
    {
        [SerializeField] private GameObject structurePrefab;
        [SerializeField] private GameObject unitPrefab;

        public static SquareStructure ViewToData(SquareStructureView structureView) {
            SquareStructure structure = new SquareStructure();
            structure.HpCost = structureView.HpCost;

            foreach(SquareUnitView unitView in structureView.SquareUnits) {
                SquareUnit unit = new SquareUnit();
                unit.Position = unitView.transform.localPosition;
                unit.RotationZ = unitView.transform.rotation.eulerAngles.z;
                unit.Scale = 1;
                unit.AttackCost = unitView.AttackCost;
                unit.DefenceCost = unitView.DefenceCost;
                unit.ScaleX = unitView.transform.localScale.x;
                unit.ScaleY = unitView.transform.localScale.y;
                unit.AttackCoolTimeCutCost = unitView.AttackCoolTimeCutCost;
                structure.SquareUnits.Add(unit);
            }

            return structure;
        }

        public SquareStructureView DataToView(SquareStructure structure, Transform parent = null) {
            GameObject go;
            if(parent == null) {
                go = Instantiate(structurePrefab);
            } else {
                go = Instantiate(structurePrefab, parent);
            }
            
            return DataToViewInternal(go, structure);
        }

        public SquareStructureView DataToView(SquareStructure structure, Vector3 position, Transform parent = null) {
            GameObject go;
            if(parent == null) {
                go = Instantiate(structurePrefab, position, Quaternion.identity);
            } else {
                go = Instantiate(structurePrefab, position, Quaternion.identity, parent);
            }

            return DataToViewInternal(go, structure);
        }

        private SquareStructureView DataToViewInternal(GameObject structureViewGameObject, SquareStructure structure) {
            SquareStructureView structureView = structureViewGameObject.GetComponent<SquareStructureView>();
            structureView.HpCost = structure.HpCost;
            structureView.Hp = structureView.HpMax;

            foreach(SquareUnit unit in structure.SquareUnits) {
                SquareUnitView unitView = Instantiate(unitPrefab, structureView.transform).GetComponent<SquareUnitView>();
                unitView.transform.localPosition = unit.Position;
                unitView.transform.rotation = Quaternion.Euler(0, 0, unit.RotationZ);
                // 旧バージョン
                if(unit.Scale != 1) {
                    unitView.transform.localScale = new Vector3(unit.Scale, unit.Scale, unit.Scale);
                // 新バージョン
                } else {
                    unitView.transform.localScale = new Vector3(unit.ScaleX, unit.ScaleY, 1);
                }
                unitView.AttackCost = unit.AttackCost;
                unitView.DefenceCost = unit.DefenceCost;
                unitView.AttackCoolTimeCutCost = unit.AttackCoolTimeCutCost;
                structureView.JointUnit(unitView);
            }

            return structureView;
        }
    }
}