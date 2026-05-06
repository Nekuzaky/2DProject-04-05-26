using UnityEngine;

public class PopEffect : MonoBehaviour
{
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private AnimationCurve _scaleCurve;

    private float _timer;
    private Vector3 _baseScale;

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

    private void OnUpdateTick()
    {
        _timer -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(_timer / _duration);
        transform.localScale = _baseScale * _scaleCurve.Evaluate(t);

        if (_timer <= 0f)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}
