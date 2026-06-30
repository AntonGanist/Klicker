using Global.SaveSystem.SavableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMutagenShow : MonoBehaviour
{
    [SerializeField] private Image[] _images = new Image[3];
    [SerializeField] private Sprite[] _sprites;
    private Dictionary<string, int> _skillIndexMap;
    private Dictionary<string, int> _equippedMutagens = new Dictionary<string, int>(); // Хранит SkillId и индекс изображения

    public void Initialize(List<SkillWithLevel> skills, int count)
    {
        _skillIndexMap = new Dictionary<string, int>();
        int mutagenCounter = 0;
        for (int i = 0; i < skills.Count; i++)
        {
            var skillData = skills[i].skillData;
            if (skillData.Variants == SkillsVariants.Mutagen)
            {
                _skillIndexMap[skillData.SkillId] = mutagenCounter;
                mutagenCounter++;
            }
        }
        if (count != 0)
        {
            foreach (var skill in skills)
            {
                if (skill.skillData.Variants == SkillsVariants.Mutagen && skill.skillData.Use)
                    Equip(skill.skillData.SkillId);
            }
        }
    }

    public void Equip(string skillId)
    {
        if (!_skillIndexMap.ContainsKey(skillId) || _equippedMutagens.ContainsKey(skillId))
            return;

        for (int i = 0; i < _images.Length; i++)
        {
            if (!_images[i].gameObject.activeSelf)
            {
                _images[i].sprite = _sprites[_skillIndexMap[skillId]];
                _images[i].gameObject.SetActive(true);
                _images[i].name = skillId;
                _equippedMutagens[skillId] = i; 
                return;
            }
        }

        Debug.LogWarning("Нет свободных слотов для мутагенов!");
    }

    public void UnEquip(string skillId)
    {
        if (_equippedMutagens.TryGetValue(skillId, out int slotIndex))
        {
            _images[slotIndex].gameObject.SetActive(false);
            _images[slotIndex].name = "EmptySlot";
            _equippedMutagens.Remove(skillId);
        }
    }
}