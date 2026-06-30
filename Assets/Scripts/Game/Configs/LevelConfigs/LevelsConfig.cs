using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Game.Configs.LevelConfigs
{
    [CreateAssetMenu(menuName = "Configs/LevelsConfig", fileName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels;

        private Dictionary<int, Dictionary<int, LevelData>> _levelsMap;

        public LevelData GetLevel(int location, int level)
        {
            if (_levelsMap.IsNullOrEmpty()) FillLevelMap();
            return _levelsMap[location][level];
        }

        public int GetMaxLevelOnLocation(int location)
        {
            if (_levelsMap.IsNullOrEmpty()) FillLevelMap();

            if (!_levelsMap.ContainsKey(location))
            {
                Debug.LogError($"No levels found for location {location}");
                return 0;
            }

            var maxLevel = 0;
            foreach (var levelNumber in _levelsMap[location].Keys)
            {
                if (levelNumber > maxLevel)
                    maxLevel = levelNumber;
            }
            return maxLevel;
        }

        public Vector2Int GetMaxLocationAndLevel()
        {
            if (_levelsMap.IsNullOrEmpty()) FillLevelMap();
            if (_levelsMap.Count == 0) return Vector2Int.zero;

            var maxLocation = 0;
            var maxLevel = 0;

            foreach (var location in _levelsMap.Keys)
            {
                if (location > maxLocation)
                {
                    maxLocation = location;
                    maxLevel = 0; 
                }

                foreach (var level in _levelsMap[location].Keys)
                {
                    if (level > maxLevel)
                        maxLevel = level;
                }
            }

            return new Vector2Int(maxLocation, maxLevel);
        }

        private void FillLevelMap()
        {
            _levelsMap = new();

            foreach (var levelData in _levels)
            {
                var locationMap = _levelsMap.GetOrCreate(levelData.Location);
                locationMap[levelData.LevelNumber] = levelData;
            }
        }
    }
}
