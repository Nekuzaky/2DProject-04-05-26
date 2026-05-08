using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Trigger", menuName = "Gun Parts/Trigger")]
public class GunPartTrigger : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Trigger Stats</size></b></color>")]
    [FormerlySerializedAs("DamageBonus")]
    public int Damage = 10; // Base damage per shot. Higher values mean more damage.

    [FormerlySerializedAs("FireRateBonus")]
    public float FireRate = 0.5f; // Time in seconds between shots. Lower values mean faster firing.
    #endregion
}
