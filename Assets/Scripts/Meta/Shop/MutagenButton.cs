using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Meta.Shop
{
    public class MutagenButton : ShopItem
    {
        [SerializeField] private Button _equipButton;
        [SerializeField] private Image _equipImage;
        [SerializeField] private Button _saleButton;

        [SerializeField] private Color _canEquipColor;
        [SerializeField] private Color _cannotEquipColor;

        [SerializeField] private TextMeshProUGUI _equipText;
        [SerializeField] private string _equip;
        [SerializeField] private string _dontequip;

        public override void Initialize(
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
            _equipButton.onClick.RemoveAllListeners();
            _saleButton.onClick.RemoveAllListeners();

            base.Initialize(onClick, label, value, triggerValue, description, cost, isEnough);
            _equipButton.onClick.AddListener(() => onClickEquip?.Invoke(SkillId));
            _saleButton.onClick.AddListener(() => onClickSale?.Invoke(SkillId));
            _equipImage.color = use ? _cannotEquipColor : _canEquipColor;
            _equipText.text = use ? _dontequip : _equip;
        }
    }
}