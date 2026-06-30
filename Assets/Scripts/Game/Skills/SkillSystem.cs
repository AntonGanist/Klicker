using System;
using System.Collections.Generic;
using Game.ClickButton;
using Game.Enemies;
using Global.SaveSystem.SavableObjects;

namespace Game.Skills
{
    public class SkillSystem
    {
        private SkillScope _scope;
        private SkillCounter _skillCounter;
        private Dictionary<SkillTrigger, List<Skill>> _skillsByTrigger;

        public SkillSystem(OpenedSkills openedSkills, PlayerHealthAndMana playerHealth,
            Game.Magick.CreateMagick _createMagick, EnemyManager enemyManager, ClickButtonManager clickButtonManager,
            SkillCounter skillCounter)
        {
            _skillCounter = skillCounter;
            _skillsByTrigger = new();
            _scope = new()
            {
                PlayerHealth = playerHealth,
                CreateMagick = _createMagick,
                EnemyManager = enemyManager,
                ClickButtonManager = clickButtonManager,
                SkillCounter = skillCounter
            };
            foreach (var skill in openedSkills.Skills)
            {
                RegisterSkill(skill);
            }
        }
        public void InvokeTrigger(SkillTrigger trigger)
        {
            if (!_skillsByTrigger.ContainsKey(trigger)) return;

            var skillsToActivate = _skillsByTrigger[trigger];

            foreach (var skill in skillsToActivate)
            {
                skill.SkillProcess();
            }
        }

        private void RegisterSkill(SkillWithLevel skill)
        {
            var skillData = _skillCounter.GetSkillData(skill.Id);

            var skillType = Type.GetType($"Game.Skills.SkillVariants.{skill.Id}");
            if (skillType == null)
            {
                throw new($"Skill with id {skill.Id} not found");
            }

            if (Activator.CreateInstance(skillType) is not Skill skillInstance)
            {
                throw new($"can not create skill with id {skill.Id}");
            }

            skillInstance.Initialize(_scope, skillData);

            foreach (var trigger in skillData.Trigger)
            {
                if (!_skillsByTrigger.ContainsKey(trigger))
                {
                    _skillsByTrigger[trigger] = new();
                }
                _skillsByTrigger[trigger].Add(skillInstance);
            }
            skillInstance.OnSkillRegistered();
        }
    }
}