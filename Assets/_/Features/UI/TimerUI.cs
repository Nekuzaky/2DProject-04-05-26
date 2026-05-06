using UnityEngine;

public class TimerUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private TMPro.TextMeshProUGUI _timerText;
    #endregion

    #region Lifecycle
    private void Start()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion

    #region UI Updates
    private void OnUpdateTick()
    {
        if (GameManager.Instance == null || _timerText == null) return;

        var time = GameManager.Instance.Timer;
        _timerText.text = $"{time.Minutes:00}:{time.Seconds:00}";
    }
    #endregion
}
