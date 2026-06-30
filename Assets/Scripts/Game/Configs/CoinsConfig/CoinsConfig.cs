using System.Collections.Generic;
using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Configs/CoinsConfig", fileName = "CoinsConfig")]
public class CoinsConfig : ScriptableObject
{
    [Serializable]
    public struct MoneysViewData
    {
        public Sprite Sprite;
        public float Mass;
        public Vector3 Scale;
        public int Price;
    }
    public MoneyDrop MoneyPrefab;
    public List<MoneysViewData> Moneys;

    public int GetMoneysViewData(int price)
    {
        int numb = -1;
        for (int i = 0; i < Moneys.Count; i++)
        {
            if (Moneys[i].Price == price)
            {
                numb = i;
                break;
            }
        }
        return numb;
    }
}
