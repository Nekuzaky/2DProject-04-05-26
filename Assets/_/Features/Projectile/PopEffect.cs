using UnityEngine;

public class PopEffect : MonoBehaviour
{
    #region Inspector Settings
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private AnimationCurve _scaleCurve;
    #endregion

    #region State
    private float _timer;
    private Vector3 _baseScale;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _baseScale = transform.localScale;
        _timer = _duration;

        if (_scaleCurve == null || _scaleCurve.length == 0)
        {
            _scaleCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.35f, 1.5f),
                new Keyframe(1f, 0f)
            );
        }
    }

    private void Start()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }
    #endregion

    #region Animation
    private void OnUpdateTick()
    {
        _timer -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(_timer / _duration);
        transform.localScale = _baseScale * _scaleCurve.Evaluate(t);

        if (_timer <= 0f)
            Destroy(gameObject);
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
