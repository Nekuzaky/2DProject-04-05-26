using UnityEngine;

[CreateAssetMenu(fileName = "Magazine", menuName = "Gun Parts/Magazine")]
public class GunPartMagazine : ScriptableObject
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Magazine Stats</size></b></color>")]
    public int MagazineSizeBonus = 0;
    public float ReloadSpeedBonus = 0f;
    #endregion
}
