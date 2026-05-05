using UnityEngine;

[CreateAssetMenu(fileName = "Trigger", menuName = "Gun Parts/Trigger")]

public class GunPartTrigger : ScriptableObject
{
    [Header("<color=cyan><b><size=15>Trigger Stats</size></b></color>")]
    public int _damageBonus = 0;          // Added to base damage
    public float _fireRateBonus = 0f;     // Subtracted from base fire rate (lower = faster)
}
