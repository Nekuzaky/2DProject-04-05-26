using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Light2D = UnityEngine.Rendering.Universal.Light2D;

// Singleton — attach to the Main Camera.
// Handles all Lucy (LSD) related visual effects:
//   RGB Hue Cycling, Chromatic Aberration, Red Pulsing Halo on the Player.
// Creates its own high-priority Volume to avoid modifying the scene Volume.
public class LucyEffect : MonoBehaviour
{
    #region Singleton
    public static LucyEffect Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=magenta><b><size=15>RGB Screen</size></b></color>")]
    [Tooltip("Speed of hue cycling in degrees per second.")]
    [SerializeField] private float _hueCycleSpeed = 70f;

    [Tooltip("Additional saturation while the effect is active.")]
    [SerializeField] [Range(0f, 100f)] private float _saturationBoost = 35f;

    [Tooltip("Maximum chromatic aberration intensity (keep low for subtle effect).")]
    [SerializeField] [Range(0f, 1f)] private float _maxChroma = 0.2f;

    [Tooltip("Speed of chromatic aberration pulsing.")]
    [SerializeField] private float _chromaPulseSpeed = 2.5f;

    [Header("<color=red><b><size=15>Red Halo</size></b></color>")]
    [Tooltip("Color of the pulsing halo around the player.")]
    [SerializeField] private Color _haloColor = new Color(1f, 0.05f, 0.05f);

    [Tooltip("Maximum light intensity of the halo.")]
    [SerializeField] private float _haloMaxIntensity = 3f;

    [Tooltip("Pulse speed of the halo in Hz.")]
    [SerializeField] private float _haloPulseSpeed = 2.5f;

    [Tooltip("Outer radius of the halo light in world units.")]
    [SerializeField] private float _haloOuterRadius = 1.8f;
    #endregion

    #region State
    private float _elapsed;
    private float _duration;
    private bool  _isActive;

    private Volume              _effectVolume;
    private ColorAdjustments    _colorAdj;
    private ChromaticAberration _chromAber;

    private GameObject _haloObject;
    private Light2D    _haloLight;
    private Camera     _mainCamera;
    private bool       _usingUpdateManager;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        _mainCamera = Camera.main;
        if (_mainCamera == null)
            _mainCamera = GetComponent<Camera>();

        EnsurePostProcessingEnabled();

        SetupVolume();
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
        if (!_usingUpdateManager)
            OnUpdateTick();
    }

    private void OnDestroy()
    {
        OnDisable();
        if (_effectVolume != null && _effectVolume.profile != null)
            Destroy(_effectVolume.profile);
        DestroyHalo();
    }
    #endregion

    #region Setup
    private void SetupVolume()
    {
        _effectVolume          = gameObject.AddComponent<Volume>();
        _effectVolume.isGlobal = true;
        _effectVolume.priority = 6f;
        _effectVolume.weight   = 0f;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();

        _colorAdj  = profile.Add<ColorAdjustments>();
        _chromAber = profile.Add<ChromaticAberration>();

        _colorAdj.hueShift.Override(0f);
        _colorAdj.saturation.Override(0f);
        _chromAber.intensity.Override(0f);

        _effectVolume.profile = profile;
    }

    private void EnsurePostProcessingEnabled()
    {
        if (_mainCamera == null)
            return;

        if (_mainCamera.TryGetComponent(out UniversalAdditionalCameraData cameraData))
            cameraData.renderPostProcessing = true;
    }

    private void CreateHalo(Transform parent)
    {
        DestroyHalo();

        _haloObject = new GameObject("LucyHalo");
        _haloObject.transform.SetParent(parent, false);
        _haloObject.transform.localPosition = Vector3.zero;

        _haloLight = _haloObject.AddComponent<Light2D>();
        _haloLight.lightType             = Light2D.LightType.Point;
        _haloLight.color                 = _haloColor;
        _haloLight.intensity             = 0f;
        _haloLight.pointLightOuterRadius = _haloOuterRadius;
        _haloLight.pointLightInnerRadius = _haloOuterRadius * 0.25f;
    }

    private void DestroyHalo()
    {
        if (_haloObject != null)
        {
            Destroy(_haloObject);
            _haloObject = null;
            _haloLight  = null;
        }
    }
    #endregion

    #region Public API
    /// <summary>Trigger the Lucy effect on the player.</summary>
    /// <param name="duration">Visual effect duration in seconds.</param>
    /// <param name="playerTransform">Player transform to attach the halo to.</param>
    public void Trigger(float duration, Transform playerTransform)
    {
        _elapsed  = 0f;
        _duration = Mathf.Max(0.1f, duration);
        _isActive = true;

        if (playerTransform != null)
            CreateHalo(playerTransform);

        GameLogger.Log($"<color=magenta><b>LucyEffect:</b></color> Effect triggered for {duration}s.");
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

        ApplyHueCycle(envelope);
        ApplyChromaticAberration(envelope);
        ApplyHalo(envelope);

        if (_elapsed >= _duration)
            ResetEffect();
    }

    // Fast rise (first 10% of duration), smooth decay (remaining 90%)
    private static float ComputeEnvelope(float t)
    {
        float fadeIn  = Mathf.Clamp01(t / 0.1f);
        float fadeOut = 1f - Mathf.Clamp01((t - 0.1f) / 0.9f);
        return fadeIn * fadeOut;
    }

    private void ApplyHueCycle(float envelope)
    {
        float hue = Mathf.Repeat(_elapsed * _hueCycleSpeed, 360f) - 180f;
        _colorAdj.hueShift.Override(hue * envelope);
        _colorAdj.saturation.Override(_saturationBoost * envelope);
    }

    private void ApplyChromaticAberration(float envelope)
    {
        float pulse = 0.5f + 0.5f * Mathf.Abs(Mathf.Sin(_elapsed * _chromaPulseSpeed));
        _chromAber.intensity.Override(_maxChroma * pulse * envelope);
    }

    private void ApplyHalo(float envelope)
    {
        if (_haloLight == null) return;

        // Null-check: player may have been destroyed before effect ends
        if (_haloObject == null) { _haloLight = null; return; }

        float pulse = 0.5f + 0.5f * Mathf.Sin(_elapsed * _haloPulseSpeed * Mathf.PI * 2f);
        _haloLight.intensity = _haloMaxIntensity * pulse * envelope;
    }
    #endregion

    #region Reset
    private void ResetEffect()
    {
        _isActive            = false;
        _effectVolume.weight = 0f;
        _colorAdj.hueShift.Override(0f);
        _colorAdj.saturation.Override(0f);

        DestroyHalo();

        GameLogger.Log("<color=grey><b>LucyEffect:</b></color> Effect expired.");
    }
    #endregion
}
