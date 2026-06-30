using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class LastChance : Skill
    {
        private SkillData _skillData;
        private PlayerHealthAndMana _playerHealthAndMana;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _playerHealthAndMana = scope.PlayerHealth;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                _playerHealthAndMana.LastChanceMutagen(_skillData.Value);
            }
        }
    }
}