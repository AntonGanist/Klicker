using UnityEngine;
using static Game.Configs.EnemyConfigs.EnemiesConfig;

namespace Game.Enemies
{
    public class InfinityEnemyManager : EnemyManager
    {
        private int _defeatEnemy = 0;
        private bool _dropMutagen;
        public override void StartLevel()
        {
            _location = 1;
            _currentEnemyIndex = -1;

            _balanceManager.MarkLevelVisited(_location, 1);

            _health = _balanceManager.GetLevelPower(_location, 1, 0) / 3;
            _damage = _balanceManager.GetLevelPower(_location, 1, 1) / 3;
            _damageSpeed = _balanceManager.GetLevelPower(_location, 1, 2) * 1.5f;
            _reward = _balanceManager.GetLevelPower(_location, 1, 3) / 3;

            _lootManager.TakeLocation(_location);

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
                _health *= 1.1f;
                _damage *= 1.05f;
                if(_damageSpeed > 0.8f)
                    _damageSpeed /= 1.05f;
                _reward *= 1.05f;
                EventBus.Raise(new EnemyDead(_currentEnemyIndex));
            }
            _defeatEnemy++;
            if(_defeatEnemy > 10)
            {
                _defeatEnemy = 0;
                _location++;
                int rand = Random.Range(1, 11);
                if(rand < 3)
                {
                    _dropMutagen = true;
                }
            }


            _currentEnemyIndex++;
            if (_currentEnemyIndex != 0)
            {
                _lootManager.DropMoney((int)_reward);
                if (_dropMutagen)
                {
                    _lootManager.DropMutagen();
                    _dropMutagen = false;
                }
            }
            var currentEnemyViewData = RandomEnemy();

            _enemyViewData = currentEnemyViewData;
            InitHpBar(_health, currentEnemyViewData.Name,
                currentEnemyViewData.PositionHealthBar);

            Transform prefab = Instantiate(currentEnemyViewData.Prefab);
            _currentEnemyMonoBehaviour.Initialize(_health, _damage,
                _damageSpeed, currentEnemyViewData, prefab, _dopOilDamage);
            _currentEnemyMonoBehaviour.OnHideHpBar += _healthBar.Hide;
            _oilDamageType = _enemyViewData.VulnerabilityToOil;
            EventBus.Raise(new SpawnEnemy());
        }
        private EnemyViewData RandomEnemy()
        {
            int rand = Random.Range(0, _enemiesConfig.Enemies.Count);
            return _enemiesConfig.GetEnemy(rand);
        }


        public override void StartLevel(Configs.LevelConfigs.LevelData levelData, int location, int level)
        {
            throw new System.NotImplementedException();
        }
    }
}