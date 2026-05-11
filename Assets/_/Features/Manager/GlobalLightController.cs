using UnityEngine;
using UnityEngine.Rendering.Universal;

// Attach to the Global Light 2D in the scene.
// Sets a dark ambient baseline for horror atmosphere.
// Exposes an API to smoothly transition ambient intensity/color (e.g., for cutscenes).
[RequireComponent(typeof(Light2D))]
public class GlobalLightController : MonoBehaviour
{
    #region Singleton
    public static GlobalLightController Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=grey><b><size=15>Ambient</size></b></color>")]
    [Tooltip("Target ambient intensity while playing. 0.08 = nearly black, horror feel.")]
    [SerializeField] [Range(0f, 1f)] private float _ambientIntensity = 0.08f;
    [Tooltip("Deep blue-black tint — avoids pure grey for a more cinematic look.")]
    [SerializeField] private Color _ambientColor = new Color(0.05f, 0.05f, 0.15f);

    [Header("<color=cyan><b><size=15>Transitions</size></b></color>")]
    [Tooltip("Units per second for intensity lerp.")]
    [SerializeField] private float _transitionSpeed = 1.5f;
    #endregion

    #region State
    private Light2D _light;
    private float   _targetIntensity;
    private Color   _targetColor;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        _light = GetComponent<Light2D>();

        _targetIntensity  = _ambientIntensity;
        _targetColor      = _ambientColor;

        // Apply immediately — no lerp on first frame
        _light.intensity  = _ambientIntensity;
        _light.color      = _ambientColor;
    }

    private void Update()
    {
        _light.intensity = Mathf.MoveTowards(_light.intensity, _targetIntensity, _transitionSpeed * Time.deltaTime);
        _light.color     = Color.Lerp(_light.color, _targetColor, _transitionSpeed * Time.deltaTime);
    }
    #endregion

    #region Public API
    // Smoothly transition ambient to any intensity and color.
    public void SetAmbient(float intensity, Color color)
    {
        _targetIntensity = Mathf.Clamp01(intensity);
        _targetColor     = color;
    }

    // Snap back to the serialized night defaults.
    public void FadeToNight() => SetAmbient(_ambientIntensity, _ambientColor);

    // Quick flash (e.g., lightning, explosion) — returns to night automatically.
    public void Flash(float intensity, Color color, float duration)
    {
        SetAmbient(intensity, color);
        // Revert after duration using a coroutine via MonoBehaviour
        StopAllCoroutines();
        StartCoroutine(RevertAfter(duration));
    }

    public float CurrentIntensity => _light != null ? _light.intensity : 0f;
    #endregion

    #region Helpers
    private System.Collections.IEnumerator RevertAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        FadeToNight();
    }
    #endregion
}
