using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("<color=yellow><b><size=15>Prefabs</size></b></color>")]
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _shooterPrefab;

    [Header("<color=cyan><b><size=15>Spawn Settings</size></b></color>")]
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    [SerializeField] private float _spawnDistanceMin = 5f;
    [SerializeField] private float _spawnDistanceMax = 10f;

    private float _spawnTimer;
    private int _currentEnemyCount;
    private Transform _playerTransform;

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
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        _currentEnemyCount++;

        if (enemy.TryGetComponent(out EntityHealth health))
            health.OnDeath.AddListener(OnEnemyDestroyed);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : Vector3.zero;
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dist = Random.Range(_spawnDistanceMin, _spawnDistanceMax);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
    }

    public void OnEnemyDestroyed()
    {
        _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
        EnemyManager.Instance?.AddKill();
    }

    public int GetCurrentEnemyCount() => _currentEnemyCount;
    public int GetMaxEnemies() => _maxEnemies;

    private void OnDrawGizmosSelected()
    {
        Vector3 center = _playerTransform != null ? _playerTransform.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, _spawnDistanceMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _spawnDistanceMax);
    }
}
