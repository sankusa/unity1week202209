using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Runtime.UIManager.Containers;
using SankusaLib.SoundLib;

namespace Sankusa.unity1week202209.BattleScene.View {
    public class ResultPerformer : MonoBehaviour
    {
        [SerializeField] private UIView view;
        [SerializeField] private TMP_Text winText;
        [SerializeField] private TMP_Text loseText;
        [SerializeField] private TMP_Text drawText;
        [SerializeField] private TMP_Text rateText;
        [SerializeField, SoundId] private string seId;

        public bool IsPlaying => view.isShowing || view.isHiding;

        public void PlayWinPerformance(long score) {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
            drawText.gameObject.SetActive(false);
            rateText.text = "レート " + (score >= 0 ? "+" + score.ToString() : score.ToString());
            view.Show();
            SoundManager.Instance.PlaySe(seId);
        }

        public void PlayLosePerformance(long score) {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);
            drawText.gameObject.SetActive(false);
            rateText.text = "レート " + (score >= 0 ? "+" + score.ToString() : score.ToString());
            view.Show();
            SoundManager.Instance.PlaySe(seId);
        }

        public void PlayDrawPerformer() {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);
            drawText.gameObject.SetActive(true);
            rateText.text = "";
            view.Show();
            SoundManager.Instance.PlaySe(seId);
        }
    }
}