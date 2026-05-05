using UnityEngine;

public class EnemyPoolMember : MonoBehaviour
{
    private EnemySpawner _ownerSpawner;
    private EntityHealth _health;
    private GameObject _sourcePrefab;

    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.ResetHealth();
    }

    public void SetOwner(EnemySpawner ownerSpawner)
    {
        _ownerSpawner = ownerSpawner;
    }

    public void SetSourcePrefab(GameObject sourcePrefab)
    {
        _sourcePrefab = sourcePrefab;
    }

    public void ReturnToPool()
    {
        if (_ownerSpawner != null)
        {
            _ownerSpawner.ReturnEnemyToPool(gameObject, _sourcePrefab);
            return;
        }

        gameObject.SetActive(false);
    }
}