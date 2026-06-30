using UnityEngine;

[CreateAssetMenu(menuName = "Configs/PosionConfig", fileName = "PosionConfig")]
public class PosionConfig : ScriptableObject
{
    public Sprite HealthPosion;
    public Sprite ManaPosion;
    [Range(0f, 1f)] public float HealthPosionPower;
    [Range(0f, 1f)] public float ManaPosionPower;
}
