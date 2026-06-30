using Meta.Shop;
using UnityEngine;
using YG;

public class Advertisement : MonoBehaviour
{
    [SerializeField] private ShopWindow _shopWindow;
    private int _minCost;
    private void Start()
    {
        _minCost = _shopWindow.MinCost();
        Debug.Log(_minCost);
    }
}
