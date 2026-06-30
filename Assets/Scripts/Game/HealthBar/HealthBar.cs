using UnityEngine;
using UnityEngine.UI;

namespace Game.HealthBar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] protected Slider _slider;

        public void Show()
        {
            _slider.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _slider.gameObject.SetActive(false);
        }

        public void SetMaxValue(float value)
        {
            _slider.maxValue = value;
            _slider.value = value;
        }

        public virtual void DecreaseValue(float value)
        {
            _slider.value -= value;
        }
        public void IncreaseValue(float value)
        {
            _slider.value += value;
        }
        public void ChangeValue(float value)
        {
            _slider.value = value;
        }
        public virtual void DecreaseValue(float value, Vector3 pos, AttackType attackType)
        {
            DecreaseValue(value);
        }
    }
}