using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Enemies;
using Game.ClickButton;

[Serializable]
public struct DialogData
{
    public string Text;
    public bool Variants;
    public int DefeatEnemy;
    public string[] VariantTexts;
    public int[] VariantTargets;
    public int NextDialogIndex;

    public int AnimationNumber;
    public int Reward;
    public bool UseCrit;
    public bool UsePosion;
}

public class DialogSystem : MonoBehaviour, IEventReceiver<EnemyDead>, IEventReceiver<Unsubscribe>
{
    [SerializeField] private GameObject _dialogPanel;
    [SerializeField] private GameObject[] _otherUI;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TextMeshProUGUI[] _buttonsTexts;
    [SerializeField] private ShowText _showText;
    [SerializeField] private VisualDialog _visualDialog;
    [SerializeField] private ClickButtonManager _clickButtonManager;
    private MoneyCounter _moneyCounter;

    private DialogData[] _dialogDatas;
    private float _interval = 0;
    private float _autoSkipDelay;

    private int _currentDialogIndex = 0;
    private Coroutine _autoSkipCoroutine;
    private bool _isTextComplete = false;
    private bool _waitingForChoice = false;
    private bool _endDialog = false;

    public void Initialize(DialogConfig dialogConfig, int location, int level, MoneyCounter moneyCounter)
    {
        EventBus.Register(this as IEventReceiver<EnemyDead>);
        EventBus.Register(this as IEventReceiver<Unsubscribe>);

        InitializeButtons();
        _dialogDatas = dialogConfig.GetDialogData(location, level);
        _moneyCounter = moneyCounter;
        _interval = dialogConfig.Interval;
        _autoSkipDelay = dialogConfig.AutoSkipDelay;
        _visualDialog.Initialize(dialogConfig.GetAnimatorController(location, level));
        _currentDialogIndex = FindFirstDialogWithDefeatEnemy(-1);
        if (_currentDialogIndex >= 0)
        {
            EventBus.Raise(new ChangeTime(false));
            OffOnUI(true);
            ShowNextDialog();
        }
        else
            OffOnUI(false);
    }

    private int FindFirstDialogWithDefeatEnemy(int defeatEnemyValue)
    {
        for (int i = 0; i < _dialogDatas.Length; i++)
        {
            if (_dialogDatas[i].DefeatEnemy == defeatEnemyValue)
            {
                return i;
            }
        }
        return -1;
    }
    public void OnEvent(EnemyDead @event)
    {
        _currentDialogIndex = FindFirstDialogWithDefeatEnemy(@event.Number);
        if (_currentDialogIndex >= 0 && _interval != 0)
        {
            _endDialog = false;
            EventBus.Raise(new ChangeTime(false));
            OffOnUI(true);
            ShowNextDialog();
        }
    }
    public void OffDialog() => _dialogPanel.SetActive(false);
    private void OffOnUI(bool active)
    {
        _enemyManager.Dialog(active);
        _dialogPanel.SetActive(active);
        for (int i = 0; i < _otherUI.Length; i++)
            _otherUI[i].SetActive(!active);
    }
    private void InitializeButtons()
    {
        foreach (var button in _buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void ShowNextDialog()
    {
        if (_currentDialogIndex >= _dialogDatas.Length || _currentDialogIndex < 0)
        {
            EndDialog();
            return;
        }
        _visualDialog.PlayAnimationForEnemy(_dialogDatas[_currentDialogIndex].DefeatEnemy,
                                               _dialogDatas[_currentDialogIndex].AnimationNumber);
        _waitingForChoice = false;
        _isTextComplete = false;
        HideButtons();

        _showText.StartShow(_dialogDatas[_currentDialogIndex].Text, _interval, () =>
        {
            OnTextComplete();

            if (_dialogDatas[_currentDialogIndex].Variants)
            {
                ShowVariants();
            }
            else if (_dialogDatas[_currentDialogIndex].NextDialogIndex >= 0)
            {
                StartAutoSkip();
            }
            else
            {
                StartCoroutine(EndDialogWithDelay());
            }
        });
    }

    private void OnTextComplete() => _isTextComplete = true;

    private void ShowVariants()
    {
        if (!_dialogDatas[_currentDialogIndex].Variants ||
            _dialogDatas[_currentDialogIndex].VariantTexts.Length != _buttons.Length ||
            _dialogDatas[_currentDialogIndex].VariantTargets.Length != _buttons.Length)
        {
            return;
        }

        _waitingForChoice = true;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i < _dialogDatas[_currentDialogIndex].VariantTexts.Length)
            {
                _buttons[i].gameObject.SetActive(true);
                _buttonsTexts[i].text = _dialogDatas[_currentDialogIndex].VariantTexts[i];

                int targetIndex = _dialogDatas[_currentDialogIndex].VariantTargets[i];
                _buttons[i].onClick.RemoveAllListeners();
                _buttons[i].onClick.AddListener(() => OnVariantSelected(targetIndex));
            }
        }
    }

    private void HideButtons()
    {
        foreach (var button in _buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void OnVariantSelected(int nextDialogIndex)
    {
        _currentDialogIndex = nextDialogIndex;
        HideButtons();
        ShowNextDialog();
    }

    private void StartAutoSkip()
    {
        if (_autoSkipCoroutine != null)
            StopCoroutine(_autoSkipCoroutine);
        _autoSkipCoroutine = StartCoroutine(AutoSkip());
    }

    IEnumerator AutoSkip()
    {
        yield return new WaitForSeconds(_autoSkipDelay);
        SkipToNext();
    }

    IEnumerator EndDialogWithDelay()
    {
        yield return new WaitForSeconds(_autoSkipDelay);
        EndDialog();
    }

    public void SkipToNext()
    {
        if (_waitingForChoice) return;

        if (!_isTextComplete)
        {
            _showText.ForceComplete();
            return;
        }
        if (_dialogDatas[_currentDialogIndex].NextDialogIndex >= 0)
        {
            _currentDialogIndex = _dialogDatas[_currentDialogIndex].NextDialogIndex;
            ShowNextDialog();
        }
        else
        {
            EndDialog();
        }
    }

    private void EndDialog()
    {
        if (_autoSkipCoroutine != null)
            StopCoroutine(_autoSkipCoroutine);
        OffOnUI(false);

        _endDialog = true;
        StopAllCoroutines();
        EventBus.Raise(new ChangeTime(true));

        if(_currentDialogIndex < 0) return;
        if (_dialogDatas[_currentDialogIndex].UseCrit)
            _clickButtonManager.ShowCrit();
        if (_dialogDatas[_currentDialogIndex].UsePosion)
            _clickButtonManager.PublicShowPosion();
        if (_dialogDatas[_currentDialogIndex].Reward != 0)
            _moneyCounter.ChangeTestMoney(_dialogDatas[_currentDialogIndex].Reward);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_waitingForChoice && !_endDialog)
        {
            SkipToNext();
        }
    }

    public UniqueId Id { get; } = new UniqueId();
    public void OnEvent(Unsubscribe @event)
    {
        EventBus.Unregister(this as IEventReceiver<EnemyDead>);
        EventBus.Unregister(this as IEventReceiver<Unsubscribe>);
    }
}