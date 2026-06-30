using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;
using System.Diagnostics;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class ManaPotionsSkill : Skill
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
            _clickButtonManager.InitializeManaPosion((int)_skillData.Value);
        }
    }
}
