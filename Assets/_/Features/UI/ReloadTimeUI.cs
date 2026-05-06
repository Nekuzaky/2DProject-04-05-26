using UnityEngine;
using UnityEngine.UI;

public class ReloadTimeUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private PlayerShooter _playerShooter;
    [SerializeField] private GameObject _ammoPanel;
    [SerializeField] private GameObject _reloadPanel;
    [SerializeField] private Image _reloadFillImage;
    #endregion

    #region State
    private bool _isReloading = false;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        ResolveReferences();

        if (_playerShooter == null)
            _playerShooter = FindAnyObjectByType<PlayerShooter>();

        if (_playerShooter == null)
        {
            Debug.LogError("<color=red><b>ReloadTimeUI:</b></color> PlayerShooter not found!");
            return;
        }

        if (_reloadPanel == gameObject)
            Debug.LogWarning("<color=yellow><b>ReloadTimeUI:</b></color> Put this script on an always-active HUD object, not on the Reload UI object itself.");

        _playerShooter.OnReloadStart    += HandleReloadStart;
        _playerShooter.OnReloadFinished += HandleReloadFinished;

        if (_reloadPanel != null) _reloadPanel.SetActive(false);
        if (_ammoPanel   != null) _ammoPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_playerShooter != null)
        {
            _playerShooter.OnReloadStart    -= HandleReloadStart;
            _playerShooter.OnReloadFinished -= HandleReloadFinished;
        }

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion

    #region References Resolution
    private void ResolveReferences()
    {
        if (_ammoPanel == null)
            _ammoPanel = GameObject.Find("AmmoUI");

        if (_reloadPanel == null)
            _reloadPanel = GameObject.Find("ReloadTimeUI");

        if (_reloadFillImage == null && _reloadPanel != null)
            _reloadFillImage = _reloadPanel.GetComponentInChildren<Image>(true);
    }
    #endregion

    #region Reload Handling
    private void HandleReloadStart(float duration)
    {
        _isReloading = true;

        if (_ammoPanel   != null) _ammoPanel.SetActive(false);
        if (_reloadPanel != null) _reloadPanel.SetActive(true);
        if (_reloadFillImage != null) _reloadFillImage.fillAmount = 0f;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void HandleReloadFinished()
    {
        _isReloading = false;

        if (_reloadPanel != null) _reloadPanel.SetActive(false);
        if (_ammoPanel   != null) _ammoPanel.SetActive(true);
        if (_reloadFillImage != null) _reloadFillImage.fillAmount = 1f;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        if (!_isReloading || _playerShooter == null || _reloadFillImage == null) return;
        _reloadFillImage.fillAmount = _playerShooter.ReloadProgress;
    }
    #endregion
}
