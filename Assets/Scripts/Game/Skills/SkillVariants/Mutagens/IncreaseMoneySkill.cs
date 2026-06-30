using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.Magick;
using Game.Enemies;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class IncreaseMoneySkill : Skill
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
                _enemyManager.IncreaseMoney(_skillData.Value);
        }
    }
}
