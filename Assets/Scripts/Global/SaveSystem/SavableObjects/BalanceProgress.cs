using System;
using System.Collections.Generic;

namespace Global.SaveSystem.SavableObjects
{
    [Serializable]
    public class BalanceProgress : ISavable
    {
        public List<LocationData> Locations = new List<LocationData>();

        [Serializable]
        public class LocationData
        {
            public List<LevelData> Levels = new List<LevelData>();
        }

        [Serializable]
        public class LevelData
        {
            public float Health;
            public float Damage;
            public float AttackSpeed;
            public float Reward;
            public bool WasVisited;
            public bool CanDropMutagen = true;
        }
    }
}