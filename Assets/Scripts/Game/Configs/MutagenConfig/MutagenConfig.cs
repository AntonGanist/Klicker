using System.Collections.Generic;
using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Configs/MutagenConfig", fileName = "MutagenConfig")]
public class MutagenConfig : ScriptableObject
{
    [Serializable]
    public struct MutagenViewData
    {
        public Sprite Sprite;
        public float Mass;
        public Vector3 Scale;
        public string Id;
    }
    public DropMutagen MutagenPrefab;
    public List<MutagenViewData> Mutagens;

    public int GetMutagenViewData(string id)
    {
        int numb = -1;
        for (int i = 0; i < Mutagens.Count; i++)
        {
            if (Mutagens[i].Id == id)
            {
                numb = i;
                break;
            }
        }
        return numb;
    }
}