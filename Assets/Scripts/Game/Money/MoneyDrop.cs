using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CoinsConfig;

[RequireComponent(typeof(Rigidbody2D))] 
public class MoneyDrop : MonoBehaviour, IEventReceiver<ChangeTime>, IEventReceiver<Unsubscribe>
{
    [SerializeField] private float _force; 
    [SerializeField] private Rigidbody2D _rb; 
    [SerializeField] private Button _button; 
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _image;

    private MoneyCounter _moneyCounter;
    private int _price;
    private Transform _position;

    private bool _stopTime;
    private float _firstTime;
    private float _secondTime;
    private void Awake()
    {
        EventBus.Register(this as IEventReceiver<ChangeTime>);
        EventBus.Register(this as IEventReceiver<Unsubscribe>);
    }
    public void Initialize(int denom, MoneyCounter moneyCounter, Transform pos, MoneysViewData moneysViewData)
    {
        _stopTime = false;
        _firstTime = 0;
        _secondTime = 0;
        StartMove(moneysViewData);
        _price = denom;
        _moneyCounter = moneyCounter;
        _position = pos;
        _button.onClick.AddListener(CollectMoney);
        StartCoroutine(Disappearing());
    }
    private void StartMove(MoneysViewData moneysViewData)
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _image.sprite = moneysViewData.Sprite;
        transform.localScale = moneysViewData.Scale;
        Vector3 pos = Vector3.zero;
        _rectTransform.anchoredPosition3D = pos;
        _rb.mass = moneysViewData.Mass;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        _rb.AddForce(transform.right * _force, ForceMode2D.Impulse);
    }
    private void CollectMoney()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(MoveToWallet());
    }
    private IEnumerator MoveToWallet()
    {
        while (Vector3.Distance(transform.position, _position.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _position.position,
                _speed * Time.deltaTime
            );
            yield return null;
        }

        _moneyCounter.ChangeTestMoney(_price);
        gameObject.SetActive(false);
    }
    private IEnumerator Disappearing()
    {
        while (_firstTime < 5)
        {
            if (_stopTime == false)
                _firstTime += Time.deltaTime;
            yield return null;
        }
        _animator.SetTrigger("изчезает");
        yield return new WaitForSeconds(0.5f);
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        while (_secondTime < animationLength)
        {
            if (_stopTime == false)
                _secondTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    private void OnDisable() => _button.onClick.RemoveListener(CollectMoney);

    public void OnEvent(ChangeTime @event)
    {
        if (@event.Change)
        {
            _stopTime = false;
            _animator.SetFloat("speed", 1);
        }
        else
        {
            _stopTime = true;
            _animator.SetFloat("speed", 0);
        }
    }

    public UniqueId Id { get; } = new UniqueId();
    public void OnEvent(Unsubscribe @event)
    {
        EventBus.Unregister(this as IEventReceiver<ChangeTime>);
        EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
    }
}