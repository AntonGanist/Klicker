using Game.Magick;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class MagickPowerSkill : Skill
    {
        private SkillData _skillData;
        public CreateMagick CreateMagick;


        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            CreateMagick = scope.CreateMagick;
        }

        public override void SkillProcess()
        {
            CreateMagick.TakePower(_skillData.Value);
        }
    }
}
