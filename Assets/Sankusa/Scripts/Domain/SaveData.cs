using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;
using SankusaLib;

namespace Sankusa.unity1week202209.Domain {
    [Serializable]
    public class SaveData
    {
        [SerializeField] private PlayerInfo playerInfo; 
        [SerializeField] private SquareStructureStorage squareStructureStorage;

        [Inject]
        public SaveData(PlayerInfo playerInfo, SquareStructureStorage squareStructureStorage) {
            this.playerInfo = playerInfo;
            this.squareStructureStorage = squareStructureStorage;
        }

        public void Load(string key) {
            // ※PlayerPrefs.Get + JsonUtility.FromJsonOverWrite
            JsonSave.LoadOverWrite(key, this);
        }

        public void Save(string key) {
            // ※JsonUtility.ToJson + ※PlayerPrefs.Set
            JsonSave.Save(key, this);
        }
    }
}