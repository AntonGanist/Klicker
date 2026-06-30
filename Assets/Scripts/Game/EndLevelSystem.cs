using Game.Configs.LevelConfigs;
using Global.SaveSystem;
using Global.SaveSystem.SavableObjects;
using SceneManagement;
using UnityEngine;

namespace Game
{
    public class EndLevelSystem : IEventReceiver<LevelPassed>, IEventReceiver<Unsubscribe>,
        IEventReceiver<ChangeTime>
    {
        private readonly EndLevelWindow.EndLevelWindow _endLevelWindow;
        private readonly SaveSystem _saveSystem;
        private readonly GameEnterParams _gameEnterParams;
        private readonly LevelsConfig _levelsConfig;
        private readonly MoneyCounter _moneyCounter;
        private InfinityGameManager _infinityGameManager;

        private bool _stopTime = true;
        private bool _compite;
        private bool _isPassed;

        public EndLevelSystem(EndLevelWindow.EndLevelWindow endLevelWindow, SaveSystem saveSystem,
    GameEnterParams gameEnterParams, MoneyCounter moneyCounter, LevelsConfig levelsConfig,
    InfinityGameManager infinityGameManager = null)
        {
            _levelsConfig = levelsConfig;
            _gameEnterParams = gameEnterParams;
            _saveSystem = saveSystem;
            _endLevelWindow = endLevelWindow;
            _moneyCounter = moneyCounter;
            _infinityGameManager = infinityGameManager;
            EventBus.Register(this as IEventReceiver<LevelPassed>);
            EventBus.Register(this as IEventReceiver<Unsubscribe>);
            EventBus.Register(this as IEventReceiver<ChangeTime>);
        }

        public void OnEvent(LevelPassed @event)
        {
            _isPassed = @event.isPassed;
            _compite = true;
            if (!_stopTime) return;
            LevelPassed();
        }
        private void LevelPassed()
        {
            _saveSystem.SaveData(SavableObjectType.OpenedSkills);
            if (_isPassed)
            {
                TrySaveProgress();
                _endLevelWindow.ShowWinWindow(_moneyCounter, _gameEnterParams.Location, _gameEnterParams.Level);
            }
            else
            {
                if (_infinityGameManager != null)
                    TrySaveInfinityStat();
                _endLevelWindow.ShowLoseWindow();
            }
        }
        private void TrySaveProgress()
        {
            var progress = (Progress)_saveSystem.GetData(SavableObjectType.Progress);
            if (_gameEnterParams.Location != progress.CurrentLocation ||
                _gameEnterParams.Level != progress.CurrentLevel) return;

            var maxLocationAndLevel = _levelsConfig.GetMaxLocationAndLevel();
            if (progress.CurrentLocation > maxLocationAndLevel.x ||
               (progress.CurrentLocation == maxLocationAndLevel.x
                    && progress.CurrentLevel > maxLocationAndLevel.y)) return;

            var maxLevel = _levelsConfig.GetMaxLevelOnLocation(progress.CurrentLocation);
            if (progress.CurrentLevel >= maxLevel)
            {
                progress.CurrentLevel = 1;
                progress.CurrentLocation++;
            }
            else
            {
                progress.CurrentLevel++;
            }

            _saveSystem.SaveData(SavableObjectType.Progress);
        }
        private void TrySaveInfinityStat()
        {
            var progress = (InfinityLevelStat)_saveSystem.GetData(SavableObjectType.InfinityLevelStat);
            int kills = progress.BestNumberMonstersKilled;
            if (kills < _infinityGameManager.GetKilled())
            {
                progress.BestNumberMonstersKilled = _infinityGameManager.GetKilled();
                kills = _infinityGameManager.GetKilled();
                _endLevelWindow.Record(0);
            }
            int money = progress.MaxGetMoney;
            if (money < _moneyCounter.GetTestMoney())
            {
                progress.MaxGetMoney = _moneyCounter.GetTestMoney();
                money = _moneyCounter.GetTestMoney();
                _endLevelWindow.Record(1);
            }
            int crit = progress.BestCriticalCombo;
            if (crit < _infinityGameManager.GetCombo())
            {
                progress.BestCriticalCombo = _infinityGameManager.GetCombo();
                crit = _infinityGameManager.GetCombo();
                _endLevelWindow.Record(2);
            }
            
            _endLevelWindow.RecordText(0, kills);
            _endLevelWindow.RecordText(1, money);
            _endLevelWindow.RecordText(2, crit);
            _saveSystem.SaveData(SavableObjectType.InfinityLevelStat);
        }

        public void OnEvent(ChangeTime @event)
        {
            _stopTime = @event.Change;
            if (!_stopTime || !_compite) return;
            LevelPassed();
        }


        public UniqueId Id { get; } = new UniqueId();
    public void OnEvent(Unsubscribe @event)
    {
        EventBus.Unregister(this as IEventReceiver<LevelPassed>);
        EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
        EventBus.Unregister(this as IEventReceiver<ChangeTime>);
    }
}
}