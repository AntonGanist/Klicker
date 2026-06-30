using System;
using UnityEngine;

namespace Game.Skills.Data
{
    [Serializable]
    public class SkillData
    {
        public bool Unlock;
        public string SkillId;
        public SkillsVariants Variants;
        public int Cost;
        public SkillTrigger[] Trigger;

        public float Value;
        [Range(0f, 1f)] public float TriggerValue;

        public float ChangeValue;
        public float LimitValue;
        public float ChangeTriggerValue;

        public string ShopName;
        public string Description;

        public bool Use;
        public bool Max;
        public SkillData(bool Unlock, string SkillId, SkillsVariants Variants, float Value, 
            SkillTrigger[] Trigger, float TriggerValue = 0, int Cost = 0, float ChangeValue = 0, 
            float ChangeTriggerValue = 0, string ShopName = "", string Description = "", bool Use = false, bool Max = false)
        {
            this.Unlock = Unlock;
            this.SkillId = SkillId;
            this.Variants = Variants;
            this.Value = Value;
            this.Trigger = Trigger;
            this.TriggerValue = TriggerValue;
            this.ChangeTriggerValue = ChangeTriggerValue;
            this.Cost = Cost;
            this.ChangeValue = ChangeValue;
            this.ShopName = ShopName;
            this.Description = Description;
            this.Use = Use;
            this.Max = Max;
        }
    }
}