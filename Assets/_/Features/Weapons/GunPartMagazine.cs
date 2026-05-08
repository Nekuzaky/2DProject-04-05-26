using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Magazine", menuName = "Gun Parts/Magazine")]
public class GunPartMagazine : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Magazine Stats</size></b></color>")]
    [FormerlySerializedAs("MagazineSizeBonus")]
    public int AmmoCapacity = 30; // Number of bullets the magazine can hold. Higher values mean more ammo before needing to reload.

    [FormerlySerializedAs("ReloadSpeedBonus")]
    public float ReloadTime = 1.5f; // Time in seconds to reload. Lower values mean faster reloads.
    #endregion
}
