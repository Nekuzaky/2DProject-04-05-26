using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Base Weapon Stats</size></b></color>")]
    public int _baseDamage = 10;
    public float _baseFireRate = 0.5f;
    public int _baseAmmoCapacity = 30;
    public float _baseReloadTime = 1.5f;
    public float _baseSpreadAngle = 5f;
    public float _projectileSpeed = 20f;
    public float _range = 15f;

    [Header("<color=purple><b><size=15>Equipped Gun Parts</size></b></color>")]
    public GunPartTrigger _trigger;
    public GunPartMagazine _magazine;
    public GunPartGrip _grip;

    public int FinalDamage => _baseDamage + (_trigger != null ? _trigger._damageBonus : 0);
    public float FinalFireRate => Mathf.Max(0.05f, _baseFireRate - (_trigger != null ? _trigger._fireRateBonus : 0f));
    public int FinalAmmoCapacity => _baseAmmoCapacity + (_magazine != null ? _magazine._magazineSizeBonus : 0);
    public float FinalReloadTime => Mathf.Max(0.1f, _baseReloadTime - (_magazine != null ? _magazine._reloadSpeedBonus : 0f));
    public float FinalSpreadAngle => Mathf.Max(0f, _baseSpreadAngle - (_grip != null ? _grip._accuracyBonus : 0f));
}
