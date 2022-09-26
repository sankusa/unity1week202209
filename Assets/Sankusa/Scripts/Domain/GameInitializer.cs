using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sankusa.unity1week202209.Constant;
using SankusaLib.SoundLib;

namespace Sankusa.unity1week202209.Domain {
    public class GameInitializer : IInitializable
    {
        private SaveData saveData;
        private SquareStructureStorage structureStorage;

        [Inject]
        public GameInitializer(SaveData saveData, SquareStructureStorage structureStorage) {
            this.saveData = saveData;
            this.structureStorage = structureStorage;
        }

        public void Initialize() {
            if(!PlayerPrefs.HasKey(GameConstant.SAVE_KEY)) {
                SoundManager.Instance.MasterVolume = 0.7f;
                for(int i = 0; i < GameConstant.SQUARE_STRUCTURE_STORAGE_CAPACITY; i++) {
                    structureStorage.SquareStructures.Add(SquareStructure.Default);
                }

                saveData.Save(GameConstant.SAVE_KEY);
            } else {
                saveData.Load(GameConstant.SAVE_KEY);
            }
        }
    }
}