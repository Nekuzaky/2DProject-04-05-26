using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("<color=yellow><b><size=15>References</size></b></color>")]
    [SerializeField] private PlayerShooter _playerShooter;
    [SerializeField] private TMP_Text _ammoText;

    private void OnEnable()
    {
        if (_playerShooter != null)
        {
            _playerShooter.OnAmmoChanged.AddListener(UpdateAmmoDisplay);
        }
    }

    private void OnDisable()
    {
        if (_playerShooter != null)
        {
            _playerShooter.OnAmmoChanged.RemoveListener(UpdateAmmoDisplay);
        }
    }

    private void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
}