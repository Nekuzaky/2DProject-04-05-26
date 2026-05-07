using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Trigger", menuName = "Gun Parts/Trigger")]
public class GunPartTrigger : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Trigger Stats</size></b></color>")]
    [FormerlySerializedAs("DamageBonus")]
    public int Damage = 10;

    [FormerlySerializedAs("FireRateBonus")]
    public float FireRate = 0.5f;
    #endregion
}
