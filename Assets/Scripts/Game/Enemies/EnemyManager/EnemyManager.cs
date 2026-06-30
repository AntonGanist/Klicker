using Game.ClickButton;
using Game.Configs.EnemyConfigs;
using Game.Configs.LevelConfigs;
using UnityEngine;
using static Game.Configs.EnemyConfigs.EnemiesConfig;

namespace Game.Enemies
{
    public struct LastAttackData
    {
        public float Damage;
        public AttackType Type;
        public OilDamageType Oil;
    }

    public abstract class EnemyManager : MonoBehaviour, IEventReceiver<LevelPassed>, IEventReceiver<Unsubscribe>
    {
        [SerializeField] protected Transform _enemyContainer;
        [SerializeField] protected EnemiesConfig _enemiesConfig;
        [SerializeField] protected LootManager _lootManager;
        protected int _location;

        protected Enemy _currentEnemyMonoBehaviour;
        protected EnemyHealthBar _healthBar;
        protected int _currentEnemyIndex;

        protected EnemyViewData _enemyViewData;

        protected EndLevelWindow.EndLevelWindow _endLevelWindow;

        protected LastAttackData _lastAttackData;

        protected ClickButtonManager _clickButtonManager;

        protected float _health;
        protected float _damage;
        protected float _damageSpeed;
        protected float _reward;

        protected SkillCounter _skillCounter;
        protected BalanceManager _balanceManager;
        protected Formuls _formuls;

        protected float _dopOilDamage;

        protected OilDamageType _oilDamageType;
        public void Initialize(EnemyHealthBar healthBar, MoneyCounter moneyCounter, EndLevelWindow.EndLevelWindow endLevelWindow,
            SkillCounter skillCounter, BalanceManager balanceManager, Formuls formuls,
            ClickButtonManager clickButtonManager)
        {
            EventBus.Register(this as IEventReceiver<LevelPassed>);
            EventBus.Register(this as IEventReceiver<Unsubscribe>);
            _healthBar = healthBar;
            _endLevelWindow = endLevelWindow;
            _balanceManager = balanceManager;
            _lootManager.Initialize(moneyCounter, skillCounter);
            _skillCounter = skillCounter;
            _formuls = formuls;
            _clickButtonManager = clickButtonManager;
        }

        public abstract void StartLevel(LevelData levelData, int location, int level);
        public abstract void StartLevel();

        protected void InitHpBar(float health, string name, Vector2 pos)
        {
            _healthBar.Show();
            _healthBar.SetMaxValue(health);
            _healthBar.SetName(name);
            _healthBar.CurrentPosition(pos);
            _clickButtonManager.ChangeYPositionBdish(pos.y);
        }
        public void ChangeHealthBar(LastAttackData lastAttackData)
        {
            _healthBar.DecreaseValue(lastAttackData.Damage, _enemyViewData.ViewDamagePosition,
                lastAttackData.Type);
            _lastAttackData = lastAttackData;
        }

        public void RepeatAttack()
        {
            EventBus.Raise(new PlayerAttackEvent(_lastAttackData.Damage, _lastAttackData.Type, _lastAttackData.Oil));
        }
        public void IncreaseMoney(float amount) => _lootManager.IncreaseMoney(amount);
        public void OnEvent(LevelPassed @event)
        {
            if (!@event.isPassed)
            {
                _endLevelWindow.TakeEnemyType(_enemyViewData);
            }
            StopAllCoroutines();
        }

        public void Dialog(bool active)=> _currentEnemyMonoBehaviour.Dialog(active);

        public OilDamageType GetOil() => _oilDamageType;
        public void ChangeOilDamage(float change) => _dopOilDamage = change;
        public void ChangeEnemyCorutine(float change) => _currentEnemyMonoBehaviour.ChangeEnemyCorutine(change);

        public UniqueId Id { get; } = new UniqueId();
        public void OnEvent(Unsubscribe @event)
        {
            EventBus.Register(this as IEventReceiver<LevelPassed>);
            EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
        }
    }
}
