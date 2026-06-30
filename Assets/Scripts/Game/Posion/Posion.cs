using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Posion : MonoBehaviour, IEventReceiver<ChangeTime>, IEventReceiver<Unsubscribe>
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    [SerializeField] private ParticleSystemWrapper _particleSystem;

    [SerializeField] private Vector2 posX;
    [SerializeField] private Vector2 posY;

    [SerializeField] private TextMeshProUGUI _textQuantityHealthPosion;
    [SerializeField] private TextMeshProUGUI _textQuantityManaPosion;
    [SerializeField] private AudioSource _audioSource;

    private PosionConfig _posionConfig;
    private Sprite _healthSprite;
    private Sprite _manaSprite;
    private int _quantityHealthPosion;
    private int _quantityManaPosion;
    private bool _stop = false;

    private bool _isHealthPosion = false;
    private float _healthK;
    private float _manaK;

    public void Initialize(PosionConfig posionConfig)
    {
        EventBus.Register(this as IEventReceiver<ChangeTime>);
        EventBus.Register(this as IEventReceiver<Unsubscribe>);
        _posionConfig = posionConfig;
        _healthSprite = _posionConfig.HealthPosion;
        _manaSprite = _posionConfig.ManaPosion;
        _healthK = _posionConfig.HealthPosionPower;
        _manaK = _posionConfig.ManaPosionPower;
    }
    public void SubscribeOnClick(UnityAction action)
    {
        _button.onClick.AddListener(action);
        _button.onClick.AddListener(ClickPosion);
    }
    public void TakeHealthPosion(int quantityHealthPosion)
    {
        _quantityHealthPosion = quantityHealthPosion;
        _textQuantityHealthPosion.text = _quantityHealthPosion.ToString();
    }
    public void TakeManaPosion(int quantityManaPosion)
    {
        _quantityManaPosion = quantityManaPosion;
        _textQuantityManaPosion.text = _quantityManaPosion.ToString();
    }

    private void ClickPosion()
    {
        if (_stop) return;
        _audioSource.Play();
        if (_isHealthPosion && _quantityHealthPosion > 0)
        {
            EventBus.Raise(new HealthRecovery(_healthK));
            _quantityHealthPosion--;
            _textQuantityHealthPosion.text = _quantityHealthPosion.ToString();
            if (_quantityHealthPosion == 0) OffButton();
        }
        else if (!_isHealthPosion && _quantityManaPosion > 0)
        {
            EventBus.Raise(new ManaRecovery(_manaK));
            _quantityManaPosion--;
            _textQuantityManaPosion.text = _quantityManaPosion.ToString();
            if (_quantityManaPosion == 0) OffButton();
        }
    }
    public void OnButton(bool begin = false)
    {
        if (_stop) return;

        if ((_quantityHealthPosion + _quantityManaPosion) == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if ((_isHealthPosion && _quantityHealthPosion == 0) ||
           (!_isHealthPosion && _quantityManaPosion == 0))
        {
            _isHealthPosion = !_isHealthPosion;
        }

        gameObject.SetActive(true);

        Vector2 pos = new Vector2(Random.Range(posX.x, posX.y),Random.Range(posY.x, posY.y));
        transform.localPosition = pos;

        _image.sprite = _isHealthPosion ? _healthSprite : _manaSprite;
        _particleSystem.PlayAtPosition(pos, begin);
    }
    public void OffButton()
    {
        _isHealthPosion = Random.Range(0, 2) == 1;
        gameObject.SetActive(false);
    }
    public void ChangePowerPosion(float power)
    {
        _healthK = power;
        _manaK = power;
    }
    public void OnEvent(ChangeTime @event)
    {
        if (@event.Change)
            _stop = false;
        else
        {
            _stop = true;
            OffButton();
        }
    }

    public UniqueId Id { get; } = new UniqueId();
    public void OnEvent(Unsubscribe @event)
    {
        EventBus.Unregister(this as IEventReceiver<ChangeTime>);
        EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
    }
}
