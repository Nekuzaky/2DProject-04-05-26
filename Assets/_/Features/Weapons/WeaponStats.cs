using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    #region Inspector Settings - Base Stats
    [Header("<color=cyan><b><size=15>Base Weapon Stats</size></b></color>")]
    public int BaseDamage = 10;
    public float BaseFireRate = 0.5f;
    public int BaseAmmoCapacity = 30;
    public float BaseReloadTime = 1.5f;
    public float BaseSpreadAngle = 5f;
    public float ProjectileSpeed = 20f;
    public float Range = 15f;
    #endregion

    #region Inspector Settings - Gun Parts
    [Header("<color=purple><b><size=15>Equipped Gun Parts</size></b></color>")]
    public GunPartTrigger Trigger;
    public GunPartMagazine Magazine;
    public GunPartGrip Grip;
    #endregion

    #region Properties
    public int FinalDamage => BaseDamage + (Trigger != null ? Trigger.DamageBonus : 0);
    public float FinalFireRate => Mathf.Max(0.05f, BaseFireRate - (Trigger != null ? Trigger.FireRateBonus : 0f));
    public int FinalAmmoCapacity => BaseAmmoCapacity + (Magazine != null ? Magazine.MagazineSizeBonus : 0);
    public float FinalReloadTime => Mathf.Max(0.1f, BaseReloadTime - (Magazine != null ? Magazine.ReloadSpeedBonus : 0f));
    public float FinalSpreadAngle => Mathf.Max(0f, BaseSpreadAngle - (Grip != null ? Grip.AccuracyBonus : 0f));
    #endregion
}
