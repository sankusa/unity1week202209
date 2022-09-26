using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sankusa.unity1week202209.View;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using Sankusa.unity1week202209.Constant;
using UniRx;

namespace Sankusa.unity1week202209.MainScene.View {
    public class SquareUnitEditView : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform squareUnitGeneratePosition;
        public Vector2 SquareUnitGeneratePosition => squareUnitGeneratePosition.position;

        [Header("ステータスUI")]
        [SerializeField] private UISlider rotationSlider;
        [SerializeField] private TMP_Text rotationText;
        [SerializeField] private UISlider scaleSlider;
        [SerializeField] private TMP_Text scaleText;
        [SerializeField] private UISlider attackSlider;
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private UISlider defenceSlider;
        [SerializeField] private TMP_Text defenceText;
        [SerializeField] private UISlider scaleXSlider;
        [SerializeField] private TMP_Text scaleXText;
        [SerializeField] private UISlider scaleYSlider;
        [SerializeField] private TMP_Text scaleYText;
        [SerializeField] private UISlider attackCoolTimeCutSlider;
        [SerializeField] private TMP_Text attackCoolTimeCutText;
        [SerializeField] private TMP_Text attackCoolTimeText;

        private SquareUnitView unitView;
        public SquareUnitView UnitView => unitView;
        private Drag unitViewDrag;
        public Drag UnitViewDrag => unitViewDrag;

        void Start() {
            rotationSlider.minValue = -180;
            rotationSlider.maxValue = 180;
            rotationSlider.OnValueChangedCallback.AddListener(x => {
                unitView.transform.rotation = Quaternion.Euler(0, 0, x);
                rotationText.text = x.ToString();
            });

            scaleSlider.maxValue = 100;
            scaleSlider.OnValueChangedCallback.AddListener(x => {
                float scale = x / 100f + 1;
                unitView.transform.localScale = new Vector3(scale, scale, scale);
                scaleText.text = scale.ToString("0.00");
            });

            scaleXSlider.maxValue = 100;
            scaleXSlider.OnValueChangedCallback.AddListener(x => {
                float scaleX = x / 100f + 1;
                unitView.transform.localScale = new Vector3(scaleX, unitView.transform.localScale.y, unitView.transform.localScale.z);
                scaleXText.text = scaleX.ToString("0.00");
            });

            scaleYSlider.maxValue = 100;
            scaleYSlider.OnValueChangedCallback.AddListener(x => {
                float scaleY = x / 100f + 1;
                unitView.transform.localScale = new Vector3(unitView.transform.localScale.x, scaleY, unitView.transform.localScale.z);
                scaleYText.text = scaleY.ToString("0.00");
            });

            attackSlider.maxValue = GameConstant.ATTACK_COST_MAX;
            attackSlider.OnValueChangedCallback.AddListener(x => {
                unitView.AttackCost = (int)x;
                attackText.text = unitView.Attack.ToString();
                
                attackCoolTimeCutSlider.maxValue = Mathf.Ceil(x * GameConstant.ATTACK_COOL_TIME_CUT_COST_MAX_PER_ATTACK_COST_MAX);
                attackCoolTimeCutSlider.value = Mathf.Clamp(attackCoolTimeCutSlider.value, 0, attackCoolTimeCutSlider.maxValue);
                attackCoolTimeCutSlider.OnValueChangedCallback.Invoke(attackCoolTimeCutSlider.value);
            });

            defenceSlider.maxValue = GameConstant.DEFENCE_COST_MAX;
            defenceSlider.OnValueChangedCallback.AddListener(x => {
                unitView.DefenceCost = (int)x;
                defenceText.text = unitView.Defence.ToString();
            });

            attackCoolTimeCutSlider.maxValue = 0;
            attackCoolTimeCutSlider.OnValueChangedCallback.AddListener(x => {
                unitView.AttackCoolTimeCutCost = (int)x;
                attackCoolTimeCutText.text = (unitView.AttackCoolTimeCut * 100).ToString("0") + "%";
            });

            this.ObserveEveryValueChanged(_ => unitView != null ? unitView.AttackCoolTimeMax : 0).Subscribe(x => attackCoolTimeText.text = x.ToString("0.00") + "s").AddTo(this);
        }

        public GameObject GenerateNewSquareUnit() {
            GameObject go = Instantiate(unitPrefab, squareUnitGeneratePosition);
            go.layer = 0;
            unitView = go.GetComponent<SquareUnitView>();
            go.AddComponent<EventTrigger>();
            unitViewDrag = go.AddComponent<Drag>();
            unitView.Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            unitView.Rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // 各スライダーリセット
            rotationSlider.value = 0;
            rotationSlider.OnValueChangedCallback.Invoke(rotationSlider.value);
            scaleSlider.value = 0;
            scaleSlider.OnValueChangedCallback.Invoke(scaleSlider.value);
            attackSlider.value = 0;
            attackSlider.OnValueChangedCallback.Invoke(attackSlider.value);
            defenceSlider.value = 0;
            defenceSlider.OnValueChangedCallback.Invoke(defenceSlider.value);
            scaleXSlider.value = 0;
            scaleXSlider.OnValueChangedCallback.Invoke(scaleXSlider.value);
            scaleYSlider.value = 0;
            scaleYSlider.OnValueChangedCallback.Invoke(scaleYSlider.value);
            attackCoolTimeCutSlider.value = 0;
            attackCoolTimeCutSlider.OnValueChangedCallback.Invoke(attackCoolTimeCutSlider.value);

            return go;
        }

        public void Clear() {
            int childCount = squareUnitGeneratePosition.childCount;
            for(int i = childCount - 1; i >= 0; i--) {
                Destroy(squareUnitGeneratePosition.GetChild(i).gameObject);
            }
            unitView = null;
        }
    }
}