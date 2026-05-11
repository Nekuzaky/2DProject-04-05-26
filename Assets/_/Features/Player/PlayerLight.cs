using UnityEngine;
using UnityEngine.Rendering.Universal;

// Attach to the Player GameObject (requires a Light2D on the same object).
// Simulates a carried light source (lighter / lantern) with a candle flicker.
// Tints green when WeedEffect is active — hook LucyEffect the same way if desired.
[RequireComponent(typeof(Light2D))]
public class PlayerLight : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=orange><b><size=15>Base Light</size></b></color>")]
    [Tooltip("Warm orange = lighter, cool white = moon.")]
    [SerializeField] private Color _baseColor = new Color(1f, 0.75f, 0.4f);
    [SerializeField] [Range(0f, 5f)] private float _baseIntensity = 1.2f;
    [SerializeField] private float _outerRadius = 5f;
    [SerializeField] private float _innerRadius = 1f;
    [SerializeField] [Range(0f, 1f)] private float _falloff = 0.5f;

    [Header("<color=yellow><b><size=15>Flicker</size></b></color>")]
    [Tooltip("Higher = more nervous flame.")]
    [SerializeField] private float _flickerSpeed = 9f;
    [SerializeField] [Range(0f, 0.3f)] private float _flickerStrength = 0.1f;

    [Header("<color=lime><b><size=15>Drug Integration</size></b></color>")]
    [Tooltip("Extra intensity added at peak drug effect.")]
    [SerializeField] private float _drugIntensityBoost = 0.5f;
    [SerializeField] private Color _weedTint = new Color(0.3f, 1f, 0.3f);
    #endregion

    #region State
    private Light2D _light;
    private float   _noiseOffset;
    private bool    _usingUpdateManager;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _light.lightType              = Light2D.LightType.Point;
        _light.color                  = _baseColor;
        _light.intensity              = _baseIntensity;
        _light.pointLightOuterRadius  = _outerRadius;
        _light.pointLightInnerRadius  = _innerRadius;
        _light.falloffIntensity       = _falloff;

        _noiseOffset = Random.Range(0f, 100f);
    }

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
            _usingUpdateManager = true;
        }
        else
        {
            _usingUpdateManager = false;
        }
    }

    private void OnDisable()
    {
        if (_usingUpdateManager && UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;

        _usingUpdateManager = false;
    }

    private void Update()
    {
        if (!_usingUpdateManager) OnUpdateTick();
    }
    #endregion

    #region Tick
    private void OnUpdateTick()
    {
        float weedT = WeedEffect.Instance != null && WeedEffect.Instance.IsActive
            ? WeedEffect.Instance.RemainingRatio
            : 0f;

        // Uncomment when LucyEffect exposes IsActive / RemainingRatio:
        // float lucyT = LucyEffect.Instance != null && LucyEffect.Instance.IsActive
        //     ? LucyEffect.Instance.RemainingRatio
        //     : 0f;
        float lucyT = 0f;

        float drugT = Mathf.Max(weedT, lucyT);

        // Perlin noise flicker (candle feel — non-repeating, smooth)
        float noise   = Mathf.PerlinNoise(Time.time * _flickerSpeed + _noiseOffset, 0f);
        float flicker = Mathf.Lerp(-_flickerStrength, _flickerStrength, noise);

        _light.intensity = Mathf.Max(0f, _baseIntensity + flicker + _drugIntensityBoost * drugT);

        // Color tint toward drug color
        Color target = _baseColor;
        if (weedT > 0f) target = Color.Lerp(_baseColor, _weedTint, weedT * 0.6f);
        _light.color = target;
    }
    #endregion
}
