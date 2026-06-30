using Game.ClickButton;
using Game.Enemies;
using Game.Magick;

namespace Game.Skills
{
    public class SkillScope
    {
        public PlayerHealthAndMana PlayerHealth;
        public CreateMagick CreateMagick;
        public EnemyManager EnemyManager;
        public ClickButtonManager ClickButtonManager;
        public SkillCounter SkillCounter;
    }
}