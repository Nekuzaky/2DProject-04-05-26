using UnityEngine;

[CreateAssetMenu(fileName = "Magazine", menuName = "Gun Parts/Magazine")]
public class GunPartMagazine : ScriptableObject
{
    [Header("<color=cyan><b><size=15>Magazine Stats</size></b></color>")]
    public int _magazineSizeBonus = 0;    // Added to base ammo capacity
    public float _reloadSpeedBonus = 0f;  // Subtracted from base reload time (lower = faster)
}
