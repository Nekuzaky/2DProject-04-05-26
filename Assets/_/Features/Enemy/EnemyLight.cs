using UnityEngine;
using UnityEngine.Rendering.Universal;

// Attach to any enemy prefab root.
// Creates a child Light2D automatically at runtime — no prefab modification needed.
// Pool-safe: light is toggled on/off via OnEnable/OnDisable, not destroyed.
public class EnemyLight : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=red><b><size=15>Enemy Light</size></b></color>")]
    [Tooltip("Hostile red by default — orange for melee, pale blue for ghosts, etc.")]
    [SerializeField] private Color _lightColor = new Color(1f, 0.2f, 0.1f);
    [SerializeField] [Range(0f, 3f)] private float _intensity = 0.8f;
    [SerializeField] private float _outerRadius = 1.8f;
    [SerializeField] private float _innerRadius = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float _falloff = 0.8f;

    [Header("<color=yellow><b><size=15>Pulse</size></b></color>")]
    [Tooltip("Each enemy gets a random phase so they don't all pulse in sync.")]
    [SerializeField] private float _pulseSpeed = 2.5f;
    [SerializeField] [Range(0f, 0.5f)] private float _pulseStrength = 0.2f;
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
        GameObject lightObj = new GameObject("EnemyLight");
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

        float pulse = Mathf.Sin(Time.time * _pulseSpeed + _phaseOffset);
        _light.intensity = Mathf.Max(0f, _intensity + pulse * _pulseStrength);
    }
    #endregion
}
