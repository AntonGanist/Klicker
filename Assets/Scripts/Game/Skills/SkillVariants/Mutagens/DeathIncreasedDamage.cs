using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class DeathIncreasedDamage : Skill
    {
        private SkillData _skillData;
        private PlayerHealthAndMana _playerHealthAndMana;
        private ClickButtonManager _clickButtonManager;
        private bool _done = true;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _playerHealthAndMana = scope.PlayerHealth;
            _clickButtonManager = scope.ClickButtonManager;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use && _done)
            {
                if(_playerHealthAndMana.GetHealth() <= _playerHealthAndMana.GetMaxHealth() * 0.3f)
                {
                    _clickButtonManager.ChangeAttack(_skillData.Value);
                    _done = false;
                }
            }
        }
    }
}