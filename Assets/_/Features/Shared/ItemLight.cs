using UnityEngine;
using UnityEngine.Rendering.Universal;

// Attach to any pickup item root.
// Creates a child Light2D automatically — no prefab modification needed.
// Pool-safe: light toggled via OnEnable/OnDisable.
public class ItemLight : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=lime><b><size=15>Item Light</size></b></color>")]
    [Tooltip("Match the item identity: green=weed, purple=lucy, yellow=cash, red=crack…")]
    [SerializeField] private Color _lightColor     = new Color(0.3f, 1f, 0.3f);
    [SerializeField] [Range(0f, 3f)] private float _intensity    = 0.9f;
    [SerializeField] private float                 _outerRadius  = 1.2f;
    [SerializeField] private float                 _innerRadius  = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float _falloff      = 0.9f;

    [Header("<color=yellow><b><size=15>Pulse</size></b></color>")]
    [Tooltip("Slow breathe = mysterious object. Keep below 1.5 for items.")]
    [SerializeField] private float                 _pulseSpeed   = 0.8f;
    [SerializeField] [Range(0f, 0.5f)] private float _pulseStrength = 0.3f;
    #endregion

    #region State
    private Light2D _light;
    private float   _phaseOffset;
    private bool    _usingUpdateManager;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        CreateLight();
    }

    private void OnEnable()
    {
        if (_light != null) _light.enabled = true;

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
        if (_light != null) _light.enabled = false;

        if (_usingUpdateManager && UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;

        _usingUpdateManager = false;
    }

    private void Update()
    {
        if (!_usingUpdateManager) OnUpdateTick();
    }
    #endregion

    #region Setup
    private void CreateLight()
    {
        GameObject lightObj = new GameObject("ItemLight");
        lightObj.transform.SetParent(transform, false);

        _light = lightObj.AddComponent<Light2D>();
        _light.lightType             = Light2D.LightType.Point;
        _light.color                 = _lightColor;
        _light.intensity             = _intensity;
        _light.pointLightOuterRadius = _outerRadius;
        _light.pointLightInnerRadius = _innerRadius;
        _light.falloffIntensity      = _falloff;
    }
    #endregion

    #region Tick
    private void OnUpdateTick()
    {
        if (_light == null) return;

        // Smooth sine breathe — slower and softer than EnemyLight
        float breathe = Mathf.Sin(Time.time * _pulseSpeed + _phaseOffset);
        _light.intensity = Mathf.Max(0f, _intensity + breathe * _pulseStrength);
    }
    #endregion
}
