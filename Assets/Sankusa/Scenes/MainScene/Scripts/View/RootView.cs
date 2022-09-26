using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Components;
using SankusaLib;
using Sankusa.unity1week202209.Constant;
using Sankusa.unity1week202209.Domain;
using Sankusa.unity1week202209.View;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Sankusa.unity1week202209.MainScene.View {
    public class RootView : MonoBehaviour
    {   
        [SerializeField] private UIView view;
        [SerializeField] private UIButton createButton;
        [SerializeField] private UIButton uploadButton;
        [SerializeField] private UIButton battleButton;
        [SerializeField] private GameObject loadingCanvas;

        [Inject] private PlayerInfo playerInfo;
        [Inject] private SquareStructureStorage structureStorage;
        [Inject] private SaveData saveData;
        [Inject] private IRankingAccessor rankingAccessor;

        [Inject] private SquareStructureConverter converter;
        [Inject] private RankingView rankingView;
        [Inject] private SquareStructureEditView structureEditView;
        [Inject] private SquareStructureDisplayView structureDisplayView;

        void Start() {
            createButton.AddListenerToPointerClick(() => OnCreateButtonClick());
            uploadButton.AddListenerToPointerClick(() => {var _ = OnUploadButtonClick();});
            battleButton.AddListenerToPointerClick(() => rankingView.Show());
        }

        private void OnCreateButtonClick() {
            view.Hide();
            structureEditView.StartEdit(x => {
                Debug.Log(JsonUtility.ToJson(x));
                structureStorage.SquareStructures[structureDisplayView.DisplayIndex] = x;
                if(playerInfo.LastUploadedStructureIndex == structureDisplayView.DisplayIndex) playerInfo.LastUploadedStructureIndex = -1;
                view.Show();
                saveData.Save(GameConstant.SAVE_KEY);
            }
            ,() => {
                view.Show();
            });
        }

        private async UniTask OnUploadButtonClick() {
            bool success = false;
            loadingCanvas.SetActive(true);
            if(playerInfo.RankingKey == "") {
                RankingRecord record = new RankingRecord(GameConstant.PLAYER_NAME_DEFAULT, GameConstant.RATE_DEFAULT, structureStorage.SquareStructures[structureDisplayView.DisplayIndex]);
                success = await rankingAccessor.SaveRecord(record, this.GetCancellationTokenOnDestroy());
                if(success) playerInfo.RankingKey = record.Key;
            } else {
                success = await rankingAccessor.UpdateStructure(playerInfo.RankingKey, structureStorage.SquareStructures[structureDisplayView.DisplayIndex], this.GetCancellationTokenOnDestroy());
                playerInfo.LastUploadedStructureIndex = structureDisplayView.DisplayIndex;
            }
            if(success) saveData.Save(GameConstant.SAVE_KEY);
            else Debug.Log("Error");
            loadingCanvas.SetActive(false);
        }
    }
}