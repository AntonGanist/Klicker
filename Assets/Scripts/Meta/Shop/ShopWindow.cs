using System.Collections.Generic;
using Game.Skills.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.Shop
{
    public class ShopWindow : MonoBehaviour
    {
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        public Button OffButton;

        [SerializeField] private List<GameObject> _blocks;

        [SerializeField] private List<ShopItem> _items;

        [SerializeField] private EquipMutagenShow _equipMutagenShow;

        private Dictionary<string, ShopItem> _itemsMap;
        private int _currentBlock = 0;
        private MoneyCounter _moneyCounter;
        private SkillCounter _skillCounter;
        private int _equpMutagen;

        public void Initialize(MoneyCounter moneyCounter, SkillCounter skillCounter)
        {
            _moneyCounter = moneyCounter;
            _skillCounter = skillCounter;            

            InitializeItemMap();

            InitializeBlockSwitching();
            ShowShopItems();

            _equpMutagen = _skillCounter.GetEquipMutagens();
            _equipMutagenShow.Initialize(_skillCounter.Skills, _equpMutagen);
        }

        private void ShowShopItems()
        {
            foreach (var openSkill in _skillCounter.Skills)
            {
                var skillData = openSkill.skillData;
                if (!_itemsMap.ContainsKey(skillData.SkillId)) continue;
                bool isOffConditions = false, offBye = false;

                if (skillData.Variants == SkillsVariants.Condition)
                {
                    isOffConditions = CheckConditionsSkill(skillData);
                    offBye = false;
                }
                else if (skillData.Variants == SkillsVariants.Mutagen)
                {
                    isOffConditions = CheckMutagensSkill(skillData);
                    offBye = false;
                }
                if (isOffConditions == false) offBye = _moneyCounter.GetMoney() >= skillData.Cost;
                if (skillData.Max) offBye = true;
                if (skillData.Unlock == true)
                {
                    _itemsMap[skillData.SkillId].SetActive(true);
                    if(skillData.Variants != SkillsVariants.Mutagen)
                        _itemsMap[skillData.SkillId].Initialize
                            (skillId => SkillUpgrade(skillId, skillData.Cost),
                            skillData.ShopName,
                            skillData.Value,
                            skillData.TriggerValue * 100,
                            skillData.Description,
                            skillData.Cost,
                            offBye);
                    else
                        _itemsMap[skillData.SkillId].GetComponent<MutagenButton>().Initialize
                            (skillId => SkillUpgrade(skillId, skillData.Cost),
                            skillData.ShopName,
                            skillData.Value,
                            skillData.TriggerValue * 100,
                            skillData.Description,
                            skillData.Cost,
                            offBye,
                            skillId => EquipSkill(skillId),
                            skillId => SaleSkill(skillId, skillData.Cost),
                            skillData.Use);
                }
                else
                {
                    _itemsMap[skillData.SkillId].SetActive(false);
                }
            }
        }
        private void InitializeItemMap()
        {
            _itemsMap = new();
            foreach (var shopItem in _items)
            {
                _itemsMap[shopItem.SkillId] = shopItem;
            }
        }

        private void SkillUpgrade(string skillId, int cost)
        {
            if (_moneyCounter.GetMoney() - cost < 0) return;
            var skillData = _skillCounter.GetSkillData(skillId);

            if (CheckConditionsSkill(skillData))return;

            if (skillData.TriggerValue >= 1 && skillData.Value == 0)
            {
                skillData.TriggerValue = 1;
                skillData.Max = true;
                return;
            }
            if (skillData.Value >= skillData.LimitValue && skillData.LimitValue != 0)
            {
                skillData.Max = true;
                return;
            }


            skillData.Value += skillData.ChangeValue;
            skillData.TriggerValue += skillData.ChangeTriggerValue;
            if (skillData.TriggerValue > 1)
                skillData.TriggerValue = 1;
            if (skillData.Variants == SkillsVariants.Basa) skillData.Cost += 2;

            _moneyCounter.ChangeStoreMoney(cost);
            _skillCounter.ChangeSkill(skillData);

            _moneyCounter.Save();
            _skillCounter.Save();
            ShowShopItems();
        }
        private void SaleSkill(string skillId, int cost)
        {
            var skillData = _skillCounter.GetSkillData(skillId);

            skillData.Use = false;
            skillData.Unlock = false;

            _moneyCounter.ChangeStoreMoney(-cost);
            _skillCounter.ChangeSkill(skillData);
            _equipMutagenShow.UnEquip(skillId);

            _moneyCounter.Save();
            _skillCounter.Save();
            ShowShopItems();
        }
        private void EquipSkill(string skillId)
        {
            var skillData = _skillCounter.GetSkillData(skillId);

            bool can = !skillData.Use;
            if (_equpMutagen == 3 && can == true) return;
            skillData.Use = !skillData.Use;

            if(skillData.Use == true)
            {
                _equipMutagenShow.Equip(skillId);
                _equpMutagen++;
            }
            else
            {
                _equipMutagenShow.UnEquip(skillId);
                _equpMutagen--;
            }

            _skillCounter.ChangeSkill(skillData);

            _skillCounter.Save();
            ShowShopItems();
        }

        private bool CheckConditionsSkill(SkillData skillData)
        {
            bool change = false;
            if(skillData.SkillId == "HealingPotionsSkill" || skillData.SkillId == "ManaPotionsSkill")
            {
                if(skillData.Value >= 7) change = true;
            }
            else if (skillData.SkillId == "CorpseEaterOilSkill" || skillData.SkillId == "OgrOilSkill" || 
                skillData.SkillId == "MagickOilSkill" || skillData.SkillId == "BeastOilSkill")
            {
                var oilSkills = new string[] { "CorpseEaterOilSkill", "OgrOilSkill", "MagickOilSkill", "BeastOilSkill" };

                foreach (var oilSkillId in oilSkills)
                {
                    var oilSkill = _skillCounter.GetSkillData(oilSkillId);
                    if (oilSkill.Value > 0)
                    {
                        change = true;
                        break;
                    }
                }
            }
           return change;
        }
        private bool CheckMutagensSkill(SkillData skillData)
        {
            bool change = false;
            int number = 0;
            foreach(var skill in _skillCounter.Skills)
            {
                if (skill.skillData.Variants == SkillsVariants.Mutagen && skill.skillData.Use == true) number++;
            }
            if(number >= 3)change = true;
            return change;
        }


        private void InitializeBlockSwitching()
        {
            _previousButton.onClick.AddListener(() => ShowBlock(_currentBlock - 1));
            _nextButton.onClick.AddListener(() => ShowBlock(_currentBlock + 1));
            ShowBlock(_currentBlock);
        }
        private void ShowBlock(int index)
        {
            for (var i = 0; i < _blocks.Count; i++)
            {
                _currentBlock = (index + _blocks.Count) % _blocks.Count;
                _blocks[i].SetActive(i == _currentBlock);
            }
        }

        public int MinCost()
        {
            int cost = 0;
            for (int i = 0; i < _skillCounter.Skills.Count; ++i)
                cost += _skillCounter.Skills[i].skillData.Cost;
            return cost / _skillCounter.Skills.Count;
        }
        public void SetActive(bool active) => gameObject.SetActive(active);
    }
}