using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class RegenManaSkill : Skill
    {
        private SkillData _skillData;

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
                    EventBus.Raise(new ManaRecovery(_skillData.Value));
            }
        }
    }
}