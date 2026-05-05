using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "Trigger", menuName = "Gun Parts/Trigger")]
public class GunPartTrigger : ScriptableObject
{
    [Header("<color=cyan><b><size=15>Trigger Stats</size></b></color>")]
    public int DamageBonus = 0;
    public float FireRateBonus = 0f;
}
