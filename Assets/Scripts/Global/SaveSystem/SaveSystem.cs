using System;
using System.Collections.Generic;
using Global.SaveSystem.SavableObjects;
using UnityEngine;

namespace Global.SaveSystem
{
    public class SaveSystem
    {
        private readonly Dictionary<SavableObjectType, ISavable> _savableObjects;

        public SaveSystem()
        {
            _savableObjects = new() {
                { SavableObjectType.Wallet, new Wallet() },
                { SavableObjectType.Progress, new Progress() },
                { SavableObjectType.OpenedSkills, new OpenedSkills() },
                { SavableObjectType.BalanceProgress, new BalanceProgress() },
                { SavableObjectType.InfinityLevelStat, new InfinityLevelStat() },
            };
            //ResetAllData();
            CreateDefaultSavesIfNeeded();
            LoadData();
        }

        private void LoadData()
        {
            foreach (var (key, savableObject) in _savableObjects)
            {
                if (!PlayerPrefs.HasKey(key.ToString())) continue;
                var json = PlayerPrefs.GetString(key.ToString());
                JsonUtility.FromJsonOverwrite(json, savableObject);
            }
        }
        private void CreateDefaultSavesIfNeeded()
        {
            foreach (var (key, savableObject) in _savableObjects)
            {
                if (!PlayerPrefs.HasKey(key.ToString()))
                {
                    SaveData(key);
                }
            }
        }

        public ISavable GetData(SavableObjectType objectType)
        {
            return _savableObjects[objectType];
        }

        public void SaveData(SavableObjectType objectType)
        {
            var objectToSave = _savableObjects[objectType];
            var json = JsonUtility.ToJson(objectToSave);
            PlayerPrefs.SetString(objectType.ToString(), json);
            PlayerPrefs.Save();
        }

        public void SaveAll()
        {
            foreach (var (key, value) in _savableObjects)
            {
                var json = JsonUtility.ToJson(value);
                PlayerPrefs.SetString(key.ToString(), json);
            }

            PlayerPrefs.Save();
        }
        public void ResetAllData()
        {
            foreach (var key in _savableObjects.Keys)
            {
                PlayerPrefs.DeleteKey(key.ToString());
            }
            PlayerPrefs.Save();
            var keys = new List<SavableObjectType>(_savableObjects.Keys);
            foreach (var key in keys)
            {
                _savableObjects[key] = Activator.CreateInstance(_savableObjects[key].GetType()) as ISavable;
            }
            SaveAll();
        }

    }
}