using Game.ClickButton;
using Game.Enemies;
using Game.Skills.Data;
using UnityEngine.Scripting;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class ThickSkinned : Skill
    {
        private SkillData _skillData;
        private PlayerHealthAndMana _playerHealthAndMana;
        private EnemyManager _enemyManager;
        private ClickButtonManager _clickButtonManager;
        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _playerHealthAndMana = scope.PlayerHealth;
            _enemyManager = scope.EnemyManager;
            _clickButtonManager = scope.ClickButtonManager;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                OilDamageType oilType1 = _enemyManager.GetOil();
                OilDamageType oilType2 = _clickButtonManager.GetOil();

                if (oilType2 == oilType1 && oilType1 != OilDamageType.None && oilType2 != OilDamageType.None)
                    _playerHealthAndMana.ChangeArmor(_skillData.TriggerValue);
                else
                    _playerHealthAndMana.ChangeArmor(0);
            }
        }
    }
}