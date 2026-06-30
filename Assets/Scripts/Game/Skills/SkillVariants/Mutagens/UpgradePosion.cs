using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class UpgradePosion : Skill
    {
        private SkillData _skillData;
        public ClickButtonManager _clickButtonManager;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _clickButtonManager = scope.ClickButtonManager;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                _clickButtonManager.ChangePowerPosion(_skillData.TriggerValue);
            }
        }
    }
}
