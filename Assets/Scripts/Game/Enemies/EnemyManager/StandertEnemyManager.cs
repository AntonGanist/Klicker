using System.Collections;
using Game.Configs.LevelConfigs;
using UnityEngine;

namespace Game.Enemies
{
    public class StandertEnemyManager : EnemyManager
    {
        [SerializeField] private LevelsConfig _levelsConfig;
        [SerializeField] private HowManyEnemies _howManyEnemies;
        private LevelData _levelData;

        private int _level;

        public override void StartLevel(LevelData levelData, int location, int level)
        {
            _levelData = levelData;
            _location = location;
            _level = level;
            _currentEnemyIndex = -1;

            _balanceManager.MarkLevelVisited(location, level);

            int bossIndex = _levelData.Enemies.FindIndex(e => e.IsBoss);
            _howManyEnemies.Initialize(_levelData.Enemies.Count, bossIndex);
            int count = _levelData.Enemies.Count;

            _health = _balanceManager.GetLevelPower(_location, _level, 0);
            _damage = _balanceManager.GetLevelPower(_location, _level, 1);
            _damageSpeed = _balanceManager.GetLevelPower(_location, _level, 2);
            _reward = _balanceManager.GetLevelPower(_location, _level, 3);
            if (count > 1)
            {
                _health /= count;
                _damage /= count;
                _reward /= count;
            }

            _lootManager.TakeLocation(location);

            if (_currentEnemyMonoBehaviour == null)
            {
                _currentEnemyMonoBehaviour = Instantiate(_enemiesConfig.EnemyPrefab, _enemyContainer);
                _currentEnemyMonoBehaviour.OnDead += SpawnEnemy;
                _currentEnemyMonoBehaviour.OnChangeHealthBar += ChangeHealthBar;
            }
            SpawnEnemy();
        }
        private void SpawnEnemy()
        {
            if (_currentEnemyIndex != -1)
            {
                EventBus.Raise(new EnemyDead(_currentEnemyIndex));
            }
            StartCoroutine(SpawnEnemyCorutine());
        }
        private IEnumerator SpawnEnemyCorutine()
        {
            _currentEnemyIndex++;
            string bossName = "";
            if (_currentEnemyIndex != 0)
            {
                bool wasBoss = _levelData.Enemies[_currentEnemyIndex - 1].IsBoss;
                _howManyEnemies.UpdateEnemyDefeated(_currentEnemyIndex - 1, wasBoss);

                _lootManager.DropMoney((int)_reward);
                if (_levelData.Enemies[_currentEnemyIndex - 1].DropMutagen
                    && _balanceManager.CheckDropMutagen(_location, _level))
                {
                    _balanceManager.CantDropMutagen(_location, _level);
                    _lootManager.DropMutagen();
                }
            }
            if (_currentEnemyIndex >= _levelData.Enemies.Count)
            {
                yield return StartCoroutine(EndLevel());
                yield break;
            }
            var currentEnemy = _levelData.Enemies[_currentEnemyIndex];
            if (currentEnemy.IsBoss)
            {
                bossName = "аняя ";
            }
            var currentEnemyViewData = _enemiesConfig.GetEnemy(currentEnemy.Id);

            _enemyViewData = currentEnemyViewData;
            InitHpBar(_health, bossName + currentEnemyViewData.Name,
                currentEnemyViewData.PositionHealthBar);

            Transform prefab = Instantiate(currentEnemyViewData.Prefab);
            _currentEnemyMonoBehaviour.Initialize(_health, _damage,
                _damageSpeed, currentEnemyViewData, prefab, _dopOilDamage);
            _currentEnemyMonoBehaviour.OnHideHpBar += _healthBar.Hide;
            _oilDamageType = _enemyViewData.VulnerabilityToOil;
            EventBus.Raise(new SpawnEnemy());
        }    
        private IEnumerator EndLevel()
        {
            bool endLevel = false;
            while (endLevel == false)
            {
                endLevel = !_lootManager.GetActive();
                yield return null;
            }
            bool isBossDefeated = _levelData.Enemies[_currentEnemyIndex - 1].IsBoss;
            if (isBossDefeated)
            {
                _balanceManager.OnBossDefeated(_location, _level);
                int locationReward = _formuls.CalculateLocationReward(_location);
                if (_location < _levelsConfig.GetMaxLocationAndLevel().x)
                {
                    _formuls.UpdateConsumablesPrices(locationReward, _skillCounter, _levelsConfig, _location, _level);
                    _skillCounter.Save();
                }
            }

            EventBus.Raise(new LevelPassed(true, false));
            StopAllCoroutines();
        }
        public override void StartLevel()
        {
            throw new System.NotImplementedException();
        }
    }
}