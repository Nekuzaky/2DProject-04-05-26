using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("<color=yellow><b><size=15>Spawners</size></b></color>")]
    [SerializeField] private EnemySpawner[] _spawners;

    public event System.Action<int> OnEnemyCountChanged;
    public event System.Action<int> OnKillCountChanged;

    private int _totalEnemyCount;
    public int TotalEnemyCount => _totalEnemyCount;

    private int _killCount;
    public int KillCount => _killCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (_spawners == null || _spawners.Length == 0)
            _spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        int count = 0;
        foreach (EnemySpawner spawner in _spawners)
            if (spawner != null) count += spawner.GetCurrentEnemyCount();

        if (count != _totalEnemyCount)
        {
            _totalEnemyCount = count;
            OnEnemyCountChanged?.Invoke(_totalEnemyCount);
        }
    }

    public void StartSpawning()
    {
        foreach (EnemySpawner spawner in _spawners)
            if (spawner != null) spawner.enabled = true;
    }

    public void StopSpawning()
    {
        foreach (EnemySpawner spawner in _spawners)
            if (spawner != null) spawner.enabled = false;
    }

    public void AddKill()
    {
        _killCount++;
        OnKillCountChanged?.Invoke(_killCount);
    }
}
