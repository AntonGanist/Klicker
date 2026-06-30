using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.ClickButton
{
    public class CriticalDamage : MonoBehaviour, IEventReceiver<ChangeTime>, 
        IEventReceiver<Unsubscribe>
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        [SerializeField] private ParticleSystemWrapper _particleSystem;

        [SerializeField] private Vector2 posX;
        [SerializeField] private Vector2 posY;

        [SerializeField] private Image _comboImage;
        [SerializeField] private Transform _comboPos;
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private Animator _animator;

        private Vector2 _startPos;
        private float _range;
        private int _combo;

        private float _attack;
        private bool _stop = false;
        private OilDamageType _type = OilDamageType.None;
        private bool _accumulating;
        private float _accumulatingAttack = 0;
        private float _startAccumulatingAttack = 0;
        public void Initialize(Sprite sprite, float attack, Sprite sprite2, float range)
        {
            EventBus.Register(this as IEventReceiver<ChangeTime>);
            EventBus.Register(this as IEventReceiver<Unsubscribe>);
            _image.sprite = sprite;
            _comboImage.sprite = sprite2;
            _range = range;
            _startPos = _comboPos.localPosition;
            _startPos.x = 360;
            OnButton(true);
            OffButton();
            _attack = attack;
        }
        public void SubscribeOnClick(UnityAction action)
        {
            _button.onClick.AddListener(action);
            _button.onClick.AddListener(Attack);
        }
        public void TakeOil(OilDamageType oilDamageType) => _type = oilDamageType;

        private void Attack()
        {
            if (_stop) return;
            Combo();
            EventBus.Raise(new PlayerAttackEvent(_attack + _accumulatingAttack, AttackType.Critical, _type));
            EventBus.Raise(new OnCriticalButtonClickedEvent(_combo));
            if (_accumulating)
                _accumulatingAttack++;
        }

        public void AccumulatingCritDamage(float power)
        {
            _accumulating = true;
            _startAccumulatingAttack = power;
            _accumulatingAttack = power;
        }
        public void ChangeAttack(float current) => _attack *= current;

        private void Combo()
        {
            _combo++;
            _comboText.text = _combo.ToString();
            _comboPos.gameObject.SetActive(true);
            _animator.Play("0");
            float x = Random.Range(-_range, _range) + _startPos.x;
            float y = Random.Range(-_range, _range) + _startPos.y;
            _comboPos.localPosition = new Vector2(x, y);
        }
        public void ChangeYPositionBdish(float change)
        {
            _startPos.y = change;
            float x = Random.Range(-_range, _range) + _startPos.x;
            _comboPos.localPosition = new Vector2(x, _startPos.y);
        }

        public void OnButton(bool begien = false)
        {
            if (_stop) return;
            gameObject.SetActive(true);
            float randX = Random.Range(posX.x, posX.y);
            float randY = Random.Range(posY.x, posY.y);

            Vector2 pos = new Vector2(randX, randY);
            transform.localPosition = pos;
            _particleSystem.PlayAtPosition(pos, begien);
        }
        public void OffButton()
        {
            _combo = 0;
            gameObject.SetActive(false);
            _comboPos.gameObject.SetActive(false);
            _accumulatingAttack = _startAccumulatingAttack;
        }
        public void OnEvent(ChangeTime @event)
        {
            if(@event.Change)
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
}