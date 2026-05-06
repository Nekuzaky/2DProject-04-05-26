using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Bob</size></b></color>")]
    [SerializeField] private float _bobFrequency = 9f;
    [SerializeField] private float _squishAmount  = 0.10f;

    [Header("<color=cyan><b><size=15>Lean</size></b></color>")]
    [SerializeField] private float _leanAngle     = 12f;
    [SerializeField] private float _leanSmoothing = 8f;

    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform _visual;   // child sprite object (or leave null to use self)

    private SpriteRenderer _renderer;
    private Rigidbody2D    _rb;
    private Vector3        _baseScale;
    private float          _bobTimer;

    private void Awake()
    {
        Transform target = _visual != null ? _visual : transform;
        _renderer  = target.GetComponent<SpriteRenderer>();
        _rb        = GetComponent<Rigidbody2D>();
        _baseScale = target.localScale;
    }

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnUpdateTick()
    {
        Transform target = _visual != null ? _visual : transform;
        float speed    = _rb != null ? _rb.linearVelocity.magnitude : 0f;
        bool isMoving  = speed > 0.1f;

        // Flip sprite based on horizontal direction
        if (_renderer != null && _rb != null && Mathf.Abs(_rb.linearVelocity.x) > 0.1f)
            _renderer.flipX = _rb.linearVelocity.x < 0f;

        if (!isMoving)
        {
            target.localScale = Vector3.Lerp(target.localScale, _baseScale, Time.deltaTime * 12f);
            target.localRotation = Quaternion.Lerp(target.localRotation, Quaternion.identity, Time.deltaTime * 12f);
            return;
        }

        // Bob timer scales with speed
        _bobTimer += Time.deltaTime * _bobFrequency * (speed / 3f);

        // Squish/stretch : compression Y au sol, étirement au saut
        float bob    = Mathf.Sin(_bobTimer);
        float scaleY = _baseScale.y * (1f + bob * _squishAmount);
        float scaleX = _baseScale.x * (1f - bob * _squishAmount * 0.5f);
        target.localScale = new Vector3(scaleX, scaleY, _baseScale.z);

        // Lean uniquement sur un visual enfant (jamais sur le root avec Rigidbody2D)
        if (_visual != null && _rb != null)
        {
            float targetAngle  = -_rb.linearVelocity.x * _leanAngle / Mathf.Max(speed, 0.1f);
            float currentAngle = _visual.localEulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * _leanSmoothing);
            _visual.localRotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }

    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
