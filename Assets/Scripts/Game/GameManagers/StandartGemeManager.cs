using Game.Configs.LevelConfigs;
using Global.SaveSystem;
using Global.SaveSystem.SavableObjects;
using SceneManagement;
using UnityEngine;

namespace Game
{
    public class StandartGemeManager : GameManager
    {
        [SerializeField] protected LevelsConfig _levelsConfig;
        [SerializeField] protected DialogSystem _dialogSystem;
        [SerializeField] protected DialogConfig _dialogConfig;
        private int _currentLocation;
        private int _currentLevel;

        protected override void StartLevel()
        {
            var maxLocationAndLevel = _levelsConfig.GetMaxLocationAndLevel();
            var location = _gameEnterParams.Location;
            var level = _gameEnterParams.Level;
            _currentLocation = location;
            _currentLevel = level;
            if (location > maxLocationAndLevel.x ||
               (location == maxLocationAndLevel.x && level > maxLocationAndLevel.y))
            {
                location = maxLocationAndLevel.x;
                level = maxLocationAndLevel.y;
            }
            var levelData = _levelsConfig.GetLevel(location, level);

            _backgraund.sprite = levelData.Backgraund;
            _enemyManager.StartLevel(levelData, location, level);
            _playerHealth.Initialize();

            if (_dialogConfig.Cheak(location, level))
                _dialogSystem.Initialize(_dialogConfig, location, level, _moneyCounter);
            else _dialogSystem.OffDialog();
        }
        protected override void NextLevel()
        {
            var progress = (Progress)_saveSystem.GetData(SavableObjectType.Progress);
            var maxLocationAndLevel = _levelsConfig.GetMaxLocationAndLevel();
            if (_currentLevel < _levelsConfig.GetMaxLevelOnLocation(_currentLocation))
            {
                _currentLevel++;
            }
            else if (_currentLocation < maxLocationAndLevel.x)
            {
                _currentLocation++;
                _currentLevel = 1;
            }
            else
            {
                return;
            }
            _sceneLoader.LoadGameplayScene(new GameEnterParams(_currentLocation, _currentLevel));
        }

        protected override void LevelSetting()
        {
            _endLevelWindow.Initialize(_levelsConfig);
            _endLevelSystem = new(_endLevelWindow, _saveSystem, _gameEnterParams, _moneyCounter, _levelsConfig);
        }

        protected override void EnemyDead(int count){}

        protected override void Crit(int count){}
    }
}
