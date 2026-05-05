using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("<color=yellow><b><size=15>Prefabs</size></b></color>")]
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _shooterPrefab;

    [Header("<color=cyan><b><size=15>Spawn Settings</size></b></color>")]
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    [SerializeField] private int _poolWarmupPerPrefab = 5;

    public float BaseSpawnInterval => _baseSpawnInterval;
    [SerializeField] private float _spawnDistanceMin = 5f;
    [SerializeField] private float _spawnDistanceMax = 10f;

    private float _spawnTimer;
    private int _currentEnemyCount;
    private Transform _playerTransform;
    private int _baseMaxEnemies;
    private float _baseSpawnInterval;
    private readonly Dictionary<GameObject, Queue<GameObject>> _enemyPools = new();

    private void Awake()
    {
        _baseMaxEnemies = _maxEnemies;
        _baseSpawnInterval = _spawnInterval;
        InitializePool(_meleePrefab);
        InitializePool(_shooterPrefab);
    }

    private void Start()
    {
        _playerTransform = FindAnyObjectByType<PlayerController>()?.transform;
    }

    private void Update()
    {
        if (_currentEnemyCount >= _maxEnemies) return;

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

        Vector3 spawnPos = GetSpawnPosition();
        GameObject enemy = GetEnemyFromPool(prefab);
        if (enemy == null) return;

        enemy.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
        enemy.SetActive(true);
        _currentEnemyCount++;
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : Vector3.zero;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = Random.Range(_spawnDistanceMin, _spawnDistanceMax);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
    }

    public int GetCurrentEnemyCount() => _currentEnemyCount;
    public int GetMaxEnemies() => _maxEnemies;

    public void ReturnEnemyToPool(GameObject enemy, GameObject sourcePrefab)
    {
        if (enemy == null) return;

        _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
        EnemyManager.Instance?.AddKill();

        enemy.SetActive(false);

        EnemyPoolMember poolMember = enemy.GetComponent<EnemyPoolMember>();
        if (poolMember == null)
        {
            Destroy(enemy);
            return;
        }

        GameObject prefab = sourcePrefab;
        if (prefab == null)
        {
            Destroy(enemy);
            return;
        }

        if (!_enemyPools.ContainsKey(prefab))
            _enemyPools[prefab] = new Queue<GameObject>();

        _enemyPools[prefab].Enqueue(enemy);
    }

    public void SetDifficulty(int additionalMaxEnemies, float spawnInterval)
    {
        _maxEnemies = _baseMaxEnemies + additionalMaxEnemies;
        _spawnInterval = spawnInterval;
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
        if (pool.Count > 0)
            return pool.Dequeue();

        return CreateEnemyInstance(prefab);
    }

    private GameObject CreateEnemyInstance(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);

        EnemyPoolMember poolMember = enemy.GetComponent<EnemyPoolMember>();
        if (poolMember == null)
            poolMember = enemy.AddComponent<EnemyPoolMember>();

        poolMember.SetOwner(this);
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
}
