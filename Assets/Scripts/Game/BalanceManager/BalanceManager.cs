using UnityEngine;
using Game.Configs.LevelConfigs;
using Global.SaveSystem.SavableObjects;
using Global.SaveSystem;
using System.Linq;

public class BalanceManager : MonoBehaviour
{
    [SerializeField] private LevelsConfig _levelsConfig;
    [SerializeField] private int _baseReward = 5;
    [SerializeField] private float _rewardProgression = 1.2f;
    [SerializeField] private int _bossRewardMultiplier = 3;
    [SerializeField] private float _firstLevelMultiplier = 0.4f;
    [SerializeField] private float _levelProgressionMultiplier = 1.15f;

    private Formuls _formuls;
    private SaveSystem _saveSystem;
    private BalanceProgress _balanceProgress;

    public void Initialize(SaveSystem saveSystem, Formuls formuls)
    {
        _saveSystem = saveSystem;
        _formuls = formuls;
        _balanceProgress = (BalanceProgress)saveSystem.GetData(SavableObjectType.BalanceProgress) ?? new BalanceProgress();
        if (_balanceProgress.Locations.Count == 0)
        {
            InitializeFirstLocation();
        }
       /* for (int i = 0; i < _balanceProgress.Locations.Count; i++)
        {
            for (int j = 0; j < _balanceProgress.Locations[i].Levels.Count; j++)
            {
                Debug.Log($"лока {i + 1}, уровень {j + 1}, хп {_balanceProgress.Locations[i].Levels[j].Health}," +
                    $"урон {_balanceProgress.Locations[i].Levels[j].Damage}," +
                    $"скорость {_balanceProgress.Locations[i].Levels[j].AttackSpeed}," +
                    $"награда {_balanceProgress.Locations[i].Levels[j].Reward}," +
                    $" был? {_balanceProgress.Locations[i].Levels[j].WasVisited}");
            }
        }*/
    }

    private void InitializeFirstLocation()
    {
        _balanceProgress.Locations.Clear();
        var firstLocation = new BalanceProgress.LocationData();
        int firstLocationLevels = _levelsConfig.GetMaxLevelOnLocation(1);
        firstLocation.Levels.Add(new BalanceProgress.LevelData
        {
            Health = 90 * _firstLevelMultiplier,
            Damage = 5 * _firstLevelMultiplier,
            AttackSpeed = _formuls.CalculateBossAttackSpeed(),
            Reward = CalculateLevelReward(1, 1, false),
            WasVisited = false
        });
        for (int lvl = 2; lvl < firstLocationLevels; lvl++)
        {
            firstLocation.Levels.Add(CalculateRegularLevel(1, lvl, firstLocation.Levels[lvl - 2]));
        }
        firstLocation.Levels.Add(new BalanceProgress.LevelData
        {
            WasVisited = false,
            Reward = CalculateLevelReward(1, firstLocationLevels, true)
        });
        _balanceProgress.Locations.Add(firstLocation);
        _saveSystem.SaveData(SavableObjectType.BalanceProgress);
    }

    public void OnBossDefeated(int location, int level)
    {
        if (location < 1 || location > _balanceProgress.Locations.Count) return;
        var locationData = _balanceProgress.Locations[location - 1];
        int levelCount = locationData.Levels.Count;
        if (!locationData.Levels[levelCount - 1].WasVisited)
        {
            var enemyLevel = _levelsConfig.GetLevel(location, level).Enemies[0];
            if (enemyLevel.AutoLeveling)
            {
                var bossStats = _formuls.CalculateBossStats();
                locationData.Levels[levelCount - 1] = new BalanceProgress.LevelData
                {
                    Health = bossStats.Health,
                    Damage = bossStats.Damage,
                    AttackSpeed = bossStats.AttackSpeed,
                    Reward = CalculateLevelReward(location, levelCount, true),
                    WasVisited = true
                };
            }
            else
            {
                var previousLevel = locationData.Levels[levelCount - 2];
                locationData.Levels[levelCount - 1] = new BalanceProgress.LevelData
                {
                    Health = previousLevel.Health * 1.5f,
                    Damage = previousLevel.Damage * 1.5f,
                    AttackSpeed = Mathf.Min(previousLevel.AttackSpeed * 0.9f, _formuls.MaxAttackSpeed),
                    Reward = CalculateLevelReward(location, levelCount, true),
                    WasVisited = true
                };
            }

        }
        if (location < _levelsConfig.GetMaxLocationAndLevel().x)
        {

            InitializeNextLocation(location);
        }

        _saveSystem.SaveData(SavableObjectType.BalanceProgress);
    }

    private void InitializeNextLocation(int previousLocation)
    {
        int newLocation = previousLocation + 1;
        if (_balanceProgress.Locations.Count >= newLocation) return;
        var newLocationData = new BalanceProgress.LocationData();
        int levelCount = _levelsConfig.GetMaxLevelOnLocation(newLocation);
        var previousBoss = _balanceProgress.Locations[previousLocation - 1].Levels.Last();
        newLocationData.Levels.Add(new BalanceProgress.LevelData
        {
            Health = previousBoss.Health * 0.7f,
            Damage = previousBoss.Damage * 0.7f,
            AttackSpeed = previousBoss.AttackSpeed,
            Reward = CalculateLevelReward(newLocation, 1, false),
            WasVisited = false
        });
        for (int lvl = 2; lvl < levelCount; lvl++)
        {
            newLocationData.Levels.Add(CalculateRegularLevel(newLocation, lvl, newLocationData.Levels[lvl - 2]));
        }
        newLocationData.Levels.Add(new BalanceProgress.LevelData
        {
            WasVisited = false,
            Reward = CalculateLevelReward(newLocation, levelCount, true)
        });

        _balanceProgress.Locations.Add(newLocationData);
    }

    private BalanceProgress.LevelData CalculateRegularLevel(int location, int level, BalanceProgress.LevelData previousLevel)
    {
        return new BalanceProgress.LevelData
        {
            Health = previousLevel.Health * _levelProgressionMultiplier,
            Damage = previousLevel.Damage * _levelProgressionMultiplier,
            AttackSpeed = Mathf.Min(previousLevel.AttackSpeed * 1.05f, _formuls.MaxAttackSpeed),
            Reward = CalculateLevelReward(location, level, false),
            WasVisited = false
        };
    }

    private int CalculateLevelReward(int location, int level, bool isBoss)
    {
        int baseReward = Mathf.RoundToInt(_baseReward * Mathf.Pow(_rewardProgression, level - 1));
        int locationBonus = (location - 1) * 5;
        int totalReward = baseReward + locationBonus;

        if (isBoss) totalReward *= _bossRewardMultiplier;

        return Mathf.Max(5, Mathf.RoundToInt(totalReward / 5f) * 5);
    }

    public void MarkLevelVisited(int location, int level)
    {
        if (location < 1 || location > _balanceProgress.Locations.Count ||
            level < 1 || level > _balanceProgress.Locations[location - 1].Levels.Count)
            return;

        var levelData = _balanceProgress.Locations[location - 1].Levels[level - 1];
        levelData.WasVisited = true;

        bool isBossLevel = level == _levelsConfig.GetMaxLevelOnLocation(location);

        if (isBossLevel && levelData.Health <= 0)
        {
            var enemyLevel = _levelsConfig.GetLevel(location, level).Enemies[0];
            if (enemyLevel.AutoLeveling)
            {
                var bossStats = _formuls.CalculateBossStats();
                levelData.Health = bossStats.Health;
                levelData.Damage = bossStats.Damage;
                levelData.AttackSpeed = bossStats.AttackSpeed;
            }
            else
            {
                var previousLevel = _balanceProgress.Locations[location - 1].Levels[level - 2];

                levelData.Health = previousLevel.Health * 1.5f;
                levelData.Damage = previousLevel.Damage * 1.5f;
                levelData.AttackSpeed = Mathf.Min(previousLevel.AttackSpeed, 
                    _formuls.MaxAttackSpeed);
            }
        }

        _saveSystem.SaveData(SavableObjectType.BalanceProgress);
    }
    public float GetLevelPower(int location, int level, int typeData)
    {
        if (location < 1 || location > _balanceProgress.Locations.Count ||
            level < 1 || level > _balanceProgress.Locations[location - 1].Levels.Count)
            return 1f;

        var levelData = _balanceProgress.Locations[location - 1].Levels[level - 1];

        return typeData switch
        {
            0 => levelData.Health,
            1 => levelData.Damage,
            2 => levelData.AttackSpeed,
            3 => levelData.Reward,
            _ => 1f
        };
    }

    public bool CheckDropMutagen(int location, int level)
    {
        var levelData = _balanceProgress.Locations[location - 1].Levels[level - 1];
        return levelData.CanDropMutagen;
    }
    public void CantDropMutagen(int location, int level)
    {
        var levelData = _balanceProgress.Locations[location - 1].Levels[level - 1];
        levelData.CanDropMutagen = false;
    }
}