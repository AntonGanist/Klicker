using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class HealthSkill : Skill
    {
        private SkillData _skillData;
        public PlayerHealthAndMana PlayerHealth;


        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            PlayerHealth = scope.PlayerHealth;
        }

        public override void SkillProcess()
        {
            PlayerHealth.TakeHealth(_skillData.Value);
        }
    }
}