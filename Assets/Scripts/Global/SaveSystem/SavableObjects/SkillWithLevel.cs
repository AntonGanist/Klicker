using System;
using Game.Skills;
using Game.Skills.Data;
namespace Global.SaveSystem.SavableObjects
{
    [Serializable]
    public class SkillWithLevel
    {
        public string Id;
        public SkillData skillData = new SkillData(false, "", SkillsVariants.Basa, 0, new SkillTrigger[0]);
    }
}