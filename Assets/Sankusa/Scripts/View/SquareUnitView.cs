using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using Sankusa.unity1week202209.Constant;
using UnityEngine.UI;

namespace Sankusa.unity1week202209.View {
    public class SquareUnitView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rb => rb;
        [SerializeField] private Collider2D col2D;
        public Collider2D Col2D => col2D;
        [SerializeField] private FixedJoint2D joint;
        public FixedJoint2D Joint => joint;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image attackImage;
        [SerializeField] private Image shieldImage;
        private UnityEvent<Collision2D> onCollisionEnter = new UnityEvent<Collision2D>();
        public IObservable<Collision2D> OnCollisionEnter => onCollisionEnter.AsObservable();
        private UnityEvent<Collision2D> onCollisionEnterOverWall = new UnityEvent<Collision2D>();
        public IObservable<Collision2D> OnCollisionEnterOverWall => onCollisionEnterOverWall.AsObservable();

        private float attackCoolTime = 0;
        public float AttackCoolTime {
            get => attackCoolTime;
            set => attackCoolTime = value;
        }

        private int attackCost = 0;
        public int AttackCost {
            get => attackCost;
            set => attackCost = value;
        }
        public int Attack => attackCoolTime == 0 ? (int)(GameConstant.ATTACK_BASE + GameConstant.ATTACK_RATE * attackCost) : 0;

        private int defenceCost = 0;
        public int DefenceCost {
            get => defenceCost;
            set => defenceCost = value;
        }
        public int Defence => (int)(GameConstant.DEFENCE_BASE + GameConstant.DEFENCE_RATE * defenceCost);

        private int attackCoolTimeCutCost = 0;
        public int AttackCoolTimeCutCost {
            get => attackCoolTimeCutCost;
            set => attackCoolTimeCutCost = value;
        }
        public float AttackCoolTimeCut => attackCost == 0 ? 0 : Mathf.Clamp01((float)attackCoolTimeCutCost / GameConstant.ATTACK_COOL_TIME_CUT_COST_MAX_PER_ATTACK_COST_MAX / attackCost) * GameConstant.ATTACK_COOL_TIME_CUT_MAX;

        public float AttackCoolTimeMax => GameConstant.ATTACK_COOL_TIME * attackCost * (1 - AttackCoolTimeCut);

        public int TotalCost => (int)((attackCost + defenceCost + attackCoolTimeCutCost) * (transform.localScale.x + transform.localScale.y) * 0.5f);

        void Start() {
            canvas.overrideSorting = true;
            this.ObserveEveryValueChanged(_ => attackCost).Subscribe(_ => attackImage.color = new Color(1, (1 - (float)attackCost / GameConstant.ATTACK_COST_MAX), (1 - (float)attackCost / GameConstant.ATTACK_COST_MAX))).AddTo(this);
            this.ObserveEveryValueChanged(_ => defenceCost).Subscribe(_ => shieldImage.color = new Color(shieldImage.color.r, shieldImage.color.g, shieldImage.color.b, (float)defenceCost / GameConstant.DEFENCE_COST_MAX)).AddTo(this);
            this.ObserveEveryValueChanged(_ => attackCoolTime).Subscribe(_ => attackImage.fillAmount = 1 - (attackCoolTime / AttackCoolTimeMax)).AddTo(this);
        }

        void Update() {
            attackCoolTime = Mathf.Max(0, attackCoolTime - Time.deltaTime);
        }

        void OnCollisionEnter2D(Collision2D col) {
            // 編集可能エリア外か判定(機体作成時)
            Collider2D[] colliders = new Collider2D[10];
            int colCount = col2D.OverlapCollider(new ContactFilter2D().NoFilter(), colliders);
            for(int i = 0; i < colCount; i++) {
                if(colliders[i].tag == "Wall") {
                    onCollisionEnterOverWall.Invoke(col);
                    return;
                }
            }

            // そのまま通す
            onCollisionEnter.Invoke(col);
        }
    }
}