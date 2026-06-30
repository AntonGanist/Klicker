using UnityEngine;

namespace Game
{
    public class InfinityGameManager : GameManager
    {
        [SerializeField] private Sprite[] _backgraundsImage;
        private int _kills = 0;
        private int _combo = 0;
        protected override void StartLevel()
        {
            _playerHealth.Initialize(true);
            int rand = Random.Range(0, _backgraundsImage.Length);
            _backgraund.sprite = _backgraundsImage[rand];
            _enemyManager.StartLevel();
        }
        protected override void NextLevel()
        {
            throw new System.NotImplementedException();
        }

        protected override void LevelSetting()
        {
            _endLevelWindow.Initialize();
            _endLevelSystem = new(_endLevelWindow, _saveSystem, _gameEnterParams, 
                _moneyCounter, null, GetComponent<InfinityGameManager>());
        }

        protected override void EnemyDead(int count)
        {
            _kills = count;
        }
        protected override void Crit(int count)
        {
            _combo = count;
        }
        public int GetKilled() => _kills;
        public int GetCombo() => _combo;
    }
}