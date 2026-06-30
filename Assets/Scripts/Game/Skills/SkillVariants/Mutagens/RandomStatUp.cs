using Game.Skills.Data;
using UnityEngine.Scripting;
using Game.ClickButton;
using Game.Magick;
using UnityEngine;

namespace Game.Skills.SkillVariants
{
    [Preserve]
    public class RandomStatUp : Skill
    {
        private SkillData _skillData;
        private ClickButtonManager _clickButtonManager;
        private PlayerHealthAndMana _playerHealthAndMana;
        private CreateMagick _createMagick;

        public override void Initialize(SkillScope scope, SkillData skillData)
        {
            _skillData = skillData;
            _clickButtonManager = scope.ClickButtonManager;
            _playerHealthAndMana = scope.PlayerHealth;
            _createMagick = scope.CreateMagick;
        }

        public override void SkillProcess()
        {
            if (_skillData.Use)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _skillData.TriggerValue)
                {
                    int x = Random.Range(1, 8);
                    if (x == 1)
                    {
                        _playerHealthAndMana.TakeHealth(_skillData.Value + 1, false);
                    }
                    else if (x == 2)
                    {
                        _playerHealthAndMana.TakeMana(_skillData.Value + 1, false);
                    }
                    else if (x == 3)
                    {
                        _clickButtonManager.ChangeAttack(_skillData.Value + 1, false);
                    }
                    else if (x == 4)
                    {
                        _clickButtonManager.ChangeCritAttack(_skillData.Value + 1);
                    }
                    else if (x == 5)
                    {
                        _clickButtonManager.ChangeChanceCrit(_skillData.Value + 1);
                    }
                    else if (x == 6)
                    {
                        _createMagick.TakePower(_skillData.Value + 1, false);
                    }
                    else if (x == 7)
                    {
                        _clickButtonManager.ChangeChancePosion(_skillData.Value + 1);
                    }
                }
            }
        }
    }
}