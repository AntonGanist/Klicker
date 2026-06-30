using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.Magick
{
    public class CreateMagick : MonoBehaviour, IEventReceiver<LevelPassed>
    {
        [SerializeField] private Button _button;
        [SerializeField] private float minSwipeDistance;
        [SerializeField] private VisualCastMagick _visualCastMagick;
        [SerializeField] private PlayerHealthAndMana _playerHealth;
        [SerializeField] private MagickConfig _magickConfig;
        private float _price;
        private float _currentPrice;
        private float _power;
        private int[] _spells = new int[3];
        private Vector2 _fingerDownPosition;
        private Vector2 _fingerUpPosition;


        private float _aard;
        private float _aksi;
        private float _igni;
        private float _irden;
        private float _kwen;
        public void Initialize(float price)
        {
            EventBus.Register(this as IEventReceiver<LevelPassed>);
            this._price = price;
            _currentPrice = price;
            _button.onClick.AddListener(CastSpell);
            _visualCastMagick.OnRealizeMagick += RealizeMagick;
            _aard = _magickConfig.aard;
            _aksi = _magickConfig.aksi;
            _igni = _magickConfig.igni;
            _irden = _magickConfig.irden;
            _kwen = _magickConfig.kwen;
        }
        public void TakePower(float power) => _power = power;
        private void CastSpell()
        {
            if (_playerHealth.GetMana() >= _price)
            {
                EventBus.Raise(new ChangeTime(false, true));
                _visualCastMagick.StartCast();
                StartCoroutine(CastSpellCoroutine(5f));
                _button.gameObject.SetActive(false);
            }
        }
        private IEnumerator CastSpellCoroutine(float castTime)
        {
            float elapsed = 0f;
            bool create = false;
            int i = 0;
            yield return null;
            yield return null;
            _fingerDownPosition = _button.transform.position;
            _fingerUpPosition = _button.transform.position;
            while (elapsed < castTime)
            {
                if (i < 3)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _fingerDownPosition = Input.mousePosition;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        _fingerUpPosition = Input.mousePosition;
                        int test = DetectSwipe();
                        if (test != 0)
                        {
                            _spells[i] = test;
                            _visualCastMagick.DirVisual(i, test);
                            i++;
                        }
                    }
                }
                else
                {
                    create = Choise();
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (elapsed >= castTime && create == false)
            {
                _visualCastMagick.EndStartCast();
                _visualCastMagick.OffDir(false);
            }
            for (int j = 0; j < _spells.Length; j++)
            {
                _spells[j] = 0;
            }
            if (create)
            {
                _playerHealth.CurrentMana(_currentPrice);
                _currentPrice = _price;
            }
            
        }
        private bool Choise()
        {
            bool create = true;
            int i = -1;
            if (_spells[0] == 1 && _spells[1] == 3 && _spells[2] == 2)
            {
                i = 4;
            }
            else if (_spells[0] == 2 && _spells[1] == 1 && _spells[2] == 2)
            {
                i = 3;
            }
            else if (_spells[0] == 3 && _spells[1] == 4 && _spells[2] == 2)
            {
                i = 0;
            }
            else if (_spells[0] == 4 && _spells[1] == 3 && _spells[2] == 1)
            {
                i = 2;
            }
            else if (_spells[0] == 4 && _spells[1] == 2 && _spells[2] == 3)
            {
                i = 1;
            }
            else
            {
                _visualCastMagick.EndStartCast();
                _visualCastMagick.OffDir(false);
                create = false;
            }
            if(i != -1) _visualCastMagick.EndAnimation(i);
            return create;
        }
        private void RealizeMagick(int number)
        {
            EventBus.Raise(new ChangeTime(true, true));
            StartCoroutine(RealizeMagickWithDelay(number));
            _button.gameObject.SetActive(true);
        }
        private IEnumerator RealizeMagickWithDelay(int number)
        {
            if (number == 0)
            {
                EventBus.Raise(new PlayerAttackEvent(_aard * _power, AttackType.Cold));
            }
            else if (number == 1)
            {
                EventBus.Raise(new Aksi(_aksi * _power));
            }
            else if (number == 2)
            {
                yield return new WaitForSeconds(0.6f);
                EventBus.Raise(new PlayerAttackEvent(_igni * _power, AttackType.Fire));
            }
            else if (number == 3)
            {
                yield return new WaitForSeconds(0.6f);
                EventBus.Raise(new PlayerAttackEvent(_irden * _power, AttackType.Magick));
            }
            else if (number == 4)
            {
                _playerHealth.Kwen(_kwen * _power);
            }
        }

        public void CurrentPrice() => _currentPrice = 0;
        public void TakePower(float k, bool K) => _power *= k;

        private int DetectSwipe()
        {
            int dir = 0;
            if (SwipeDistanceCheck())
            {
                Vector2 direction = _fingerUpPosition - _fingerDownPosition;
                direction.Normalize();
                if (IsVerticalSwipe(direction))
                {
                    return direction.y > 0 ? 1 : 2; // 1 - вверх, 2 - вниз
                }

                return direction.x > 0 ? 3 : 4; // 3 - вправо, 4 - влево
            }
            return dir;
        }
        private bool SwipeDistanceCheck()
        {
            return Vector2.Distance(_fingerDownPosition, _fingerUpPosition) >= minSwipeDistance;
        }
        private bool IsVerticalSwipe(Vector2 direction)
        {
            return Mathf.Abs(direction.y) > Mathf.Abs(direction.x);
        }

        public UniqueId Id { get; } = new UniqueId();
        public void OnEvent(LevelPassed @event)
        {
            StopAllCoroutines();
            EventBus.Unregister(this as IEventReceiver<LevelPassed>);
            gameObject.SetActive(false);
        }
    }
}
