using System.Collections.Generic;
using Game.Skills.Data;
using Global.SaveSystem.SavableObjects;
using UnityEngine;

namespace Game.Configs.SkillsConfig
{
    [CreateAssetMenu(menuName = "Configs/SkillsConfig", fileName = "SkillsConfig")]
    public class SkillsConfig : ScriptableObject
    {
        public List<SkillData> Skills;

        private Dictionary<string, SkillData> _skillDataByLevelMap;
        public SkillData GetSkillData(string skillId)
        {
            if (_skillDataByLevelMap == null || _skillDataByLevelMap.Count == 0)
            {
                FillSkillDataMap();
            }

            return _skillDataByLevelMap[skillId];
        }

        private void FillSkillDataMap()
        {
            _skillDataByLevelMap = new();
            foreach (var skillData in Skills)
            {
                if (!_skillDataByLevelMap.ContainsKey(skillData.SkillId))
                {
                    _skillDataByLevelMap[skillData.SkillId] = skillData;
                }
            }
        }
        public void ChangeSkill(SkillData skillData)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (skillData.SkillId == Skills[i].SkillId)
                {
                    Skills[i] = new SkillData(skillData.Unlock,
                                skillData.SkillId,
                                skillData.Variants,
                                skillData.Value,
                                skillData.Trigger,
                                skillData.TriggerValue,
                                skillData.Cost,
                                skillData.ChangeValue,
                                skillData.ChangeTriggerValue,
                                skillData.ShopName,
                                skillData.Description,
                                skillData.Use
                    );
                    break;
                }

            }
        }
        public void StartsChangeSkills(List<SkillWithLevel> skillsWithLevel)
        {
            for (int i = 0; i < skillsWithLevel.Count; i++)
            {
                Skills[i] = skillsWithLevel[i].skillData;
            }
        }
    }
}