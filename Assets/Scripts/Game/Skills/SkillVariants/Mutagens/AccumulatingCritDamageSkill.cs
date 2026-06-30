using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class AccumulatingCritDamageSkill : Skill
    {
        private SkillData _skillData;
        private ClickButtonManager _clickButtonManager;

        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _clickButtonManager = scope.ClickButtonManager;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                _clickButtonManager.AccumulatingCritDamage(_skillData.Value);
            }
        }
    }
}