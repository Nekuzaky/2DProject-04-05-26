using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    #region Singleton
    public static EnemyManager Instance { get; private set; }
    #endregion

    #region Inspector Settings - Prefabs
    [Header("<color=yellow><b><size=15>Prefabs</size></b></color>")]
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _shooterPrefab;
    #endregion

    #region Inspector Settings - Spawn
    [Header("<color=cyan><b><size=15>Spawn Settings</size></b></color>")]
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    [SerializeField] private int _poolWarmupPerPrefab = 5;
    [SerializeField] private float _spawnDistanceMin = 5f;
    [SerializeField] private float _spawnDistanceMax = 10f;
    #endregion

    #region Inspector Settings - Target
    [Header("<color=green><b><size=15>Target</size></b></color>")]
    [SerializeField] private Transform _playerTargetOverride;
    #endregion

    #region Events
    public event System.Action<int> OnEnemyCountChanged;
    public event System.Action<int> OnKillCountChanged;
    #endregion

    #region Properties
    public int KillCount => _killCount;
    public int CurrentEnemyCount => _currentEnemyCount;
    public int MaxEnemies => _maxEnemies;
    public float BaseSpawnInterval => _baseSpawnInterval;
    public Transform PlayerTarget { get; private set; }
    public EntityHealth PlayerHealth { get; private set; }
    #endregion

    #region State
    private int _killCount;
    private int _currentEnemyCount;
    private int _baseMaxEnemies;
    private float _baseSpawnInterval;
    private float _spawnTimer;
    private bool _spawningEnabled = true;
    private Transform _playerTransform;
    private readonly Dictionary<GameObject, Queue<GameObject>> _enemyPoolDict = new();
    #endregion

    #region Lifecycle
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
        if (_playerTargetOverride != null)
        {
            PlayerTarget     = _playerTargetOverride;
            PlayerHealth     = _playerTargetOverride.GetComponent<EntityHealth>();
            _playerTransform = PlayerTarget;
        }
        else
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerTarget     = player.transform;
                PlayerHealth     = player.GetComponent<EntityHealth>();
                _playerTransform = PlayerTarget;
            }
        }

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }
    #endregion

    #region Spawning Logic
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
    #endregion

    #region Pool Management
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

        if (!_enemyPoolDict.ContainsKey(sourcePrefab))
            _enemyPoolDict[sourcePrefab] = new Queue<GameObject>();

        _enemyPoolDict[sourcePrefab].Enqueue(enemy);
    }

    private void AddKill()
    {
        _killCount++;
        OnKillCountChanged?.Invoke(_killCount);
    }

    private void InitializePool(GameObject prefab)
    {
        if (prefab == null) return;
        if (_enemyPoolDict.ContainsKey(prefab)) return;

        Queue<GameObject> pool = new Queue<GameObject>();
        _enemyPoolDict[prefab] = pool;

        for (int i = 0; i < _poolWarmupPerPrefab; i++)
        {
            GameObject enemy = CreateEnemyInstance(prefab);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    private GameObject GetEnemyFromPool(GameObject prefab)
    {
        if (!_enemyPoolDict.ContainsKey(prefab))
            InitializePool(prefab);

        Queue<GameObject> pool = _enemyPoolDict[prefab];
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
    #endregion

    #region Public API
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
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, _spawnDistanceMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _spawnDistanceMax);
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion
}
