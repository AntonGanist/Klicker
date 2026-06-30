using Game.HealthBar;
using TMPro;
using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private ViewDamage _viewDamagePrefab;
    private PoolMono<ViewDamage> _pool;
    private void Awake()
    {
        _pool = new PoolMono<ViewDamage>(_viewDamagePrefab, 10, transform);
        _pool.autoExpand = true;
    }
    public void SetName(string name)
    {
        _text.text = name;
    }
    public void CurrentPosition(Vector3 position)
    {
        position.z = 100;
        _slider.transform.localPosition = position;
    }
    public override void DecreaseValue(float value, Vector3 pos, AttackType attackType)
    {
        if (_slider.value == 0) return;
        base.DecreaseValue(value);
        string damageText;
        if (Mathf.Approximately(value, Mathf.Round(value)))
        {
            damageText = Mathf.RoundToInt(value).ToString();
        }
        else
        {
            damageText = value.ToString("F1");
        }

        var damage = _pool.GetFreeElement();
        damage.Initialize(pos, damageText, attackType);
    }
}