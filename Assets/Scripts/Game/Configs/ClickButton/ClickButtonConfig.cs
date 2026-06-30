using UnityEngine;
using UnityEngine.UI;

namespace Game.ClickButton
{
    [CreateAssetMenu(menuName = "Configs/ClickButtonConfig", fileName = "ClickButtonConfig")]
    public class ClickButtonConfig : ScriptableObject
    {
        public Sprite DefaultSprite;
        public ColorBlock ButtonColors;

        public Sprite CriticalSprite;

        public Sprite CriticalComboSprite;
        public float Range;
    }
}