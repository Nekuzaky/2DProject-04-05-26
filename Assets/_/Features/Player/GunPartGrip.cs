using UnityEngine;



[CreateAssetMenu(fileName = "Grip", menuName = "Gun Parts/Grip")]
public class GunPartGrip : ScriptableObject
{
    [Header("<color=cyan><b><size=15>Grip Stats</size></b></color>")]
    public float _accuracyBonus = 0f;
}
