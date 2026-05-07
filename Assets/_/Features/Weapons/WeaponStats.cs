using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    #region Inspector Settings - Gun Parts
    [Header("<color=purple><b><size=15>Equipped Gun Parts</size></b></color>")]
    public GunPartTrigger  Trigger;
    public GunPartMagazine Magazine;
    public GunPartGrip     Grip;

    [Header("<color=cyan><b><size=15>Shared Weapon Settings</size></b></color>")]
    public float ProjectileSpeed = 20f;
    public float Range = 15f;
    #endregion

    #region Final Stats
    public int   FinalDamage       => Trigger  != null ? Trigger.Damage : 10;
    public float FinalFireRate     => Mathf.Max(0.05f, Trigger  != null ? Trigger.FireRate : 0.5f);
    public int   FinalAmmoCapacity => Magazine != null ? Magazine.AmmoCapacity : 30;
    public float FinalReloadTime   => Mathf.Max(0.1f, Magazine != null ? Magazine.ReloadTime : 1.5f);
    public float FinalSpreadAngle  => Mathf.Max(0f, Grip != null ? Grip.SpreadAngle : 5f);
    #endregion
}
