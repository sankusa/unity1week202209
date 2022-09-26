using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sankusa.unity1week202209.MainScene.View {
    public class BattleHistoryRecordView : MonoBehaviour
    {
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image background;

        public void SetValues(bool sendBattle, string playerName, int result, long score) {
            if(sendBattle) {
                categoryText.text = "攻撃";
                background.color = new Color(1, 0, 0, 0.2f);
            } else {
                categoryText.text = "防御";
                background.color = new Color(0, 0, 1, 0.2f);
            }
            playerNameText.text = playerName;
            if(result == 1) {
                resultText.text = "勝利";
                resultText.color = Color.red;
            } else if(result == -1) {
                resultText.text = "敗北";
                resultText.color = Color.blue;
            } else {
                resultText.text = "引分";
                resultText.color = Color.green;
            }
            scoreText.text = score > 0 ? "+" + score.ToString() : score.ToString();
        }
    }
}