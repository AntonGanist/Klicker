using UnityEngine;
using System.Collections;

public class EnemyEffect : MonoBehaviour, IEventReceiver<PlayerAttackEvent>,
    IEventReceiver<ChangeTime>, IEventReceiver<Aksi>
{
    [SerializeField] private GameObject _fire;
    [SerializeField] private GameObject _aksi;
    [SerializeField] private GameObject _irden;
    [SerializeField] private SpriteRenderer[] _spriteRenderers;
    [SerializeField] private Color _colorCold;
    [SerializeField] private AudioSource _audioSource;
    private Coroutine _coroutineCold;
    private Coroutine _coroutineFire;
    private float _coldTime;
    private float _fireTime;
    private float _aksiTime;

    public void Initialize(float coldTime, float fireTime, float aksiTime)
    {
        EventBus.Register(this as IEventReceiver<PlayerAttackEvent>);
        EventBus.Register(this as IEventReceiver<ChangeTime>);
        EventBus.Register(this as IEventReceiver<Aksi>);
        _coldTime = coldTime;
        _fireTime = fireTime;
        _aksiTime = aksiTime;
    }
    public void OnEvent(PlayerAttackEvent @event)
    {
        if (@event.Type == AttackType.Cold)
        {
            if (_coroutineCold != null) StopCoroutine(_coroutineCold);
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].color = _colorCold;
            }
            _audioSource.Play();
            _coroutineCold = StartCoroutine(CorutineCold(_coldTime));
        }
        else if (@event.Type == AttackType.Fire)
        {
            if (_coroutineFire != null) StopCoroutine(_coroutineFire);
            _coroutineFire = StartCoroutine(CorutineFire(_fireTime));
        }
        else if (@event.Type == AttackType.Magick)
        {
            if(_irden.activeSelf == true) _irden.SetActive(false);
            _irden.SetActive(true);
        }
    }
    public void OnEvent(Aksi @event)
    {
        _aksi.SetActive(true);
        StartCoroutine(AksiCoroutine());
    }
    private IEnumerator AksiCoroutine()
    {
        yield return new WaitForSeconds(_aksiTime);
        _aksi.SetActive(false);
    }
    private IEnumerator CorutineCold(float castTime)
    {
        yield return new WaitForSeconds(castTime);
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].color = Color.white;
        }

        _coroutineCold = null;
    }
    private IEnumerator CorutineFire(float castTime)
    {
        _fire.SetActive(true);
        yield return new WaitForSeconds(castTime);
        _fire.SetActive(false);
        _coroutineFire = null;
    }
    public void OnEvent(ChangeTime @event)
    {
        if(@event.Change == false)
            _fire.SetActive(false);
        else
            if (_coroutineFire != null) _fire.SetActive(true);
    }
    public void OffEffect()
    {
        _fire.SetActive(false);
        _aksi.SetActive(false);
    }

    public UniqueId Id { get; } = new UniqueId();
    public void Unsubscribe()
    {
        StopAllCoroutines();
        EventBus.Unregister(this as IEventReceiver<PlayerAttackEvent>);
        EventBus.Unregister(this as IEventReceiver<ChangeTime>);
        EventBus.Unregister(this as IEventReceiver<Aksi>);
    }
}
