using UnityEngine;

public class EnemyCounterUI : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private TMPro.TextMeshProUGUI _counterText;

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

    private void UpdateKillCount(int count)
    {
        if (_counterText != null)
            _counterText.text = $"{count}";
    }
}
