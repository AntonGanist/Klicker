using Game.ClickButton;
using Game.Enemies;
using Game.Skills;
//using Global.Audio;
using Global.SaveSystem;
using Global.SaveSystem.SavableObjects;
using SceneManagement;
using UnityEngine;
using Progress = Global.SaveSystem.SavableObjects.Progress;
using UnityEngine.UI;
using Game.Magick;

namespace Game
{
    public abstract class GameManager : EntryPoint, IEventReceiver<Restart>, IEventReceiver<Unsubscribe>, 
        IEventReceiver<OnButtonClickedEvent>, IEventReceiver<GoInMeta>, IEventReceiver<OnCriticalButtonClickedEvent>,
        IEventReceiver<ChangeTime>, IEventReceiver<EnemyDead>, IEventReceiver<EnemyAttackEvent>,
        IEventReceiver<NextLevel>, IEventReceiver<HealthRecovery>, IEventReceiver<ManaRecovery>, IEventReceiver<SpawnEnemy>
    {
        [SerializeField] protected PlayerHealthAndMana _playerHealth;
        [SerializeField] protected ClickButtonManager _clickButtonManager;
        [SerializeField] protected EnemyManager _enemyManager;
        [SerializeField] protected EnemyHealthBar _healthBar;
        [SerializeField] protected EndLevelWindow.EndLevelWindow _endLevelWindow;
        [SerializeField] protected Image _backgraund;
        [SerializeField] protected Button _backButton;
        [SerializeField] protected CreateMagick _createMagick;

        //private AudioManager _audioManager;

        protected SkillSystem _skillSystem;
        protected EndLevelSystem _endLevelSystem;
        protected SceneLoader _sceneLoader;

        protected const string COMMON_OBJECT_TAG = "CommonObject";

        protected GameEnterParams _gameEnterParams;
        protected SaveSystem _saveSystem;

        protected MoneyCounter _moneyCounter;

        public override void Run(SceneEnterParams enterParams)
        {
            Subscribe();
            var commonObject = GameObject.FindWithTag(COMMON_OBJECT_TAG).GetComponent<CommonObject>();
            _saveSystem = commonObject.SaveSystem;
            _moneyCounter = commonObject.MoneyCounter;
            //_audioManager = commonObject.AudioManager;
            _sceneLoader = commonObject.SceneLoader;
            var balansManager = commonObject.BalanceManager;

            if (enterParams is GameEnterParams gameEnterParams)
            {
                _gameEnterParams = gameEnterParams;
            }


            var skillCounter = commonObject.SkillCounter;
            var formuls = commonObject.Formuls;

            var openedSkills = (OpenedSkills)_saveSystem.GetData(SavableObjectType.OpenedSkills);

            _skillSystem = new(openedSkills, _playerHealth, _createMagick, _enemyManager, 
                _clickButtonManager, skillCounter);
            _skillSystem.InvokeTrigger(SkillTrigger.OnStart);

            _clickButtonManager.Initialize();
            _enemyManager.Initialize(_healthBar, _moneyCounter, _endLevelWindow, skillCounter, 
                balansManager, formuls, _clickButtonManager);
            _createMagick.Initialize(10f);

            _backButton.onClick.AddListener(LoadMetaScene);
            LevelSetting();
            StartLevel();
        }
        protected abstract void LevelSetting();
        protected abstract void StartLevel();
        protected abstract void NextLevel();
        protected abstract void EnemyDead(int count);
        protected abstract void Crit(int count);

        private void LoadMetaScene() => _sceneLoader.LoadMetaScene();
        public void OnEvent(Restart @event)
        {
            if (_gameEnterParams != null)
            {
                _sceneLoader.LoadGameplayScene(_gameEnterParams);
            }
            else
            {
                _sceneLoader.LoadInfinityScene();
            }
        }
        public void OnEvent(NextLevel @event) => NextLevel();
        public void OnEvent(GoInMeta @event) => _sceneLoader.LoadMetaScene();
        public void OnEvent(OnButtonClickedEvent @event) => _skillSystem.InvokeTrigger(SkillTrigger.OnDamage);
        public void OnEvent(OnCriticalButtonClickedEvent @event)
        {
            _skillSystem.InvokeTrigger(SkillTrigger.OnCriticalDamage);
            Crit(@event.Number);
        }
        public void OnEvent(ChangeTime @event)
        {
            if(!@event.Change && @event.IsMagick)
                _skillSystem.InvokeTrigger(SkillTrigger.OnCastSpell);
        }
        public void OnEvent(EnemyDead @event)
        {
            _skillSystem.InvokeTrigger(SkillTrigger.OnDead);
            EnemyDead(@event.Number);
        }
        public void OnEvent(EnemyAttackEvent @event) => _skillSystem.InvokeTrigger(SkillTrigger.OnTakeDamage);
        public void OnEvent(HealthRecovery @event) => _skillSystem.InvokeTrigger(SkillTrigger.OnPosion);
        public void OnEvent(ManaRecovery @event) => _skillSystem.InvokeTrigger(SkillTrigger.OnPosion);
        public void OnEvent(SpawnEnemy @event) => _skillSystem.InvokeTrigger(SkillTrigger.OnSpawnEnemy);

        
        private void Subscribe()
        {
            EventBus.Register(this as IEventReceiver<Restart>);
            EventBus.Register(this as IEventReceiver<GoInMeta>);
            EventBus.Register(this as IEventReceiver<Unsubscribe>);
            EventBus.Register(this as IEventReceiver<OnButtonClickedEvent>);
            EventBus.Register(this as IEventReceiver<OnCriticalButtonClickedEvent>);
            EventBus.Register(this as IEventReceiver<ChangeTime>);
            EventBus.Register(this as IEventReceiver<EnemyDead>);
            EventBus.Register(this as IEventReceiver<NextLevel>);
            EventBus.Register(this as IEventReceiver<EnemyAttackEvent>);
            EventBus.Register(this as IEventReceiver<HealthRecovery>);
            EventBus.Register(this as IEventReceiver<ManaRecovery>);
            EventBus.Register(this as IEventReceiver<SpawnEnemy>);
        }
        public UniqueId Id { get; } = new UniqueId();
        public void OnEvent(Unsubscribe @event)
        {
            EventBus.Unregister(this as IEventReceiver<Restart>);
            EventBus.Unregister(this as IEventReceiver<GoInMeta>);
            EventBus.Unregister(this as IEventReceiver<OnButtonClickedEvent>);
            EventBus.Unregister(this as IEventReceiver<OnCriticalButtonClickedEvent>);
            EventBus.Unregister(this as IEventReceiver<ChangeTime>);
            EventBus.Unregister(this as IEventReceiver<EnemyDead>);
            EventBus.Unregister(this as IEventReceiver<NextLevel>);
            EventBus.Unregister(this as IEventReceiver<EnemyAttackEvent>);
            EventBus.Unregister(this as IEventReceiver<HealthRecovery>);
            EventBus.Unregister(this as IEventReceiver<ManaRecovery>);
            EventBus.Unregister(this as IEventReceiver<SpawnEnemy>);
            EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
        }
    }
}