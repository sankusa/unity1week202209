using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using Sankusa.unity1week202209.View;
using UnityEngine.EventSystems;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using UniRx;
using Cysharp.Threading.Tasks;
using SankusaLib;
using UnityEngine.Events;
using System;
using TMPro;
using SankusaLib.SoundLib;

namespace Sankusa.unity1week202209.MainScene.View {
    public class SquareStructureEditView : MonoBehaviour
    {
        [SerializeField] private GameObject structurePrefab;
        [SerializeField] private SquareUnitEditView unitEditView;
        [SerializeField] private UIView unitEditUiView;
        [SerializeField] private UIView structureEditUiView;
        [SerializeField] private UIView structureGenerateUiView;
        [SerializeField] private UIButton unitEditFinishButton;
        [SerializeField] private UIButton structureEditFinishButton;
        [SerializeField] private Transform structureGeneratePosition;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text unitCountText;
        [SerializeField] private UIButton cancelButton;
        [SerializeField] private UIButton undoButton;
        [Header("ステータスUI")]
        [SerializeField] private UISlider hpSlider;
        [SerializeField] private TMP_Text hpText;
        [Header("その他")]
        [SerializeField, SoundId] private string putSeId;
        [SerializeField] private GameObject connectEffectPrefab;

        private SquareStructureView structureView;

        private UnityEvent<SquareStructure> onFinishEdit = new UnityEvent<SquareStructure>();
        public IObservable<SquareStructure> OnFinishEdit => onFinishEdit.AsObservable();

        private UnityEvent onCancel = new UnityEvent();
        public IObservable<Unit> OnCancel => onCancel.AsObservable();

        private int unitCount = 0;

        void Start() {
            this.ObserveEveryValueChanged(_ => unitCount).Subscribe(x => {
                if(x == GameConstant.SQUARE_UNIT_COUNT_MAX) {
                    unitEditUiView.Hide();
                    structureEditUiView.Show();
                } else {
                    unitEditUiView.Show();
                    structureEditUiView.Hide();
                }
            }).AddTo(this);
            structureEditFinishButton.AddListenerToPointerClick(() => {
                FinishEdit();
            });
            cancelButton.AddListenerToPointerClick(() => {
                FinishEdit(true);
            });
            undoButton.AddListenerToPointerClick(() => {
                Undo();
            });
            this.ObserveEveryValueChanged(_ => unitCount).Subscribe(_ => unitCountText.text = (unitCount + 1).ToString() + "/" + GameConstant.SQUARE_UNIT_COUNT_MAX.ToString()).AddTo(this);
            // ステータスUI
            hpSlider.maxValue = GameConstant.HP_COST_MAX;
            hpSlider.OnValueChangedCallback.AddListener(x => {
                structureView.HpCost = (int)x;
                hpText.text = structureView.HpMax.ToString();
                
            });
            this.ObserveEveryValueChanged(_ => {
                return (structureView != null ? structureView.TotalCost : 0) + (unitEditView.UnitView != null ? unitEditView.UnitView.TotalCost: 0);
            }).Subscribe(x => {
                int remainingCost = GameConstant.COST_MAX - x;
                costText.text = (remainingCost).ToString();
                if(remainingCost >= 0) {
                    costText.color = Color.white;
                    structureEditFinishButton.interactable = true;
                    if(unitEditView.UnitViewDrag != null) unitEditView.UnitViewDrag.enabled = true;
                } else {
                    costText.color = Color.red;
                    structureEditFinishButton.interactable = false;
                    if(unitEditView.UnitViewDrag != null) unitEditView.UnitViewDrag.enabled = false;
                }
            }).AddTo(this);
        }

        public void StartEdit(UnityAction<SquareStructure> onFinishEdit = null, UnityAction onCancel = null) {
            if(onFinishEdit != null) this.onFinishEdit.AddListener(onFinishEdit);
            if(onCancel != null) this.onCancel.AddListener(onCancel);
            structureView = Instantiate(structurePrefab, structureGeneratePosition).GetComponent<SquareStructureView>();
            structureView.Rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // ステータスUI初期化,イベント(初回)手動発火
            unitCount = 0;
            hpSlider.value = 0;
            hpSlider.OnValueChangedCallback.Invoke(hpSlider.value);

            unitEditUiView.Show();
            structureGenerateUiView.Show();

            var _ = UniTask.Create(async () => {
                await UniTask.WaitUntil(() => !unitEditUiView.isShowing);
                EditFirstUnit();
            });
        }

        public void FinishEdit(bool cancel = false) {
            if(cancel) {
                onCancel.Invoke();
            } else {
                onFinishEdit.Invoke(SquareStructureConverter.ViewToData(structureView));
            }
            onFinishEdit.RemoveAllListeners();
            onCancel.RemoveAllListeners();
            unitEditView.Clear();
            unitEditUiView.Hide();
            structureEditUiView.Hide();
            structureGenerateUiView.Hide();
            Destroy(structureView.gameObject);
        }

        private void EditFirstUnit() {
            GameObject go = unitEditView.GenerateNewSquareUnit();
            // 同レイヤーのブロック同士は衝突判定しない設定(プロジェクト設定)のため、設置時のみレイヤー変更
            go.layer = 0;
            SquareUnitView unit = go.GetComponent<SquareUnitView>();

            Drag drag = go.GetComponent<Drag>();

            drag.OnDragEnd.First().Subscribe(_ => {
                SoundManager.Instance.PlaySe(putSeId);
                Destroy(drag);
                Destroy(go.GetComponent<EventTrigger>());
                structureView.JointUnit(unit);
                structureView.Rb.constraints = RigidbodyConstraints2D.FreezePosition;
                EditNextUnit();
                go.transform.localPosition = Vector2.zero;
                unitCount++;
            }).AddTo(this);
        }

        private void EditNextUnit() {
            GameObject go = unitEditView.GenerateNewSquareUnit();
            // 同レイヤーのブロック同士は衝突判定しない設定(プロジェクト設定)のため、設置時のみレイヤー変更
            go.layer = 0;
            SquareUnitView unit = go.GetComponent<SquareUnitView>();

            Drag drag = go.GetComponent<Drag>();

            unit.OnCollisionEnter.TakeWhile(_ => drag.Dragging).Take(1).Subscribe(col => {
                SoundManager.Instance.PlaySe(putSeId);
                foreach(ContactPoint2D point in col.contacts) {
                    Instantiate(connectEffectPrefab, point.point, Quaternion.identity);
                    break;
                }
                Destroy(drag);
                Destroy(go.GetComponent<EventTrigger>());
                structureView.JointUnit(unit);
                structureView.Rb.constraints = RigidbodyConstraints2D.FreezePosition;
                unitCount++;
                if(unitCount == GameConstant.SQUARE_UNIT_COUNT_MAX) {
                    unitEditView.Clear();
                } else {
                    EditNextUnit();
                }
                // Instantiateと同じフレームだと、ValidationCheck内のCollider2D.Distanceの挙動がおかしくなるので遅延
                this.StartDelayCoroutine(0.05f, () => {
                    if(structureView.ValidationCheck() == false) Undo();
                });
            }).AddTo(this);

            // 設置可能エリア外
            unit.OnCollisionEnterOverWall.Subscribe(_ => {
                drag.Reset();
                go.transform.position = unitEditView.SquareUnitGeneratePosition;
            }).AddTo(this);

            // 設置せずに離した場合
            drag.OnDragEnd.Subscribe(_ => {
                go.transform.position = unitEditView.SquareUnitGeneratePosition;
            }).AddTo(this);
        }

        public void Undo() {
            if(unitCount == 0) return;
            if(unitCount > 0) {
                structureView.DestroyLastUnit();
                unitEditView.Clear();
                unitCount--;
                if(unitCount == 0) {
                    EditFirstUnit();
                } else {
                    EditNextUnit();
                }
            }
        }
    }
}