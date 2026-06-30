using UnityEngine;
using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.Magick;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class FreeMagickCastSkill : Skill
    {
        private SkillData _skillData;
        private CreateMagick _createMagick;

        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _createMagick = scope.CreateMagick;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _skillData.TriggerValue) _createMagick.CurrentPrice();
            }
        }
    }
}
