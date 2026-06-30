using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace Game.ClickButton
{
    public class ClickButton : MonoBehaviour, IEventReceiver<ChangeTime>, 
        IEventReceiver<Unsubscribe>
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        private float _attack;
        private bool _stop = false;
        private OilDamageType _type = OilDamageType.None;
        private bool _attackedThisFrame = false;
        public void Initialize(Sprite sprite, ColorBlock colorBlock, float attack)
        {
            EventBus.Register(this as IEventReceiver<ChangeTime>);
            EventBus.Register(this as IEventReceiver<Unsubscribe>);
            _image.sprite = sprite;
            _button.colors = colorBlock;
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
            if (_stop || _attackedThisFrame) return;

            _attackedThisFrame = true;
            EventBus.Raise(new PlayerAttackEvent(_attack, AttackType.Standart, _type));
            EventBus.Raise(new OnButtonClickedEvent());
        }

        private void LateUpdate() => _attackedThisFrame = false;
        public void ChangeAttack(float current) => _attack += current;
        public void ChangeAttack(float current, bool k) => _attack *= current;

        public void OnEvent(ChangeTime @event) => _stop = !@event.Change;

        public UniqueId Id { get; } = new UniqueId();
        public void OnEvent(Unsubscribe @event)
        {
            EventBus.Unregister(this as IEventReceiver<ChangeTime>);
            EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
        }
    }
}