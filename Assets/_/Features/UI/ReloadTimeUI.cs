using UnityEngine;
using TMPro;

public class ReloadTimeUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private PlayerShooter _playerShooter;
    [SerializeField] private GameObject _ammoPanel;
    [SerializeField] private TMP_Text _reloadText;
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
            GameLogger.LogError("<color=red><b>ReloadTimeUI:</b></color> PlayerShooter not found!");
            return;
        }

        _playerShooter.OnReloadStart    += HandleReloadStart;
        _playerShooter.OnReloadFinished += HandleReloadFinished;

        if (_ammoPanel   != null) _ammoPanel.SetActive(true);
        if (_reloadText != null) _reloadText.gameObject.SetActive(false);
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

        if (_reloadText == null)
            _reloadText = GameObject.Find("ReloadTimeUI")?.GetComponent<TMP_Text>();
    }
    #endregion

    #region Reload Handling
    private void HandleReloadStart(float duration)
    {
        _isReloading = true;

        if (_ammoPanel   != null) _ammoPanel.SetActive(false);
        if (_reloadText != null) _reloadText.gameObject.SetActive(true);
        if (_reloadText != null) _reloadText.text = "Reload... 0%";

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void HandleReloadFinished()
    {
        _isReloading = false;

        if (_ammoPanel   != null) _ammoPanel.SetActive(true);
        if (_reloadText != null) _reloadText.gameObject.SetActive(false);

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        if (!_isReloading || _playerShooter == null || _reloadText == null) return;

        int progressPercent = Mathf.RoundToInt(_playerShooter.ReloadProgress * 100f);
        _reloadText.text = $"Reload.. {progressPercent}%";
    }
    #endregion
}
