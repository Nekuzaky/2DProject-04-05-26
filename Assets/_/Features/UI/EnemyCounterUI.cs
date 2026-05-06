using UnityEngine;

public class EnemyCounterUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private TMPro.TextMeshProUGUI _counterText;
    #endregion

    #region Lifecycle
    private void Start()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnKillCountChanged += UpdateKillCount;
            UpdateKillCount(EnemyManager.Instance.KillCount);
        }
    }

    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.OnKillCountChanged -= UpdateKillCount;
    }
    #endregion

    #region UI Updates
    private void UpdateKillCount(int count)
    {
        if (_counterText != null)
            _counterText.text = $"{count}";
    }
    #endregion
}
