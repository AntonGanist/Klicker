using Global.SaveSystem.SavableObjects;
using Global.SaveSystem;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour, IEventReceiver<LevelPassed>, IEventReceiver<Unsubscribe>
{
    private int _moneyCount;
    private int _testMoneyCount;
    private int _countMoneyInLevev;
    [SerializeField] private TextMeshProUGUI _text;
    private SaveSystem _saveSystem;
    public void Initialize(SaveSystem saveSystem)
    {
        EventBus.Register(this as IEventReceiver<LevelPassed>);
        EventBus.Register(this as IEventReceiver<Unsubscribe>);
        _saveSystem = saveSystem;
        var _wallet = (Wallet)_saveSystem.GetData(SavableObjectType.Wallet);
        _moneyCount = _wallet.Coins;
        TextChange(_moneyCount);
    }

    public int GetMoney() => _moneyCount;
    public int GetTestMoney() => _countMoneyInLevev;

    public void OnEvent(LevelPassed @event)
    {
        if (@event.isPassed || !@event.isPassed && @event.isEndless)
        {
            _moneyCount = _testMoneyCount;
            Save();
        }
        else
        {
            _testMoneyCount = _moneyCount;
            TextChange(_testMoneyCount);
        }
    }
    public void OnEvent(Unsubscribe @event)
    {
        if (_testMoneyCount != _moneyCount)
        {
            _testMoneyCount = _moneyCount;
            TextChange(_testMoneyCount);
        }
        _countMoneyInLevev = 0;
    }

    public void ChangeTestMoney(int current)
    {
        _testMoneyCount += current;
        _countMoneyInLevev += current;
        TextChange(_testMoneyCount);
    }
    public void ChangeStoreMoney(int current)
    {
        _moneyCount -= current;
        _countMoneyInLevev = _moneyCount;
        TextChange(_moneyCount);
    }

    public void Save()
    {
        var _wallet = (Wallet)_saveSystem.GetData(SavableObjectType.Wallet);
        _wallet.Coins = _moneyCount;
        _saveSystem.SaveData(SavableObjectType.Wallet);
    } 

    private void TextChange(float current)
    {
        string txt = "";
        string formattedValue = "0";
        int del = 1;
        if(current > 9)
        {
            if (current >= 1000000000)
            {
                txt = " לכנה";
                del = 1000000000;
            }
            else if (current >= 1000000)
            {
                txt = " לכם";
                del = 1000000;
            }
            else if (current >= 1000)
            {
                txt = " עûס";
                del = 1000;
            }

            float currentMoney = current / del;
            if (currentMoney != 0)
            {
                if (currentMoney >= 10f)
                {
                    formattedValue = Mathf.RoundToInt(currentMoney).ToString();
                }
                else
                {
                    formattedValue = currentMoney.ToString("0.0");
                }
            }
        }
        else
        {
            formattedValue = current.ToString();
        }
        _text.text = formattedValue + txt;
    }
    public string CountMoneyInLevel() => TextReduction.Reduction(_countMoneyInLevev);

    public UniqueId Id { get; } = new UniqueId();
}
