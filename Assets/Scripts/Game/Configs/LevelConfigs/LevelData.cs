using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Configs.LevelConfigs
{
    [Serializable]
    public struct LevelData
    {
        public Sprite Backgraund;
        public int Location;
        public int LevelNumber;
        public List<EnemySpawnData> Enemies;
    }
}