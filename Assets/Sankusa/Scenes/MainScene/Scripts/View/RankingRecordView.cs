using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Runtime.UIManager.Components;
using Sankusa.unity1week202209.Domain;
using SankusaLib;
using UnityEngine.Events;
using Zenject;
using System.Linq;

namespace Sankusa.unity1week202209.MainScene.View {
    public class RankingRecordView : MonoBehaviour
    {
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private UIButton battleButton;

        public void SetRecord(int rank, RankingRecord record, bool buttonActive, bool buttonInteractive, UnityAction<RankingRecord> onClick = null) {
            rankText.text = rank.ToString();
            playerNameText.text = record.Name;
            scoreText.text = record.Score.ToString();
            battleButton.gameObject.SetActive(buttonActive);
            battleButton.interactable = buttonInteractive;
            battleButton.AddListenerToPointerClick(() => onClick?.Invoke(record));
        }

        public void SetNone() {
            rankText.text = "";
            playerNameText.text = "";
            scoreText.text = "";
            battleButton.gameObject.SetActive(false);
            battleButton.AddListenerToPointerClick(() => {});
        }
    }
}