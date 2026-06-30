using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class RegenerasionAllSkill : Skill
    {
        private SkillData _skillData;
        private bool _isHealthRegen;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _skillData.TriggerValue)
                {
                    _isHealthRegen = Random.Range(0, 2) == 1;
                    if (_isHealthRegen)
                    {
                        EventBus.Raise(new HealthRecovery(_skillData.Value));
                    }
                    else
                    {
                        EventBus.Raise(new ManaRecovery(_skillData.Value));
                    }
                }
            } 
        }
    }
}
