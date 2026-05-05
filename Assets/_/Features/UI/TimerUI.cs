
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private TMPro.TextMeshProUGUI _timerText;

    private void Start()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        if (GameManager.Instance == null || _timerText == null) return;

        var time = GameManager.Instance.Timer;
        _timerText.text = $"{time.Minutes:00}:{time.Seconds:00}";
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}
