using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("<color=yellow><b><size=15>Spawners</size></b></color>")]
    [SerializeField] private EnemySpawner[] _spawners;
    [SerializeField] private bool _spawningEnabled = true;
    [SerializeField] private bool[] _spawnerEnabled;

    public event System.Action<int> OnEnemyCountChanged;
    public event System.Action<int> OnKillCountChanged;

    private int _totalEnemyCount;
    public int TotalEnemyCount => _totalEnemyCount;

    private int _killCount;
    public int KillCount => _killCount;
    public bool SpawningEnabled => _spawningEnabled;

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

        EnsureSpawnerStateArray();
        ApplySpawnState();
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
        SetSpawningEnabled(true);
    }

    public void StopSpawning()
    {
        SetSpawningEnabled(false);
    }

    public void SetSpawningEnabled(bool isEnabled)
    {
        _spawningEnabled = isEnabled;
        ApplySpawnState();
    }

    public void SetSpawnerEnabled(int spawnerIndex, bool isEnabled)
    {
        EnsureSpawnerStateArray();
        if (spawnerIndex < 0 || spawnerIndex >= _spawnerEnabled
    .Length) return;

        _spawnerEnabled
    [spawnerIndex] = isEnabled;
        ApplySpawnState();
    }

    public bool IsSpawnerEnabled(int spawnerIndex)
    {
        EnsureSpawnerStateArray();
        if (spawnerIndex < 0 || spawnerIndex >= _spawnerEnabled
    .Length) return false;
        return _spawnerEnabled
    [spawnerIndex];
    }

    public void SetSpawnerEnabled(EnemySpawner spawner, bool isEnabled)
    {
        if (spawner == null || _spawners == null) return;

        for (int i = 0; i < _spawners.Length; i++)
        {
            if (_spawners[i] != spawner) continue;

            SetSpawnerEnabled(i, isEnabled);
            return;
        }
    }

    public void AddKill()
    {
        _killCount++;
        OnKillCountChanged?.Invoke(_killCount);
    }

    public float BaseSpawnInterval
    {
        get
        {
            if (_spawners != null && _spawners.Length > 0 && _spawners[0] != null)
                return _spawners[0].BaseSpawnInterval;
            return 5f;
        }
    }

    public void SetDifficulty(int additionalMaxEnemies, float spawnInterval)
    {
        foreach (EnemySpawner spawner in _spawners)
            if (spawner != null) spawner.SetDifficulty(additionalMaxEnemies, spawnInterval);
    }

    private void ApplySpawnState()
    {
        if (_spawners == null) return;
        EnsureSpawnerStateArray();

        for (int i = 0; i < _spawners.Length; i++)
        {
            EnemySpawner spawner = _spawners[i];
            if (spawner == null) continue;

            spawner.enabled = _spawningEnabled && _spawnerEnabled
        [i];
        }
    }

    private void EnsureSpawnerStateArray()
    {
        int length = _spawners != null ? _spawners.Length : 0;

        if (_spawnerEnabled
     != null && _spawnerEnabled
    .Length == length)
            return;

        bool[] previous = _spawnerEnabled
    ;
        _spawnerEnabled
     = new bool[length];

        for (int i = 0; i < length; i++)
        {
            bool defaultValue = true;
            if (previous != null && i < previous.Length)
                defaultValue = previous[i];

            _spawnerEnabled
        [i] = defaultValue;
        }
    }

    private void OnValidate()
    {
        EnsureSpawnerStateArray();

        if (Application.isPlaying)
            ApplySpawnState();
    }
}
