using System.Collections.Generic;
namespace Global.SaveSystem.SavableObjects
{
    public class OpenedSkills : ISavable
    {
        public List<SkillWithLevel> Skills = new() {
            new SkillWithLevel()
            {
                Id = "HealthSkill"
            },
            new SkillWithLevel()
            {
                Id = "ManaSkill"
            },
            new SkillWithLevel()
            {
                Id = "AttackSkill"
            },
            new SkillWithLevel()
            {
                Id = "CritAttackSkill"
            },
            new SkillWithLevel()
            {
                Id = "ChanceCritAttackSkill"
            },
            new SkillWithLevel()
            {
                Id = "MagickPowerSkill"
            },
            new SkillWithLevel()
            {
                Id = "ChancePosionSkill"
            },
            new SkillWithLevel() 
            {
                Id = "VampirismSkill"
            },
            new SkillWithLevel()
            {
                Id = "RegenManaSkill"
            },
            new SkillWithLevel()
            {
                Id = "FreeMagickCastSkill"
            },
            new SkillWithLevel()
            {
                Id = "DoubledDamageSkill"
            },
            new SkillWithLevel()
            {
                Id = "AccumulatingDamageSkill"
            },
            new SkillWithLevel()
            {
                Id = "IncreaseMoneySkill"
            },
            new SkillWithLevel()
            {
                Id = "RegenerasionAllSkill"
            },
            new SkillWithLevel()
            {
                Id = "HealingPotionsSkill"
            },
            new SkillWithLevel()
            {
                Id = "ManaPotionsSkill"
            },
            new SkillWithLevel()
            {
                Id = "CorpseEaterOilSkill"
            },
            new SkillWithLevel()
            {
                Id = "OgrOilSkill"
            },
            new SkillWithLevel()
            {
                Id = "MagickOilSkill"
            },
            new SkillWithLevel()
            {
                Id = "BeastOilSkill"
            },
            new SkillWithLevel()
            {
                Id = "StrengtheningPotions"
            },
            new SkillWithLevel()
            {
                Id = "LastChance"
            },
            new SkillWithLevel()
            {
                Id = "UpgradePosion"
            },
            new SkillWithLevel()
            {
                Id = "ThickSkinned"
            },
            new SkillWithLevel()
            {
                Id = "OilUpdater"
            },
            new SkillWithLevel()
            {
                Id = "AccumulatingCritDamageSkill"
            },
            new SkillWithLevel()
            {
                Id = "RandomStatUp"
            },
            new SkillWithLevel()
            {
                Id = "DeathIncreasedDamage"
            },
            new SkillWithLevel()
            {
                Id = "VulnerabilityToMagic"
            }
        };
    }
}