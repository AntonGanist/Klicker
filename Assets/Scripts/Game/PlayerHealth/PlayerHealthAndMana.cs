using System.Collections;
using Game.HealthBar;
using UnityEngine;

public class PlayerHealthAndMana : MonoBehaviour, IEventReceiver<EnemyAttackEvent>, IEventReceiver<HealthRecovery>, 
    IEventReceiver<ManaRecovery>, IEventReceiver<Unsubscribe>
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private HealthBar _manaBar;
    [SerializeField] private HealthBar _kwenBar;
    [SerializeField] private AudioSource _audioSource;
    private float _maxHealth;
    private float _health;

    private float _maxMana;
    private float _mana;

    private Coroutine _coroutine;

    private bool _lastChance;
    private float _lastChanceHeiling;

    private float _armor = 1;
    private bool _endless;
    public void Initialize(bool endless = false)
    {
        EventBus.Register(this as IEventReceiver<EnemyAttackEvent>);
        EventBus.Register(this as IEventReceiver<HealthRecovery>);
        EventBus.Register(this as IEventReceiver<ManaRecovery>);
        EventBus.Register(this as IEventReceiver<Unsubscribe>);
        _kwenBar.Hide();
        _endless = endless;
    }
    public void TakeHealth(float health)
    {
        _health = health;
        _maxHealth = health;
        InitHealthBar(_maxHealth);
    }
    public void TakeMana(float mana)
    {
        _mana = mana;
        _maxMana = mana;
        InitManaBar(_maxMana);
    }
    private void InitHealthBar(float health)
    {
        _healthBar.Show();
        _healthBar.SetMaxValue(health);
    }
    private void InitManaBar(float mana)
    {
        _manaBar.Show();
        _manaBar.SetMaxValue(mana);
    }

    public void OnEvent(EnemyAttackEvent @event)
    {
        if (_coroutine != null) return;
        bool chance = false;
        if (@event.Damage * _armor >= _health)
        {
            if(_lastChance == false)
            {
                _health = 0;
                EventBus.Raise(new LevelPassed(false, _endless));
                return;
            }
            else
            {
                _lastChance = false;
                chance = true;
                _health = _lastChanceHeiling;
            }
        }
        if (!chance)
        {
            _health -= @event.Damage * _armor;
            _healthBar.DecreaseValue(@event.Damage * _armor);
        }
        else
        {
            _healthBar.ChangeValue(_health);
        }
        
    }

    public void OnEvent(HealthRecovery @event)
    {
        float current = @event.Recovery * _maxHealth;
        if (_health == _maxHealth) return;
        if (_health + current > _maxHealth)
        {
            current = _maxHealth - _health;
            _health = _maxHealth;
        }
        else
        {
            _health += current;
        }
        _healthBar.IncreaseValue(current);
    }
    public void OnEvent(ManaRecovery @event)
    {
        float current = @event.Recovery * _maxMana;
        if (_mana == _maxMana) return;
        if (_mana + current > _maxMana)
        {
            current = _maxMana - _mana;
            _mana = _maxMana;
        }
        else
        {
            _mana += current;
        }
        _manaBar.IncreaseValue(current);
    }

    public void CurrentMana(float currentMana)
    {
        _mana -= currentMana;
        _manaBar.DecreaseValue(currentMana);
    }

    public float GetMana() => _mana;
    public float GetHealth() => _health;
    public float GetMaxHealth() => _maxHealth;

    public void Kwen(float time)
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(KwenCorutine(time));
            _audioSource.Play();
        }
    }
    private IEnumerator KwenCorutine(float totalTime)
    {
        _kwenBar.Show();
        _kwenBar.SetMaxValue(totalTime);

        float remainingTime = totalTime;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            _kwenBar.DecreaseValue(Time.deltaTime);
            yield return null;
        }

        _kwenBar.Hide();
        _coroutine = null;
    }

    public void LastChanceMutagen(float heiling)
    {
        _lastChance = true;
        _lastChanceHeiling = heiling;
    }
    public void ChangeArmor(float amount)
    {
        _armor = 1;
        if (amount != 0) _armor -= amount;
        else _armor = 1;
        if (_armor <= 0) _armor = 0.05f;
    }

    public void TakeHealth(float k, bool K)
    {
        _health = _health * k;
        _maxHealth = _health;
        InitHealthBar(_maxHealth);
    }
    public void TakeMana(float k, bool K)
    {
        _mana = _mana * k;
        _maxMana = _mana;
        InitManaBar(_maxMana);
    }

    public UniqueId Id { get; } = new UniqueId();
    public void OnEvent(Unsubscribe @event)
    {
        EventBus.Unregister(this as IEventReceiver<EnemyAttackEvent>);
        EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
    }
}