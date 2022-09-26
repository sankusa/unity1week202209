using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Sankusa.unity1week202209.Domain;
using Sankusa.unity1week202209.View;
using Doozy.Runtime.UIManager.Components;
using SankusaLib;
using DG.Tweening;
using TMPro;

namespace Sankusa.unity1week202209.MainScene.View {
    public class SquareStructureDisplayView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private TMP_Text slotText;
        [SerializeField] private TMP_Text uploadedText;
        [SerializeField] private UIButton leftButton;
        [SerializeField] private UIButton rightButton;

        [Inject] private PlayerInfo playerInfo;
        [Inject] private SquareStructureStorage structureStorage;
        [Inject] private SquareStructureConverter converter;

        private int displayIndex = 0;
        public int DisplayIndex {
            get => displayIndex;
            set => displayIndex = Mathf.Clamp(value, 0, structureStorage.SquareStructures.Count - 1);
        }

        void Start() {
            gridLayout.cellSize = new Vector2(Screen.width, Screen.height);

            leftButton.AddListenerToPointerClick(() => DisplayIndex -= 1);
            rightButton.AddListenerToPointerClick(() => DisplayIndex += 1);

            this.ObserveEveryValueChanged(_ => displayIndex).Subscribe(index => {
                gridLayout.transform.DOLocalMoveX(- gridLayout.cellSize.x * displayIndex - gridLayout.cellSize.x * 0.5f, 0.4f).SetLink(gameObject);
                slotText.text = "スロット " + (index + 1).ToString() + "/" + structureStorage.SquareStructures.Count;
                leftButton.gameObject.SetActive(!(index == 0));
                rightButton.gameObject.SetActive(!(index == structureStorage.SquareStructures.Count - 1));
                UpdateUploadedText();
            }).AddTo(this);

            this.ObserveEveryValueChanged(_ => playerInfo.LastUploadedStructureIndex).Subscribe(_ => {
                UpdateUploadedText();
            }).AddTo(this);

            this.ObserveEveryValueChanged(_ => structureStorage.SquareStructures.Count).Subscribe(count => {
                int childCount = gridLayout.transform.childCount;
                int additionalChildCount = count - childCount;

                if(additionalChildCount > 0) {
                    for(int i = 0; i < additionalChildCount;  i++) {
                        GameObject go = new GameObject();
                        go.AddComponent<RectTransform>();
                        go.transform.SetParent(gridLayout.transform);

                        int index = gridLayout.transform.childCount - 1;
                        this.ObserveEveryValueChanged(_ => structureStorage.SquareStructures[index]).Subscribe(x => {
                            for(int j = go.transform.childCount - 1; j >= 0; j--) {
                                Destroy(go.transform.GetChild(j).gameObject);
                            }
                            if(x != null) {
                                converter.DataToView(x, go.transform);
                            }
                        });
                    }
                } else if(additionalChildCount < 0) {
                    for(int i = childCount; i >= count; i--) {
                        Destroy(gridLayout.transform.GetChild(i).gameObject);
                    }
                }
            }).AddTo(this);

            if(0 <= playerInfo.LastUploadedStructureIndex && playerInfo.LastUploadedStructureIndex < structureStorage.SquareStructures.Count) {
                displayIndex = playerInfo.LastUploadedStructureIndex;
            } else {
                displayIndex = 0;
            }
        }

        private void UpdateUploadedText() {
            if(playerInfo.LastUploadedStructureIndex == displayIndex) {
                uploadedText.color = Color.white;
                uploadedText.text = "アップロード済み";
            } else {
                uploadedText.color = new Color(1, 0.35f, 0.35f);
                uploadedText.text = "未アップロード";
            }
        }
    }
}