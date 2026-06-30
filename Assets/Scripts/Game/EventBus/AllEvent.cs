public interface IAllEvent { }
public struct EnemyAttackEvent : IEvent
{
    public float Damage { get; }
    public EnemyAttackEvent(float damage)
    {
        Damage = damage;
    }
}
public struct PlayerAttackEvent : IEvent
{
    public float Damage { get; }
    public AttackType Type { get; }
    public OilDamageType Oil { get; }
    public PlayerAttackEvent(float damage, AttackType type, OilDamageType oil = OilDamageType.None)
    {
        Damage = damage;
        Type = type;
        Oil = oil;
    }
}
public struct Restart : IEvent { }
public struct Unsubscribe : IEvent { }
public struct GoInMeta : IEvent { }
public struct NextLevel : IEvent { }
public struct LevelPassed : IEvent
{
    public bool isPassed { get; }
    public bool isEndless { get; }
    public LevelPassed(bool IsPassed, bool IsEndless)
    {
        isPassed = IsPassed;
        isEndless = IsEndless;
    }
}
public struct ChangeTime : IEvent
{
    public bool Change { get; }
    public bool IsMagick { get; }
    public ChangeTime(bool Change, bool IsMagick = false)
    {
        this.Change = Change;
        this.IsMagick = IsMagick;
    }
}
public struct Aksi : IEvent
{
    public float Kooficent { get; }
    public Aksi(float Kooficent)
    {
        this.Kooficent = Kooficent;
    }
}
public struct HealthRecovery : IEvent
{
    public float Recovery { get; }
    public HealthRecovery(float Recovery)
    {
        this.Recovery = Recovery;
    }
}
public struct ManaRecovery : IEvent
{
    public float Recovery { get; }
    public ManaRecovery(float Recovery)
    {
        this.Recovery = Recovery;
    }
}
public struct OnButtonClickedEvent : IEvent { }
public struct OnCriticalButtonClickedEvent : IEvent
{
    public int Number { get; }
    public OnCriticalButtonClickedEvent(int Number)
    {
        this.Number = Number;
    }
}
public struct EnemyDead : IEvent
{
    public int Number { get; }
    public EnemyDead(int Number = 0)
    {
        this.Number = Number;
    }
}
public struct SpawnEnemy : IEvent { }


