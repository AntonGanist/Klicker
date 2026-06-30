using Game.Skills.Data;
using Global.SaveSystem.SavableObjects;
using Global.SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using Game.Configs.SkillsConfig;

public class SkillCounter : MonoBehaviour, IEventReceiver<HealthRecovery>, IEventReceiver<ManaRecovery>
{
    public List<SkillWithLevel> Skills;
    [SerializeField] private SkillsConfig _skillsConfig;
    [SerializeField] private Formuls _formuls;
    private SaveSystem _saveSystem;
    private OpenedSkills _openedSkills;
    public void Initialize(SaveSystem saveSystem)
    {
        _saveSystem = saveSystem;
        EventBus.Register(this as IEventReceiver<HealthRecovery>);
        EventBus.Register(this as IEventReceiver<ManaRecovery>);

        _openedSkills = (OpenedSkills)saveSystem.GetData(SavableObjectType.OpenedSkills);
        Skills = _openedSkills.Skills;

        if (Skills[0].skillData.SkillId == "")
        {
            StartChangeSkill();
        }
        GetDataFormuls();
        //See();
    }
    private void See()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].skillData.SkillId == "OilUpdater")
            {
                Debug.Log(_skillsConfig.Skills[i].Unlock);
                Debug.Log(_skillsConfig.Skills[i].SkillId);
                Debug.Log(_skillsConfig.Skills[i].Variants);
                Debug.Log(_skillsConfig.Skills[i].Value);
                Debug.Log(_skillsConfig.Skills[i].Trigger[0]);
                Debug.Log(_skillsConfig.Skills[i].TriggerValue);
                Debug.Log(_skillsConfig.Skills[i].Cost);
                Debug.Log(_skillsConfig.Skills[i].ChangeValue);
                Debug.Log(_skillsConfig.Skills[i].ChangeTriggerValue);
                Debug.Log(_skillsConfig.Skills[i].ShopName);
                Debug.Log(_skillsConfig.Skills[i].Description);
                Debug.Log(_skillsConfig.Skills[i].Use);
                Debug.Log(_skillsConfig.Skills[i].Max);
            }
        }
    }
    private void StartChangeSkill()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            Skills[i].skillData = new SkillData(_skillsConfig.Skills[i].Unlock,
                _skillsConfig.Skills[i].SkillId,
                _skillsConfig.Skills[i].Variants,
                _skillsConfig.Skills[i].Value,
                _skillsConfig.Skills[i].Trigger,
                _skillsConfig.Skills[i].TriggerValue,
                _skillsConfig.Skills[i].Cost,
                _skillsConfig.Skills[i].ChangeValue,
                _skillsConfig.Skills[i].ChangeTriggerValue,
                _skillsConfig.Skills[i].ShopName,
                _skillsConfig.Skills[i].Description,
                _skillsConfig.Skills[i].Use,
                _skillsConfig.Skills[i].Max
                );
            if (Skills[i].skillData.Variants == SkillsVariants.Mutagen &&
                Skills[i].skillData.Use == true)
            _formuls.CheakSkills(Skills[i].skillData);
        }
    }
    public void ChangeSkill(SkillData skillData)
    {
        var skill = Skills.Find(s => s.skillData.SkillId == skillData.SkillId);
        skill.skillData = CreateSkillData(skillData);
        _formuls.CheakSkills(skill.skillData);
    }

    private void ProcessPotionUse(string skillId, float valueChange)
    {
        var skill = Skills.Find(s => s.skillData.SkillId == skillId);
        if (skill.skillData.Value <= 0) return;
        skill.skillData = CreateSkillData(skill.skillData, valueChange);
        _formuls.CheakSkills(skill.skillData);

        Save();
    }

    public void OnEvent(HealthRecovery @event) => ProcessPotionUse("HealingPotionsSkill", -1);
    public void OnEvent(ManaRecovery @event) => ProcessPotionUse("ManaPotionsSkill", -1);

    private SkillData CreateSkillData(SkillData configData, float valueModifier = 0)
    {
        return new SkillData(
            configData.Unlock,
            configData.SkillId,
            configData.Variants,
            configData.Value + valueModifier,
            configData.Trigger,
            configData.TriggerValue,
            configData.Cost,
            configData.ChangeValue,
            configData.ChangeTriggerValue,
            configData.ShopName,
            configData.Description,
            configData.Use
        );
    }
    public SkillData GetSkillData(string skillId)
    {
        foreach (var skillWithLevel in Skills)
        {
            if (skillWithLevel.skillData.SkillId == skillId)
            {
                return skillWithLevel.skillData;
            }
        }
        return null;
    }
    public int GetEquipMutagens()
    {
        int y = 0;
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].skillData.Variants == SkillsVariants.Mutagen && Skills[i].skillData.Use == true)
            {
                y++;
            }
        }
        return y;
    }
    public void TakeMutagen(string skillId, int loc)
    {
        foreach (var skillWithLevel in Skills)
        {
            if (skillWithLevel.skillData.SkillId == skillId)
            {
                skillWithLevel.skillData.Unlock = true;
                for(int i = 0; i < loc; i++)
                {
                    skillWithLevel.skillData.Value += skillWithLevel.skillData.ChangeValue;
                    skillWithLevel.skillData.TriggerValue += skillWithLevel.skillData.ChangeTriggerValue;
                }
                _formuls.CheakSkills(skillWithLevel.skillData);
                break;
            }
        }
    }
    public void Save()
    {
        _openedSkills.Skills = Skills;
        _saveSystem.SaveData(SavableObjectType.Wallet);
    }
    public UniqueId Id { get; } = new UniqueId();
    private void GetDataFormuls()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            _formuls.CheakSkills(Skills[i].skillData);
        }
    }
}
