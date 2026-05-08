using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Grip", menuName = "Gun Parts/Grip")]
public class GunPartGrip : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Grip Stats</size></b></color>")]
    [FormerlySerializedAs("AccuracyBonus")]
    public float SpreadAngle = 5f; // Base spread angle in degrees. Higher values mean more spread (less accuracy).
    #endregion
}
