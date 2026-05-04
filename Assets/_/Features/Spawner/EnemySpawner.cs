using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("<color=yellow><b><size=15>Spawn Settings</size></b></color>")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    
    [Header("<color=cyan><b><size=15>Spawn Area</size></b></color>")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _spawnDistanceMin = 5f;
    [SerializeField] private float _spawnDistanceMax = 10f;
    
    [Header("<color=green><b><size=15>Debug</size></b></color>")]
    [SerializeField] private bool _showGizmos = true;

    private float _spawnTimer;
    private int _currentEnemyCount;

    private void Update()
    {
        if (_currentEnemyCount >= _maxEnemies)
            return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (_enemyPrefab == null)
        {
           
            return;
        }

        Vector3 spawnPosition = GetCircularSpawnPosition();
        
        GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        _currentEnemyCount++;

        EntityHealth enemyHealth = enemy.GetComponent<EntityHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath.AddListener(OnEnemyDestroyed);
        }
        else
        {
          
        }
    }

    private Vector3 GetCircularSpawnPosition()
    {
        if (_playerTransform == null)
        {
            _playerTransform = FindAnyObjectByType<PlayerController>()?.transform;
        }

        Vector3 playerPosition = _playerTransform != null ? _playerTransform.position : Vector3.zero;
        
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(_spawnDistanceMin, _spawnDistanceMax);
        
        float spawnX = playerPosition.x + Mathf.Cos(randomAngle) * randomDistance;
        float spawnY = playerPosition.y + Mathf.Sin(randomAngle) * randomDistance;
        
        return new Vector3(spawnX, spawnY, 0f);
    }

    public void OnEnemyDestroyed()
    {
        _currentEnemyCount--;
        
        if (_currentEnemyCount < 0)
            _currentEnemyCount = 0;
    }

    public int GetCurrentEnemyCount() => _currentEnemyCount;
    public int GetMaxEnemies() => _maxEnemies;

    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        Transform player = _playerTransform;
        if (player == null && Application.isPlaying)
        {
            player = FindAnyObjectByType<PlayerController>()?.transform;
        }

        Vector3 center = player != null ? player.position : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, _spawnDistanceMin);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _spawnDistanceMax);
    }
}