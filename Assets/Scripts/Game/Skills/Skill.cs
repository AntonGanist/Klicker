using Game.Skills.Data;

namespace Game.Skills
{
    public abstract class Skill
    {
        public virtual void Initialize(SkillScope scope, SkillData skillData) { }
        public virtual void OnSkillRegistered() { }
        public virtual void SkillProcess() { }
    }
}