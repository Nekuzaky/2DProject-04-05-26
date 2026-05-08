using UnityEngine;

public class WorldStreamer : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Props</size></b></color>")]
    [SerializeField] private Sprite[] _propSprites;
    [SerializeField] private int      _propCount       = 25;
    [SerializeField] private float    _spawnRadius     = 14f;
    [SerializeField] private float    _recycleDistance = 11f;

    [Header("<color=cyan><b><size=15>Visuals</size></b></color>")]
    [SerializeField] private Material _propMaterial;
    [SerializeField] private int      _sortingOrder    = -5;
    [SerializeField] private Vector2  _scaleRange      = new Vector2(0.6f, 1.2f);

    private Transform        _player;
    private Transform[]      _props;
    private SpriteRenderer[] _renderers;

    private void Start()
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        if (pc != null) _player = pc.transform;

        InitProps();

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void InitProps()
    {
        _props     = new Transform[_propCount];
        _renderers = new SpriteRenderer[_propCount];

        Vector3 center = _player != null ? _player.position : Vector3.zero;

        for (int i = 0; i < _propCount; i++)
        {
            GameObject go = new($"Prop_{i}");
            go.transform.SetParent(transform);

            SpriteRenderer sr  = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder    = _sortingOrder;
            if (_propMaterial != null) sr.material = _propMaterial;

            _props[i]     = go.transform;
            _renderers[i] = sr;

            Reposition(i, center);
        }
    }

    private void OnUpdateTick()
    {
        if (_player == null) return;

        Vector3 playerPos = _player.position;

        for (int i = 0; i < _propCount; i++)
        {
            if (Vector2.Distance(_props[i].position, playerPos) > _recycleDistance)
                Reposition(i, playerPos);
        }
    }

    private void Reposition(int index, Vector3 center)
    {
        // Position aléatoire dans l'anneau entre recycleDistance et spawnRadius
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist  = Random.Range(_recycleDistance * 0.4f, _spawnRadius);

        _props[index].position = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
        _props[index].rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        float scale = Random.Range(_scaleRange.x, _scaleRange.y);
        _props[index].localScale = Vector3.one * scale;

        if (_propSprites != null && _propSprites.Length > 0)
            _renderers[index].sprite = _propSprites[Random.Range(0, _propSprites.Length)];
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}
