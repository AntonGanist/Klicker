using System;

namespace Game.Configs.LevelConfigs
{
    [Serializable]
    public struct EnemySpawnData
    {
        public int Id;
        public bool IsBoss;
        public bool DropMutagen;
        public bool AutoLeveling;
    }
}