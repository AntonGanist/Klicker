using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class ShowText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _uiText;
    private Action _onComplete;
    private string _text;
    public void StartShow(string text, float interval, Action onComplete)
    {
        _text = text;
        _onComplete = onComplete;
        StartCoroutine(Show(text, interval));
    }

    public void ForceComplete()
    {
        _uiText.text = "";
        _uiText.text = _text;
        _text = null;
        StopAllCoroutines();
        if (_onComplete != null)
            _onComplete();
    }

    IEnumerator Show(string text, float interval)
    {
        _uiText.text = "";
        foreach (char c in text)
        {
            _uiText.text += c;
            yield return new WaitForSeconds(interval);
        }

        if (_onComplete != null)
            _onComplete();
    }
}