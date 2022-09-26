using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.View;
using Sankusa.unity1week202209.BattleScene.Domain;
using TMPro;
using UniRx;
using Doozy.Runtime.UIManager.Components;

namespace Sankusa.unity1week202209.BattleScene.View {
    public class BattleStatusView : MonoBehaviour
    {
        [SerializeField] private Transform playerGeneratePosition;
        public Transform PlayerGeneratePosition => playerGeneratePosition;

        [SerializeField] private Transform enemyGeneratePosition;
        public Transform EnemyGeneratePosition => enemyGeneratePosition;

        [SerializeField] private TMP_Text timeText;

        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private UISlider playerHpSlider;
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text enemyNameText;
        [SerializeField] private UISlider enemyHpSlider;
        [SerializeField] private TMP_Text enemyHpText;

        [Inject] private BattleStatus battleStatus;

        void Start() {
            this.ObserveEveryValueChanged(_ => battleStatus.Time).Subscribe(time => timeText.text = time.ToString("0.0")).AddTo(this);
        }

        public void Initialize(string playerName, SquareStructureView playerStructureView, string enemyName, SquareStructureView enemyStructureView) {
            playerNameText.text = playerName;
            enemyNameText.text = enemyName;

            this.ObserveEveryValueChanged(_ => playerStructureView != null ? (float)playerStructureView.Hp / playerStructureView.HpMax : 0).Subscribe(x => playerHpSlider.value = x).AddTo(this);
            this.ObserveEveryValueChanged(_ => playerStructureView != null ? playerStructureView.Hp : 0).Subscribe(x => playerHpText.text = x.ToString()).AddTo(this);
                
            this.ObserveEveryValueChanged(_ => enemyStructureView != null ? (float)enemyStructureView.Hp / enemyStructureView.HpMax : 0).Subscribe(x => enemyHpSlider.value = x).AddTo(this);
            this.ObserveEveryValueChanged(_ => enemyStructureView != null ? enemyStructureView.Hp : 0).Subscribe(x => enemyHpText.text = x.ToString()).AddTo(this);
        }
    }
}