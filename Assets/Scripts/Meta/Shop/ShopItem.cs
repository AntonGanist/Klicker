using Game.Skills.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Meta.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _currentValueText;
        [SerializeField] private TextMeshProUGUI _currentTriggerValueText;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Button _mainButton;

        public string SkillId;

        public string ÑurrentValueText = "";
        public string ÑurrentValueEndText = "";
        public string ÑurrentTriggerValueText = "";
        public string ÑurrentTriggerEndValueText = "";

        public bool Mutagen;

        private int _costSkill;

        public virtual void Initialize(
            UnityAction<string> onClick,
            string label,
            float value,
            float triggerValue,
            string description,
            int cost,
            bool isEnough,
            UnityAction<string> onClickEquip = null,
            UnityAction<string> onClickSale = null,
            bool use = false)
        {
            if (_mainButton != null)
            {
                _mainButton.onClick.RemoveAllListeners();
                _mainButton.onClick.AddListener(() => onClick?.Invoke(SkillId));
            }

            _label.text = label;
            _description.text = description;

            value = Mathf.Round(value * 100f) / 100f;

            _currentValueText.text = ÑurrentValueText + TextReduction.Reduction(value) + ÑurrentValueEndText;
            _currentTriggerValueText.text = ÑurrentTriggerValueText + 
                triggerValue.ToString("F0") + ÑurrentTriggerEndValueText;

            _cost.text = TextReduction.Reduction(cost);
            if (_mainButton != null)
                _cost.color = isEnough ? Color.green : Color.red;
            if (_mainButton != null)
                _mainButton.interactable = isEnough;
            _costSkill = cost;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public int GetCost() => _costSkill;
    }
}