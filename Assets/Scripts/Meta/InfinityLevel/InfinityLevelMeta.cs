using Global.SaveSystem.SavableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfinityLevelMeta : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _startLevel;
    [SerializeField] private TextMeshProUGUI _textEnemyKilled;
    [SerializeField] private TextMeshProUGUI _textMaxMoney;
    [SerializeField] private TextMeshProUGUI _textBestCombo;
    public void Initialize(InfinityLevelStat infinityLevelStat, UnityAction startInfinityLevelCallback)
    {
        _startLevel.onClick.AddListener(() => startInfinityLevelCallback?.Invoke());
        _textEnemyKilled.text = infinityLevelStat.BestNumberMonstersKilled.ToString();
        _textMaxMoney.text = infinityLevelStat.MaxGetMoney.ToString();
        _textBestCombo.text = infinityLevelStat.BestCriticalCombo.ToString();
    }
}
