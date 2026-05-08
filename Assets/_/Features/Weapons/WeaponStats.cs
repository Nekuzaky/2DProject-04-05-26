using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    #region Inspector Settings - Gun Parts
    [Header("<color=purple><b><size=15>Equipped Gun Parts</size></b></color>")]
    public GunPartTrigger  Trigger;
    public GunPartMagazine Magazine;
    public GunPartGrip     Grip;

    [Header("<color=cyan><b><size=15>Shared Weapon Settings</size></b></color>")]
    public float ProjectileSpeed = 20f; // Speed at which the projectile travels. Higher values mean faster projectiles.
    public float Range = 15f; 
    #endregion

    #region Final Stats
    public int   FinalDamage       => Trigger  != null ? Trigger.Damage : 10; // Base damage is 10 if no trigger is equipped
    public float FinalFireRate     => Mathf.Max(0.05f, Trigger  != null ? Trigger.FireRate : 0.5f); // Base fire rate is 0.5s if no trigger is equipped, with a minimum of 0.05s to prevent zero or negative fire rates
    public int   FinalAmmoCapacity => Magazine != null ? Magazine.AmmoCapacity : 30; // Base ammo capacity is 30 if no magazine is equipped
    public float FinalReloadTime   => Mathf.Max(0.1f, Magazine != null ? Magazine.ReloadTime : 1.5f); // Base reload time is 1.5s if no magazine is equipped, with a minimum of 0.1s to prevent zero or negative reload times
    public float FinalSpreadAngle  => Mathf.Max(0f, Grip != null ? Grip.SpreadAngle : 5f); // Base spread angle is 5 degrees if no grip is equipped, with a minimum of 0 degrees to prevent negative spread
    #endregion
}
