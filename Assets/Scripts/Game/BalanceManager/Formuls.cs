using Game.Configs.LevelConfigs;
using Game.Skills.Data;
using System.Collections.Generic;
using UnityEngine;
public struct Mutagen
{
    public string Name;
    public float Chance;
    public float Value;
    public bool Use;
}
public class Formuls : MonoBehaviour
{
    public float MaxAttackSpeed = 1.2f;
    [SerializeField] private MagickConfig _magickConfig;
    [SerializeField] private float _bossDamageMultiplicator = 1.2f;
    [SerializeField] private float _playerTimeToKill = 30f;
    [SerializeField] private float _minAttackSpeed = 3f;
    [SerializeField] private int _bossHitsToSurvive = 15;

    private float _powerPlayerAttack;
    private float _powerPlayerHealth;


    private float _health;
    private float _mana;
    private float _attack;
    private float _critAttack;
    private float _critChance;
    private float _magickPower;
    private float _usePosionChance;

    private int _quantityHealthPosion;
    private int _quantityManaPosion;

    private List<Mutagen> _mutagens = new List<Mutagen>();

    public void CheakSkills(SkillData skillData)
    {
        if(skillData.SkillId == "HealthSkill")
            _health = skillData.Value;
        else if(skillData.SkillId == "ManaSkill")
            _mana = skillData.Value;
        else if(skillData.SkillId == "AttackSkill")
            _attack = skillData.Value;
        else if(skillData.SkillId == "CritAttackSkill")
            _critAttack = skillData.Value;
        else if(skillData.SkillId == "ChanceCritAttackSkill")
            _critChance = skillData.TriggerValue;
        else if (skillData.SkillId == "MagickPowerSkill")
            _magickPower = skillData.Value;
        else if(skillData.SkillId == "ChancePosionSkill")
            _usePosionChance = skillData.TriggerValue;
        else if(skillData.SkillId == "HealingPotionsSkill")
            _quantityHealthPosion = (int)skillData.Value;
        else if (skillData.SkillId == "ManaPotionsSkill")
            _quantityManaPosion = (int)skillData.Value;
        else 
        {
            Mutagen mutagen = new Mutagen();
            mutagen.Name = skillData.SkillId;
            mutagen.Chance = skillData.TriggerValue;
            mutagen.Value = skillData.Value;
            mutagen.Use = skillData.Use;
            if(_mutagens.Count != 0)
            {
                for (int i = 0; i < _mutagens.Count; i++)
                {
                    if (_mutagens[i].Name == mutagen.Name)
                    {
                        _mutagens.RemoveAt(i);
                        break;
                    }
                }
            }
            _mutagens.Add(mutagen);
        }
    }
    private float GetAttackPower()
    {
        float RegenManaSkill = 0;
        float FreeMagickCastSkill = 0;
        float AccumulatingDamageSkill = 1;
        float RegenerasionAllSkill = 0;
        float DoubledDamageSkill = 1;
        float StrengtheningPotions = 0;
        float UpgradePosion = 0.2f;
        float AccumulatingCritDamageSkill = 1;
        float RandomStatUp = 1;
        float DeathIncreasedDamage = 0;
        float VulnerabilityToMagic = 1;
        for (int i = 0; i < _mutagens.Count; i++)
        {
            if (_mutagens[i].Use == true)
            {
                if (_mutagens[i].Name == "RegenManaSkill")
                {
                    RegenManaSkill = _mutagens[i].Chance * _mutagens[i].Value * _critChance;
                }
                else if (_mutagens[i].Name == "FreeMagickCastSkill")
                {
                    FreeMagickCastSkill = _mutagens[i].Chance * 10;
                }
                else if (_mutagens[i].Name == "AccumulatingDamageSkill")
                {
                    AccumulatingDamageSkill = _mutagens[i].Value * 8;
                }
                else if (_mutagens[i].Name == "RegenerasionAllSkill")
                {
                    RegenerasionAllSkill = _mutagens[i].Value * _mutagens[i].Chance;
                }
                else if (_mutagens[i].Name == "DoubledDamageSkill")
                {
                    DoubledDamageSkill = 2 * _mutagens[i].Chance;
                }
                else if (_mutagens[i].Name == "StrengtheningPotions")
                {
                    StrengtheningPotions = _mutagens[i].Value * (_quantityManaPosion+ _quantityHealthPosion)* _usePosionChance;
                }
                else if (_mutagens[i].Name == "UpgradePosion")
                {
                    UpgradePosion = _mutagens[i].Chance;
                }
                else if (_mutagens[i].Name == "AccumulatingCritDamageSkill")
                {
                    AccumulatingCritDamageSkill = _mutagens[i].Value * 8;
                }
                else if (_mutagens[i].Name == "RandomStatUp")
                {
                    RandomStatUp += _mutagens[i].Value;
                    RandomStatUp = RandomStatUp / 7f;
                }
                else if (_mutagens[i].Name == "DeathIncreasedDamage")
                {
                    DeathIncreasedDamage = _mutagens[i].Value * _health * 0.3f;
                }
                else if (_mutagens[i].Name == "VulnerabilityToMagic")
                {
                    VulnerabilityToMagic += _mutagens[i].Chance;
                }
            }
        }
        float phisicalDamage = (_attack + StrengtheningPotions + DeathIncreasedDamage) * AccumulatingDamageSkill 
            + (_critAttack + _attack * 1.5f) * _critChance * AccumulatingCritDamageSkill;

        float magickPower = (_mana + _quantityManaPosion * _usePosionChance * UpgradePosion * _mana * 0.5f
            + RegenManaSkill + FreeMagickCastSkill + RegenerasionAllSkill) / 10f * 
            (((_magickConfig.igni+ _magickConfig.aard+ _magickConfig.irden) / 3f + phisicalDamage 
            * _magickConfig.aksi) * _magickPower * VulnerabilityToMagic);
        _powerPlayerAttack = (phisicalDamage + magickPower) * DoubledDamageSkill * RandomStatUp;
        return _powerPlayerAttack;
    }
    private float GetPlayerHealthPower()
    {
        float RegenManaSkill = 0;
        float FreeMagickCastSkill = 0;
        float VampirismSkill = 0;
        float RegenerasionAllSkill = 0;
        float LastChance = 0;
        float UpgradePosion = 0.2f;
        float RandomStatUp = 1;
        for (int i = 0; i < _mutagens.Count; i++)
        {
            if (_mutagens[i].Use == true)
            {
                if (_mutagens[i].Name == "RegenManaSkill")
                {
                    RegenManaSkill = _mutagens[i].Chance * _mutagens[i].Value * _critChance;
                }
                else if (_mutagens[i].Name == "FreeMagickCastSkill")
                {
                    FreeMagickCastSkill = _mutagens[i].Chance * 10;
                }
                else if (_mutagens[i].Name == "VampirismSkill")
                {
                    VampirismSkill = _mutagens[i].Value * _mutagens[i].Chance * _health * 3;
                }
                else if (_mutagens[i].Name == "RegenerasionAllSkill")
                {
                    RegenerasionAllSkill = _mutagens[i].Value * _mutagens[i].Chance;
                }
                else if (_mutagens[i].Name == "LastChance")
                {
                    LastChance = _mutagens[i].Value;
                }
                else if (_mutagens[i].Name == "UpgradePosion")
                {
                    UpgradePosion = _mutagens[i].Chance;
                }
                else if (_mutagens[i].Name == "RandomStatUp")
                {
                    RandomStatUp += _mutagens[i].Value;
                    RandomStatUp = RandomStatUp / 7f;
                }
            }
            
        }
        float cleanHealth = _health + _quantityHealthPosion * _usePosionChance * UpgradePosion * _health * 0.5f + LastChance;
        float magickProtection = (_mana + _quantityManaPosion * _usePosionChance * UpgradePosion * _mana * 0.5f
            + RegenManaSkill + FreeMagickCastSkill + RegenerasionAllSkill) / 10f *
            (_magickConfig.kwen * _magickPower);

        _powerPlayerHealth = (cleanHealth + magickProtection + VampirismSkill) * RandomStatUp;
        return _powerPlayerHealth;
    }

    public int CalculateLocationReward(int location)
    {
        int baseReward = 100;
        float locationMultiplier = 1.5f;

        return Mathf.RoundToInt(baseReward * Mathf.Pow(locationMultiplier, location - 1));
    }
    public void UpdateConsumablesPrices(int locationReward, SkillCounter skillCounter, 
        LevelsConfig levelsConfig, int _location, int _level)
    {
        string[] potionSkills = { "HealingPotionsSkill", "ManaPotionsSkill" };
        string[] oilSkills = { "CorpseEaterOilSkill", "OgrOilSkill", "MagickOilSkill", "BeastOilSkill" };
        string[] mutagenSkills = { "VampirismSkill", "RegenManaSkill", "FreeMagickCastSkill",
            "DoubledDamageSkill", "AccumulatingDamageSkill", "IncreaseMoneySkill", "RegenerasionAllSkill",
            "StrengtheningPotions", "LastChance", "UpgradePosion", "ThickSkinned", "OilUpdater","AccumulatingCritDamageSkill",
            "RandomStatUp", "DeathIncreasedDamage", "VulnerabilityToMagic"
        };

        int nextLocation = levelsConfig.GetLevel(_location, _level).Location + 1;
        int totalLevels = levelsConfig.GetMaxLevelOnLocation(nextLocation);

        if (totalLevels == 0)
        {
            totalLevels = levelsConfig.GetMaxLevelOnLocation(_location);
        }

        int potionsPerLevel = 2; 
        int oilsPerLevel = 1;

        int totalPotions = totalLevels * potionsPerLevel;
        int totalOils = totalLevels * oilsPerLevel;

        int totalConsumablesBudget = locationReward / 3;

        int potionsBudget = totalConsumablesBudget / 2;
        int oilsBudget = totalConsumablesBudget / 2;

        int potionPrice = Mathf.Max(7, potionsBudget / totalPotions);
        int oilPrice = Mathf.Max(7, oilsBudget / totalOils);

        foreach (var skillId in potionSkills)
        {
            var skill = skillCounter.GetSkillData(skillId);
            if (skill != null)
            {
                skill.Cost = potionPrice;
            }
        }

        foreach (var skillId in oilSkills)
        {
            var skill = skillCounter.GetSkillData(skillId);
            if (skill != null)
            {
                skill.Cost = oilPrice;
            }
        }
        foreach (var skillId in mutagenSkills)
        {
            var skill = skillCounter.GetSkillData(skillId);
            if(skill.Unlock == false)
                skill.Cost = (int)((float)skill.Cost*1.5f);
        }
    }
    public BossStats CalculateBossStats()
    {
        float playerHealth = GetPlayerHealthPower();
        float playerAttack = GetAttackPower();

        return new BossStats
        {
            Health = CalculateBossHealth(playerAttack),
            Damage = CalculateBossDamage(playerHealth),
            AttackSpeed = CalculateBossAttackSpeed()
        };
    }
    private float CalculateBossHealth(float playerAttackPower)
    {
        return playerAttackPower * _bossHitsToSurvive * _bossDamageMultiplicator;
    }
    private float CalculateBossDamage(float playerHealth)
    {
        return (playerHealth / _playerTimeToKill) * _bossDamageMultiplicator;
    }
    public float CalculateBossAttackSpeed()
    {
        return Random.Range(_minAttackSpeed, MaxAttackSpeed);
    }
}

public struct BossStats
{
    public float Health;
    public float Damage;
    public float AttackSpeed;
}
