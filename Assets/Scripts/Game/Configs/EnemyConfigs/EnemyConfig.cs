using System;
using System.Collections.Generic;
using Game.Enemies;
using UnityEngine;

namespace Game.Configs.EnemyConfigs
{
    [CreateAssetMenu(menuName = "Configs/EnemiesConfig", fileName = "EnemiesConfig")]
    public class EnemiesConfig : ScriptableObject
    {
        [Serializable]
        public struct EnemyViewData
        {
            public int Id;
            public string Name;
            [Range(0f, 1f)] public float FrameAttack;
            public AttackType VulnerabilityType;
            public OilDamageType VulnerabilityToOil;
            public Transform Prefab;
            public ParticleSystemWrapper PrefabBlood;
            public float ColdTime;
            public float FireTime;
            [Range(0f, 1f)] public float FireDamage;
            public float AksiTime;
            public Vector3 Position;
            public Vector3 Scale;
            public Vector2 PositionHealthBar;
            public Vector3 ViewDamagePosition;
            public AudioClip AudioClip;
        }


        public Enemy EnemyPrefab;
        public List<EnemyViewData> Enemies;

        public EnemyViewData GetEnemy(int id)
        {
            foreach (var enemyData in Enemies)
            {
                if (enemyData.Id == id) return enemyData;
            }

            Debug.LogError($"Not found enemy with id {id}");
            return default;
        }
    }
}