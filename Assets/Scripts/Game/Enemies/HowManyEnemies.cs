using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HowManyEnemies : MonoBehaviour
{
    [SerializeField] private Image _enemyIndicatorPrefab;
    [SerializeField] private RectTransform _indicatorsContainer;
    [SerializeField] private Color _normalEnemyColor;
    [SerializeField] private Color _defeatedEnemyColor;
    [SerializeField] private Color _bossColor;
    [SerializeField] private Color _defeatedBossColor;
    [SerializeField] private float _margin = 20f; 

    private List<Image> _enemyIndicators = new List<Image>();

    public void Initialize(int totalEnemies, int bossIndex = -1)
    {
        if (totalEnemies <= 0) return;
        float totalWidth = (_enemyIndicatorPrefab.rectTransform.rect.width + _margin) * totalEnemies - _margin;
        float startX = -totalWidth / 2 + _enemyIndicatorPrefab.rectTransform.rect.width / 2;
        for (int i = 0; i < totalEnemies; i++)
        {
            CreateIndicator(i, startX, bossIndex);
            startX += _enemyIndicatorPrefab.rectTransform.rect.width + _margin;
        }
    }
    private void CreateIndicator(int index, float xPosition, int bossIndex)
    {
        var indicator = Instantiate(_enemyIndicatorPrefab, _indicatorsContainer);
        indicator.rectTransform.anchoredPosition = new Vector2(xPosition, 0);
        indicator.color = (index == bossIndex) ? _bossColor : _normalEnemyColor;
        _enemyIndicators.Add(indicator);
    }

    public void UpdateEnemyDefeated(int enemyIndex, bool isBoss)
    {
        if (enemyIndex >= 0 && enemyIndex < _enemyIndicators.Count)
        {
            _enemyIndicators[enemyIndex].color = isBoss ? _defeatedBossColor : _defeatedEnemyColor;
        }
    }
}