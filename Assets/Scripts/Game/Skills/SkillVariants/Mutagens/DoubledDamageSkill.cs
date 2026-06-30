using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.Enemies;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class DoubledDamageSkill : Skill
    {
        private SkillData _skillData;
        private EnemyManager _enemyManager;

        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _enemyManager = scope.EnemyManager;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _skillData.TriggerValue) _enemyManager.RepeatAttack();
            }
        }
    }
}