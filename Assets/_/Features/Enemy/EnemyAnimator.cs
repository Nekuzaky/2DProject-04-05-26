using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private float _bobFrequency = 9f;
    [SerializeField] private float _squishAmount  = 0.08f;

    private SpriteRenderer _renderer;
    private Rigidbody2D    _rb;
    private Vector3        _baseScale;
    private float          _bobTimer;

    private void Awake()
    {
        _renderer  = GetComponentInChildren<SpriteRenderer>(true);
        _rb        = GetComponent<Rigidbody2D>();
        _baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        _bobTimer = 0f;
        transform.localScale = _baseScale;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        float speed   = _rb != null ? _rb.linearVelocity.magnitude : 0f;
        bool isMoving = speed > 0.1f;

        // Flip selon la direction horizontale
        if (_renderer != null && _rb != null && Mathf.Abs(_rb.linearVelocity.x) > 0.1f)
            _renderer.flipX = _rb.linearVelocity.x < 0f;

        if (!isMoving)
        {
            transform.localScale = _baseScale;
            return;
        }

        // Squish/stretch
        _bobTimer += Time.deltaTime * _bobFrequency * (speed / 3f);
        float bob    = Mathf.Sin(_bobTimer);
        float scaleY = _baseScale.y * (1f + bob * _squishAmount);
        float scaleX = _baseScale.x * (1f - bob * _squishAmount * 0.5f);
        transform.localScale = new Vector3(scaleX, scaleY, _baseScale.z);
    }

    private void OnDisable()
    {
        transform.localScale = _baseScale;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
