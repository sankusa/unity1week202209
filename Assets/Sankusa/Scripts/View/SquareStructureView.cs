using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using System.Linq;
using SankusaLib;

using SankusaLib.SoundLib;

namespace Sankusa.unity1week202209.View {
    public class SquareStructureView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rb => rb;

        [SerializeField, SoundId] private string hitSeId;
        [SerializeField] private GameObject damageTextEffectPrefab;

        private List<SquareUnitView> squareUnits = new List<SquareUnitView>();
        public List<SquareUnitView> SquareUnits => squareUnits;

        private int hp = 0;
        public int Hp {
            get => hp;
            set => hp = Mathf.Max(0, value);
        }

        private int hpCost = 0;
        public int HpCost {
            get => hpCost;
            set => hpCost = value;
        }
        public int HpMax => (int)(GameConstant.HP_BASE + GameConstant.HP_RATE * hpCost);

        public int TotalCost => hpCost + squareUnits.Select(unit => unit.TotalCost).Sum();

        public void JointUnit(SquareUnitView unit) {
            squareUnits.Add(unit);
            unit.gameObject.layer = gameObject.layer;
            unit.transform.parent = transform;
            unit.Joint.connectedBody = rb;
            unit.OnCollisionEnter.Subscribe(col => {
                OnUnitCollisionEnter(unit, col);
            });

            this.StartDelayCoroutine(0.005f, () => {
                unit.Joint.enabled = true;
            });
        }

        public void SetLayer(int layer) {
            gameObject.layer = layer;
            foreach(Transform t in transform) {
                t.gameObject.layer = layer;
            }
        }

        public void AddForcePerMass(Vector2 force) {
            rb.AddForce(force * rb.mass);
            foreach(SquareUnitView unit in squareUnits) {
                unit.Rb.AddForce(force * unit.Rb.mass);
            }
        }

        private void OnUnitCollisionEnter(SquareUnitView unitView, Collision2D col) {
            // layer = 0 (機体作成時)はスルー
            if(unitView.gameObject.layer == 0 || col.gameObject.layer == 0) return;

            SquareUnitView enemyUnitView = col.gameObject.GetComponent<SquareUnitView>();
            if(enemyUnitView != null && unitView.gameObject.layer != col.gameObject.layer) {
                if(unitView.gameObject.layer == GameConstant.LAYER_SQUARE_UNIT_1) SoundManager.Instance.PlaySe(hitSeId);

                int damage = Mathf.Max(0, enemyUnitView.Attack - unitView.Defence);
                if(enemyUnitView.Attack > 0) enemyUnitView.AttackCoolTime = enemyUnitView.AttackCoolTimeMax;
                Hp -= damage;
                if(damage != 0) Instantiate(damageTextEffectPrefab, transform.position, Quaternion.identity).GetComponent<TextEffect2>().Text = damage.ToString();
            }
        }
        
        // ブロック設置Undo用
        public void DestroyLastUnit() {
            squareUnits.RemoveAt(squareUnits.Count - 1);
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        }

        public bool ValidationCheck() {
            for(int i = 0; i < squareUnits.Count; i++) {
                if(i == 0) {
                    if(Vector2.Distance(squareUnits[0].transform.localPosition,Vector3.zero) > GameConstant.ALLOWABLE_UNIT_DISTANCE_ERROR) return false;
                } else {
                    float distanceMin = float.MaxValue;
                    for(int j = 0; j < i; j++) {
                        float distance = squareUnits[i].Col2D.Distance(squareUnits[j].Col2D).distance;
                        if(distance < distanceMin) distanceMin = distance;
                    }
                    if(Mathf.Abs(distanceMin) > GameConstant.ALLOWABLE_UNIT_DISTANCE_ERROR) return false;
                }
            }
            return true;
        }
    }
}