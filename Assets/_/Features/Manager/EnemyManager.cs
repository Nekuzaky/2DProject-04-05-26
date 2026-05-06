using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("<color=yellow><b><size=15>Prefabs</size></b></color>")]
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _shooterPrefab;

    [Header("<color=cyan><b><size=15>Spawn Settings</size></b></color>")]
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    [SerializeField] private int _poolWarmupPerPrefab = 5;
    [SerializeField] private float _spawnDistanceMin = 5f;
    [SerializeField] private float _spawnDistanceMax = 10f;

    public event System.Action<int> OnEnemyCountChanged;
    public event System.Action<int> OnKillCountChanged;

    public int KillCount => _killCount;
    public int CurrentEnemyCount => _currentEnemyCount;
    public int MaxEnemies => _maxEnemies;
    public float BaseSpawnInterval => _baseSpawnInterval;

    private int _killCount;
    private int _currentEnemyCount;
    private int _baseMaxEnemies;
    private float _baseSpawnInterval;
    private float _spawnTimer;
    private bool _spawningEnabled = true;
    private Transform _playerTransform;
    private readonly Dictionary<GameObject, Queue<GameObject>> _enemyPools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _baseMaxEnemies    = _maxEnemies;
        _baseSpawnInterval = _spawnInterval;
        InitializePool(_meleePrefab);
        InitializePool(_shooterPrefab);
    }

    private void Start()
    {
        _playerTransform = FindAnyObjectByType<PlayerController>()?.transform;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        if (!_spawningEnabled || _currentEnemyCount >= _maxEnemies) return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _spawnInterval)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefab = Random.value < 0.5f ? _meleePrefab : _shooterPrefab;
        if (prefab == null) return;

        GameObject enemy = GetEnemyFromPool(prefab);
        if (enemy == null) return;

        enemy.transform.SetPositionAndRotation(GetSpawnPosition(), Quaternion.identity);
        enemy.SetActive(true);
        _currentEnemyCount++;
        OnEnemyCountChanged?.Invoke(_currentEnemyCount);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : Vector3.zero;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist  = Random.Range(_spawnDistanceMin, _spawnDistanceMax);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
    }

    public void ReturnEnemyToPool(GameObject enemy, GameObject sourcePrefab)
    {
        if (enemy == null) return;

        _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
        OnEnemyCountChanged?.Invoke(_currentEnemyCount);
        AddKill();

        enemy.SetActive(false);

        if (sourcePrefab == null)
        {
            Destroy(enemy);
            return;
        }

        if (!_enemyPools.ContainsKey(sourcePrefab))
            _enemyPools[sourcePrefab] = new Queue<GameObject>();

        _enemyPools[sourcePrefab].Enqueue(enemy);
    }

    private void AddKill()
    {
        _killCount++;
        OnKillCountChanged?.Invoke(_killCount);
    }

    public void StartSpawning()  => SetSpawningEnabled(true);
    public void StopSpawning()   => SetSpawningEnabled(false);

    public void SetSpawningEnabled(bool isEnabled)
    {
        _spawningEnabled = isEnabled;
    }

    public void SetDifficulty(int additionalMaxEnemies, float spawnInterval)
    {
        _maxEnemies    = _baseMaxEnemies + additionalMaxEnemies;
        _spawnInterval = spawnInterval;
    }

    public void SetBaseMaxEnemies(int baseMaxEnemies)
    {
        _baseMaxEnemies = Mathf.Max(1, baseMaxEnemies);
        _maxEnemies     = _baseMaxEnemies;
    }

    private void InitializePool(GameObject prefab)
    {
        if (prefab == null) return;
        if (_enemyPools.ContainsKey(prefab)) return;

        Queue<GameObject> pool = new Queue<GameObject>();
        _enemyPools[prefab] = pool;

        for (int i = 0; i < _poolWarmupPerPrefab; i++)
        {
            GameObject enemy = CreateEnemyInstance(prefab);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    private GameObject GetEnemyFromPool(GameObject prefab)
    {
        if (!_enemyPools.ContainsKey(prefab))
            InitializePool(prefab);

        Queue<GameObject> pool = _enemyPools[prefab];
        return pool.Count > 0 ? pool.Dequeue() : CreateEnemyInstance(prefab);
    }

    private GameObject CreateEnemyInstance(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);

        EnemyPoolMember poolMember = enemy.GetComponent<EnemyPoolMember>();
        if (poolMember == null)
            poolMember = enemy.AddComponent<EnemyPoolMember>();

        poolMember.SetSourcePrefab(prefab);
        return enemy;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, _spawnDistanceMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _spawnDistanceMax);
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}
