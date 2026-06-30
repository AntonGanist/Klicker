using Game.Configs.LevelConfigs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Game.Configs.EnemyConfigs.EnemiesConfig;

namespace Game.EndLevelWindow
{
    public class EndLevelWindow : MonoBehaviour
    {
        [SerializeField] private GameObject _loseLevelWindow;
        [SerializeField] private GameObject _winLevelWindow;

        [SerializeField] private Button _loseRestartButton;
        [SerializeField] private Button _winRestartButton;

        [SerializeField] private Button _loseGoMetaButton;
        [SerializeField] private Button _winGoMetaButton;

        [SerializeField] private Button _nextLevelButton;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _loseText;
        [SerializeField] private GameObject[] _enemyTextOil;
        [SerializeField] private GameObject[] _enemyTextSign;

        [SerializeField] private GameObject[] _record;
        [SerializeField] private TextMeshProUGUI[] _recordText;

        private LevelsConfig _levelsConfig;

        public event UnityAction OnRestartClicked;
        public event UnityAction OnMetaClicked;

        public void Initialize(LevelsConfig levelsConfig = null)
        {
            _levelsConfig = levelsConfig;
            _loseRestartButton.onClick.AddListener(Restart);
            _winRestartButton.onClick.AddListener(Restart);
            _loseGoMetaButton.onClick.AddListener(GoMeta);
            _winGoMetaButton.onClick.AddListener(GoMeta);
            _nextLevelButton.onClick.AddListener(NextLevel);
        }

        public void ShowLoseWindow()
        {
            _loseLevelWindow.SetActive(true);
            _winLevelWindow.SetActive(false);
            gameObject.SetActive(true);
            _nextLevelButton.gameObject.SetActive(false);
        }

        public void ShowWinWindow(MoneyCounter moneyCounter, int currentLocation, int currentLevel)
        {
            _text.text = "+ " + moneyCounter.CountMoneyInLevel();
            _loseLevelWindow.SetActive(false);
            _winLevelWindow.SetActive(true);
            gameObject.SetActive(true);

            if(_levelsConfig != null)
            {
                bool shouldShowNextLevelButton = ShouldShowNextLevelButton(currentLocation, currentLevel);
                _nextLevelButton.gameObject.SetActive(shouldShowNextLevelButton);
            }
        }

        private bool ShouldShowNextLevelButton(int currentLocation, int currentLevel)
        {
            var maxLocationAndLevel = _levelsConfig.GetMaxLocationAndLevel();
            bool isLastLevelInGame = currentLocation == maxLocationAndLevel.x &&
                                    currentLevel == maxLocationAndLevel.y;
            int maxLevelForCurrentLocation = _levelsConfig.GetMaxLevelOnLocation(currentLocation);
            bool isLastLevelInLocation = currentLevel >= maxLevelForCurrentLocation;
            return !isLastLevelInGame && !isLastLevelInLocation;
        }

        private void Restart()
        {
            EventBus.Raise(new Restart());
        }
        private void NextLevel()
        {
            EventBus.Raise(new NextLevel());
        }
        private void GoMeta()
        {
            EventBus.Raise(new GoInMeta());
        }

        public void TakeEnemyType(EnemyViewData type)
        {
            _loseText.text = type.Name;

            switch (type.VulnerabilityToOil)
            {
                case OilDamageType.CorpseEater:
                    _enemyTextOil[0].SetActive(true);
                    break;
                case OilDamageType.Ogre:
                    _enemyTextOil[1].SetActive(true);
                    break;
                case OilDamageType.Magick:
                    _enemyTextOil[2].SetActive(true);
                    break;
                default:
                    _enemyTextOil[3].SetActive(true);
                    break;
            }

            switch (type.VulnerabilityType)
            {
                case AttackType.Fire:
                    _enemyTextSign[0].SetActive(true);
                    break;
                case AttackType.Cold:
                    _enemyTextSign[1].SetActive(true);
                    break;
                case AttackType.Magick:
                    _enemyTextSign[2].SetActive(true);
                    break;
                default:
                    _enemyTextSign[3].SetActive(true);
                    _enemyTextSign[4].SetActive(true);
                    break;
            }
        }

        public void Record(int numb) => _record[numb].SetActive(true);
        public void RecordText(int numb, int text) => _recordText[numb].text = text.ToString();
    }
}