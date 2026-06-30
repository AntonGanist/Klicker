using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using static Game.Configs.EnemyConfigs.EnemiesConfig;

namespace Game.Enemies
{
    public class Enemy : MonoBehaviour, IEventReceiver<PlayerAttackEvent>, IEventReceiver<Aksi>,
        IEventReceiver<LevelPassed>, IEventReceiver<ChangeTime>
    {
        private Transform _enemy;
        private Animator _animator;
        private EnemyEffect _enemyEffect;

        public event UnityAction OnDead;
        public event UnityAction OnHideHpBar;
        public event UnityAction<LastAttackData> OnChangeHealthBar;

        private float _health;
        private bool _isDying = false;

        private PoolMono<ParticleSystemWrapper> _pools;

        private float _damage;
        private float _damageTime;
        private float _frameAttack;

        private float _baseDamageTime;
        private Coroutine _attackCoroutine;

        private AttackType _vulnerabilityType;
        private OilDamageType _oilDamageType;

        private float _aksiTime;
        private float _kooficent = 1;

        private float _coldTime;
        private Coroutine _coroutineCold;

        private float _fireTime;
        private Coroutine _coroutineFire;
        private float _fireDamage;

        private Vector3 _originalPosition; 
        private Coroutine _shakeCoroutine;

        private bool _dialog;
        private bool _delay;

        private float _dopOilDamage;
        public void Initialize(float health, float damage, float timeDamage,
            EnemyViewData enemiesConfig, Transform enemy, float dopOilDamage)
        {
            Subscribe();
            _pools = new PoolMono<ParticleSystemWrapper>(enemiesConfig.PrefabBlood, 5, transform);
            _pools.autoExpand = true;
            _health = health;

            _vulnerabilityType = enemiesConfig.VulnerabilityType;
            _oilDamageType = enemiesConfig.VulnerabilityToOil;

            NewEnemy(enemy, enemiesConfig);
            _damage = damage;
            _damageTime = timeDamage;
            _baseDamageTime = timeDamage;
            _attackCoroutine = StartCoroutine(AtackCoroutine());
            _enemy.gameObject.SetActive(!_dialog);
            _dopOilDamage = dopOilDamage;
        }
        private void NewEnemy(Transform transform, EnemyViewData enemiesConfig)
        {
            _enemy = transform;
            _animator = _enemy.GetComponent<Animator>();
            _animator.Rebind();
            _animator.Update(0f);
            _enemy.parent = null;
            _enemy.position = enemiesConfig.Position;
            _enemy.localScale = enemiesConfig.Scale;
            _coldTime = enemiesConfig.ColdTime;
            _fireTime = enemiesConfig.FireTime;
            _enemyEffect = _enemy.GetComponent<EnemyEffect>();
            _frameAttack = enemiesConfig.FrameAttack;
            _fireDamage = _health * enemiesConfig.FireDamage;
            _aksiTime = enemiesConfig.AksiTime;
            _enemyEffect.Initialize(_coldTime, _fireTime, _aksiTime);
        }

        public void OnEvent(PlayerAttackEvent @event)
        {
            if (_isDying) return;
            if (_delay)
            {
                _delay = false;
                return;
            }
            float damage = ProcessingAttack(@event.Damage, @event.Type, @event.Oil);
            if (damage >= _health)
            {
                _health = 0;
                _pools.OfAllElement();
                Animator animatot = _animator;
                GameObject enemy = _enemy.gameObject;
                _animator = null;
                _enemy = null;
                OnHideHpBar?.Invoke();
                Unsubscribe();
                StartCoroutine(DieCoroutine(animatot, enemy));
                return;
            }
            if(@event.Type == AttackType.Standart || @event.Type == AttackType.Critical)
            {
                var effect = _pools.GetFreeElement();
                effect.PlayAtPosition();
            }
            _health -= damage;
            LastAttackData lastAttackData = new LastAttackData { Damage = damage, 
                Type = @event.Type, Oil = @event.Oil };
            OnChangeHealthBar?.Invoke(lastAttackData);
        }
        private float ProcessingAttack(float damage, AttackType attackType, OilDamageType oilDamageType)
        {
            if (attackType == _vulnerabilityType) damage *= 1.5f;
            if (oilDamageType == _oilDamageType) damage *= (1.5f + _dopOilDamage);
            if (attackType == AttackType.Cold)
            {
                if (_coroutineCold != null) StopCoroutine(_coroutineCold);
                _coroutineCold = StartCoroutine(CoroutineCold());
            }
            if (attackType == AttackType.Fire)
            {
                if (_coroutineFire != null) StopCoroutine(_coroutineFire);
                _coroutineFire = StartCoroutine(CoroutineFire());
            }
            if (attackType == AttackType.Critical)
            {
                if (_shakeCoroutine != null)
                    StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = StartCoroutine(ShakeCoroutine());
            }

            return damage * _kooficent;
        }

        private IEnumerator AtackCoroutine()
        {
            while (_health > 0)
            {
                yield return new WaitForSeconds(_damageTime);
                if (_animator != null)
                {
                    _animator.SetTrigger("attack");
                    yield return null;
                    AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
                    while (state.normalizedTime < _frameAttack)
                    {
                        yield return null;
                        state = _animator.GetCurrentAnimatorStateInfo(0);
                    }
                    EventBus.Raise(new EnemyAttackEvent(_damage));
                }
            }
        }

        public void OnEvent(Aksi @event)
        {
            if (_kooficent == 1)
            {
                _kooficent = @event.Kooficent;
                StartCoroutine(AksiCoroutine());
            }

        }
        private IEnumerator AksiCoroutine()
        {
            yield return new WaitForSeconds(_aksiTime);
            _kooficent = 1;
        }
        private IEnumerator CoroutineCold()
        {
            _animator.SetFloat("speed", 0.25f);
            _damageTime = _baseDamageTime * 2f;
            yield return new WaitForSeconds(_coldTime);
            _damageTime = _baseDamageTime;
            _animator.SetFloat("speed", 1);
            _coroutineCold = null;
        }
        private IEnumerator CoroutineFire()
        {
            float elapsed = 0f;
            while (elapsed < _fireTime)
            {
                yield return new WaitForSeconds(1);
                EventBus.Raise(new PlayerAttackEvent(_fireDamage, AttackType.None));
                elapsed += 1f;
                yield return null;
            }
            _coroutineFire = null;
        }
        private IEnumerator ShakeCoroutine()
        {
            _originalPosition = _enemy.position;
            float elapsed = 0f;

            while (elapsed < 0.15f)
            {
                Vector3 randomOffset = Random.insideUnitSphere * 0.1f;
                randomOffset.z = 0;
                _enemy.position = _originalPosition + randomOffset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _enemy.position = _originalPosition;
            _shakeCoroutine = null;
        }

        public void OnEvent(LevelPassed @event)
        {
            Unsubscribe();
            if(_enemy != null) _enemy.gameObject.SetActive(false);
        }

        public void OnEvent(ChangeTime @event)
        {
            if (_animator == null) return;
            if (@event.Change)
            {
                if (_coroutineCold != null)
                {
                    _animator.SetFloat("speed", 0.25f);
                    _damageTime = _baseDamageTime * 2f;
                }
                else
                {
                    _animator.SetFloat("speed", 1);
                    _damageTime = _baseDamageTime;
                }
                if (_attackCoroutine == null)
                    _attackCoroutine = StartCoroutine(AtackCoroutine());
            }
            else
            {
                _animator.SetFloat("speed", 0);
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }

        private IEnumerator DieCoroutine(Animator animator, GameObject enemy)
        {
            _isDying = true;

            animator.Play("смерть");
            yield return null;
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength * 0.9f);
            enemy.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            OnDead?.Invoke();
            _pools.Clear();
            _isDying = false;
            //StopAllCoroutines();
        }
        public void Dialog(bool active)
        {
            if (active)
            {
                _delay = true;
            }
            _dialog = active;
            if (_enemy != null) _enemy.gameObject.SetActive(!active);
        }

        public void ChangeEnemyCorutine(float change)
        {
            _coldTime *= change;
            _fireTime *= change;
            _aksiTime *= change;
        }

        private void Subscribe()
        {
            EventBus.Register(this as IEventReceiver<PlayerAttackEvent>);
            EventBus.Register(this as IEventReceiver<ChangeTime>);
            EventBus.Register(this as IEventReceiver<Aksi>);
            EventBus.Register(this as IEventReceiver<LevelPassed>);
        }
        public UniqueId Id { get; } = new UniqueId();
        private void Unsubscribe()
        {
            EventBus.Unregister(this as IEventReceiver<PlayerAttackEvent>);
            EventBus.Unregister(this as IEventReceiver<ChangeTime>);
            EventBus.Unregister(this as IEventReceiver<LevelPassed>);
            EventBus.Unregister(this as IEventReceiver<Aksi>);
            StopAllCoroutines();
            _enemyEffect.Unsubscribe();
        }
    }
}
