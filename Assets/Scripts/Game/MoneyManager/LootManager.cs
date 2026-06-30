using System.Collections.Generic;
using UnityEngine;
using static CoinsConfig;
using static MutagenConfig;

public class LootManager : MonoBehaviour
{
    [SerializeField] private CoinsConfig _coinsConfig;
    [SerializeField] private MutagenConfig _mutagenConfig;
    [SerializeField] private Transform _lootPosition;
    private MoneyCounter _moneyCounter;
    private SkillCounter _skillCounter;
    private PoolMono<MoneyDrop> _poolMoney;
    private PoolMono<DropMutagen> _poolMutagen;
    private MoneyDrop _moneyPrefab;
    private DropMutagen _mutagenPrefab;
    private int[] _moneyDenominations = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };
    private float _increase;

    private List<string> _mutagenId = new List<string>();
    private int _location;
    public void Initialize(MoneyCounter moneyCounter, SkillCounter skillCounter)
    {
        _moneyCounter = moneyCounter;
        _skillCounter = skillCounter;
        _moneyPrefab = _coinsConfig.MoneyPrefab;
        _mutagenPrefab = _mutagenConfig.MutagenPrefab;

        _poolMoney = new PoolMono<MoneyDrop>(_moneyPrefab, 10, transform);
        _poolMoney.autoExpand = true;
        _poolMutagen = new PoolMono<DropMutagen>(_mutagenPrefab, 2, transform);
        _poolMutagen.autoExpand = true;

        foreach(var openSkill in _skillCounter.Skills)
        {
            if (openSkill.skillData.Variants == SkillsVariants.Mutagen
                && openSkill.skillData.Unlock == false)
                _mutagenId.Add(openSkill.skillData.SkillId);
        }
    }
    public void TakeLocation(int loc) { _location = loc; }
    public void IncreaseMoney(float amount) => _increase = amount / 100;

    public void DropMoney(int lastMoneyAmount)
    {
        float lastMoneyAmountF = lastMoneyAmount;
        lastMoneyAmountF = lastMoneyAmountF + lastMoneyAmountF * _increase;
        List<int> bestCombination = FindBestCombination(lastMoneyAmountF, 10);
        foreach (int denom in bestCombination)
        {
            var money = _poolMoney.GetFreeElement();
            MoneysViewData moneyViewData = _coinsConfig.Moneys[_coinsConfig.GetMoneysViewData(denom)];
            money.Initialize(denom, _moneyCounter, _lootPosition, moneyViewData);
        }
    }
    public void DropMutagen()
    {
        if(_mutagenId.Count != 0)
        {
            var mutagen = _poolMutagen.GetFreeElement();
            int number = UnityEngine.Random.Range(0, _mutagenId.Count);
            MutagenViewData ViewData = _mutagenConfig.Mutagens[_mutagenConfig.GetMutagenViewData(_mutagenId[number])];
            mutagen.Initialize(_mutagenId[number], _skillCounter, _lootPosition, ViewData, _location);
            _mutagenId.RemoveAt(number);
        }
    }
    private List<int> FindBestCombination(float amount, int maxCoins)
    {
        List<int> result = new List<int>();
        System.Array.Sort(_moneyDenominations);
        System.Array.Reverse(_moneyDenominations);

        foreach (int denom in _moneyDenominations)
        {
            while (amount >= denom && result.Count < maxCoins)
            {
                result.Add(denom);
                amount -= denom;
            }
        }

        while (amount > 0 && result.Count < maxCoins)
        {
            result.Add(1);
            amount--;
        }

        return result;
    }
    public bool GetActive()
    {
        bool active = true;
        if(_poolMoney.GetActiveElement() == 0 && _poolMutagen.GetActiveElement() == 0)
            active = false;
        return active;
    }
}