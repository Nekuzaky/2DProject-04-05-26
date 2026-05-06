using UnityEngine;

[CreateAssetMenu(fileName = "Grip", menuName = "Gun Parts/Grip")]
public class GunPartGrip : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Grip Stats</size></b></color>")]
    public float AccuracyBonus = 0f;
    #endregion
}
