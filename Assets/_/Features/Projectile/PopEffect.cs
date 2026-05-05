using UnityEngine;

public class PopEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private AnimationCurve scaleCurve;

    private float timer;
    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
        timer = duration;

        if (scaleCurve == null || scaleCurve.length == 0)
        {
            scaleCurve = new AnimationCurve(
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
        timer -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(timer / duration);
        transform.localScale = baseScale * scaleCurve.Evaluate(t);

        if (timer <= 0f)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}