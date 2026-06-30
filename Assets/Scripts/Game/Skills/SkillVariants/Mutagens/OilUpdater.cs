using Game.Enemies;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class OilUpdater : Skill
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
                _enemyManager.ChangeOilDamage(_skillData.TriggerValue);
            }
        }
    }
}
