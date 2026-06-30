//using Global.Audio;
using Global.SaveSystem;
using Global.SaveSystem.SavableObjects;
using Meta.Locations;
using Meta.Shop;
using SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Meta
{
    public class MetaEntryPoint : EntryPoint
    {
        [SerializeField] private LocationManager _locationManager;
        [SerializeField] private InfinityLevelMeta _infinityLevelMeta;
        [SerializeField] private ShopWindow _shopWindow;

        [SerializeField] private Button _shopButton;

        private SaveSystem _saveSystem;
        //private AudioManager _audioManager;
        private SceneLoader _sceneLoader;

        private const string COMMON_OBJECT_TAG = "CommonObject";

        public override void Run(SceneEnterParams enterParams)
        {
            var commonObject = GameObject.FindWithTag(COMMON_OBJECT_TAG).GetComponent<CommonObject>();
            _saveSystem = commonObject.SaveSystem;
            //_audioManager = commonObject.AudioManager;
            _sceneLoader = commonObject.SceneLoader;

            var progress = (Progress)_saveSystem.GetData(SavableObjectType.Progress);
            var infinityLevelStat = (InfinityLevelStat)_saveSystem.GetData(SavableObjectType.InfinityLevelStat);

            _locationManager.Initialize(progress, StartLevel);
            _infinityLevelMeta.Initialize(infinityLevelStat, StartInfinityLevel);
            _shopWindow.Initialize(commonObject.MoneyCounter, commonObject.SkillCounter);
            //_audioManager.PlayClip(AudioNames.BackgroundMetaMusic);



            _shopButton.onClick.AddListener(OpenShop);
            _shopWindow.OffButton.onClick.AddListener(OpenLocation);

        }

        private void StartLevel(int location, int level)
        {
            _sceneLoader.LoadGameplayScene(new GameEnterParams(location, level));
        }
        private void OpenShop()
        {
            YG2.InterstitialAdvShow();
            _shopWindow.SetActive(true);
        }

        private void OpenLocation()
        {
            _shopWindow.SetActive(false);
        }

        private void StartInfinityLevel()
        {
            _sceneLoader.LoadInfinityScene();
        }
    }
}