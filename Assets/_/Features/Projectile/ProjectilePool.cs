using UnityEngine;
using System.Collections.Generic;

// Centralized projectile pool — same pattern as EnemyManager's pool.
// Attach to any persistent scene object (e.g., the GameManager GameObject).
// EnemyShooterAI calls Get() instead of Instantiate(); Projectile calls Return() instead of Destroy().
public class ProjectilePool : MonoBehaviour
{
    #region Singleton
    public static ProjectilePool Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Pool Settings</size></b></color>")]
    [Tooltip("Pre-instantiated projectiles per prefab type at startup.")]
    [SerializeField] private int _warmupPerPrefab = 15;

    [Header("<color=yellow><b><size=15>Prefabs to Warm Up</size></b></color>")]
    [Tooltip("Drag projectile prefabs here to pre-warm the pool at Start.")]
    [SerializeField] private GameObject[] _warmupPrefabs;
    #endregion

    #region State
    private readonly Dictionary<GameObject, Queue<GameObject>> _poolDict = new();
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    private void Start()
    {
        foreach (GameObject prefab in _warmupPrefabs)
            Warmup(prefab, _warmupPerPrefab);
    }
    #endregion

    #region Public API
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_poolDict.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            _poolDict[prefab] = pool;
        }

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateInstance(prefab);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject projectile, GameObject sourcePrefab)
    {
        if (projectile == null) return;

        projectile.SetActive(false);

        if (sourcePrefab == null) { Destroy(projectile); return; }

        if (!_poolDict.ContainsKey(sourcePrefab))
            _poolDict[sourcePrefab] = new Queue<GameObject>();

        _poolDict[sourcePrefab].Enqueue(projectile);
    }

    public void Warmup(GameObject prefab, int count)
    {
        if (prefab == null) return;

        if (!_poolDict.ContainsKey(prefab))
            _poolDict[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = CreateInstance(prefab);
            obj.SetActive(false);
            _poolDict[prefab].Enqueue(obj);
        }
    }
    #endregion

    #region Helpers
    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);

        if (obj.TryGetComponent(out Projectile proj))
            proj.SetSourcePrefab(prefab);

        return obj;
    }
    #endregion
}
