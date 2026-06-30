using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class CritAttackSkill : Skill
    {
        private SkillData _skillData;
        public ClickButtonManager ClickButtonManager;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            ClickButtonManager = scope.ClickButtonManager;
        }

        public override void SkillProcess()
        {
            ClickButtonManager.InitializeClickButtonCritical(_skillData.Value);
        }
    }
}