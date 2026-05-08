using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Singleton — attach to the Main Camera.
// Handles all weed-related post-processing visual effects:
//   Saturation, Hue Drift, Chromatic Aberration, Green Vignette, Camera Breathe.
// Creates its own high-priority Volume to avoid modifying the scene Volume.
public class WeedEffect : MonoBehaviour
{
    #region Singleton
    public static WeedEffect Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=lime><b><size=15>Saturation</size></b></color>")]
    [Tooltip("Maximum saturation increase (more vivid colors).")]
    [SerializeField] private float _maxSaturation = 80f;

    [Header("<color=lime><b><size=15>Hue Drift</size></b></color>")]
    [Tooltip("Maximum hue drift in degrees.")]
    [SerializeField] private float _maxHueDrift = 20f;
    [SerializeField] private float _hueDriftSpeed = 0.3f;

    [Header("<color=lime><b><size=15>Chromatic Aberration</size></b></color>")]
    [Tooltip("Maximum chromatic aberration intensity.")]
    [SerializeField] [Range(0f, 1f)] private float _maxChroma = 0.5f;
    [SerializeField] private float _chromaPulseSpeed = 3f;

    [Header("<color=lime><b><size=15>Vignette</size></b></color>")]
    [SerializeField] private Color _vignetteColor = new Color(0.05f, 0.25f, 0.05f);
    [Tooltip("Maximum vignette intensity.")]
    [SerializeField] [Range(0f, 1f)] private float _maxVignetteIntensity = 0.45f;
    [SerializeField] private float _vignettePulseSpeed = 1.2f;

    [Header("<color=lime><b><size=15>Camera Breathe</size></b></color>")]
    [Tooltip("Orthographic size delta for the breathing zoom effect.")]
    [SerializeField] private float _breatheAmount = 0.25f;
    [SerializeField] private float _breatheSpeed = 0.7f;
    #endregion

    #region State
    private float _elapsed;
    private float _duration;
    private bool  _isActive;

    private Volume              _effectVolume;
    private ColorAdjustments    _colorAdj;
    private ChromaticAberration _chromAber;
    private Vignette            _vignette;

    private Camera _mainCamera;
    private float  _baseOrthoSize;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        _mainCamera = Camera.main;
        if (_mainCamera != null)
            _baseOrthoSize = _mainCamera.orthographicSize;

        SetupVolume();
    }

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }

    private void OnDestroy()
    {
        OnDisable();
        if (_effectVolume != null && _effectVolume.profile != null)
            Destroy(_effectVolume.profile);
    }
    #endregion

    #region Setup
    private void SetupVolume()
    {
        _effectVolume          = gameObject.AddComponent<Volume>();
        _effectVolume.isGlobal = true;
        _effectVolume.priority = 5f;
        _effectVolume.weight   = 0f;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();

        _colorAdj  = profile.Add<ColorAdjustments>();
        _chromAber = profile.Add<ChromaticAberration>();
        _vignette  = profile.Add<Vignette>();

        _colorAdj.saturation.Override(0f);
        _colorAdj.hueShift.Override(0f);
        _chromAber.intensity.Override(0f);
        _vignette.color.Override(_vignetteColor);
        _vignette.intensity.Override(0f);

        _effectVolume.profile = profile;
    }
    #endregion

    #region Public API
    public void Trigger(float duration)
    {
        _elapsed  = 0f;
        _duration = Mathf.Max(0.1f, duration);
        _isActive = true;
        Debug.Log($"<color=lime><b>WeedEffect:</b></color> Effect triggered for {duration}s.");
    }

    public bool  IsActive       => _isActive;
    public float RemainingRatio => _isActive ? 1f - Mathf.Clamp01(_elapsed / _duration) : 0f;
    #endregion

    #region Effect Tick
    private void OnUpdateTick()
    {
        if (!_isActive) return;

        _elapsed += Time.deltaTime;

        float t        = Mathf.Clamp01(_elapsed / _duration);
        float envelope = ComputeEnvelope(t);

        _effectVolume.weight = envelope;

        ApplySaturation(envelope);
        ApplyHueDrift(envelope);
        ApplyChromaticAberration(envelope);
        ApplyVignette(envelope);
        ApplyCameraBreath(envelope);

        if (_elapsed >= _duration)
            ResetEffect();
    }

    // Fast rise (first 10% of duration), slow decay (remaining 90%)
    private float ComputeEnvelope(float t)
    {
        float fadeIn  = Mathf.Clamp01(t / 0.1f);
        float fadeOut = 1f - Mathf.Clamp01((t - 0.1f) / 0.9f);
        return fadeIn * fadeOut;
    }

    private void ApplySaturation(float envelope)
    {
        _colorAdj.saturation.Override(_maxSaturation * envelope);
    }

    private void ApplyHueDrift(float envelope)
    {
        _colorAdj.hueShift.Override(Mathf.Sin(_elapsed * _hueDriftSpeed) * _maxHueDrift * envelope);
    }

    private void ApplyChromaticAberration(float envelope)
    {
        float pulse = 0.4f + 0.6f * Mathf.Abs(Mathf.Sin(_elapsed * _chromaPulseSpeed));
        _chromAber.intensity.Override(_maxChroma * pulse * envelope);
    }

    private void ApplyVignette(float envelope)
    {
        float pulse = 0.6f + 0.4f * Mathf.Sin(_elapsed * _vignettePulseSpeed);
        _vignette.intensity.Override(_maxVignetteIntensity * pulse * envelope);
    }

    private void ApplyCameraBreath(float envelope)
    {
        if (_mainCamera == null || !_mainCamera.orthographic) return;

        float breathe = Mathf.Sin(_elapsed * _breatheSpeed * Mathf.PI * 2f) * _breatheAmount;
        _mainCamera.orthographicSize = _baseOrthoSize + breathe * envelope;
    }
    #endregion

    #region Reset
    private void ResetEffect()
    {
        _isActive            = false;
        _effectVolume.weight = 0f;

        if (_mainCamera != null)
            _mainCamera.orthographicSize = _baseOrthoSize;

        Debug.Log("<color=grey><b>WeedEffect:</b></color> Effect expired.");
    }
    #endregion
}
