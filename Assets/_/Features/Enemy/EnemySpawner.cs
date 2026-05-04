using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxEnemies = 10;
    
    [Header("Spawn Area (Optional)")]
    [SerializeField] private bool _useRandomSpawnArea = false;
    [SerializeField] private Vector2 _spawnAreaMin = Vector2.zero;
    [SerializeField] private Vector2 _spawnAreaMax = Vector2.zero;
    
    [Header("Debug")]
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
            Debug.LogError("[EnemySpawner] Enemy Prefab est null !");
            return;
        }

        Vector3 spawnPosition = _useRandomSpawnArea ? GetRandomSpawnPosition() : transform.position;
        
        GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        _currentEnemyCount++;

        EntityHealth enemyHealth = enemy.GetComponent<EntityHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath.AddListener(OnEnemyDestroyed);
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] L'ennemi n'a pas de composant EntityHealth !");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
        float randomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
        return new Vector3(randomX, randomY, 0f);
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (_useRandomSpawnArea && _spawnAreaMax != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(
                (_spawnAreaMin.x + _spawnAreaMax.x) / 2,
                (_spawnAreaMin.y + _spawnAreaMax.y) / 2,
                0
            );
            Vector3 size = new Vector3(
                _spawnAreaMax.x - _spawnAreaMin.x,
                _spawnAreaMax.y - _spawnAreaMin.y,
                0
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}