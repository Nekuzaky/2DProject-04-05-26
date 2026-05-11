using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    #region Inspector Settings - References
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform     _aimPivot;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    #endregion

    #region Inspector Settings - Configuration
    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private float _weaponDistance = 2f;

    [Header("<color=yellow><b><size=15>Input</size></b></color>")]
    [SerializeField] private float _stickDeadzone = 0.2f;
    #endregion

    #region State
    private Camera _mainCamera;
    private bool   _usingUpdateManager;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
            GameLogger.LogError("<color=red>PlayerAim: Main Camera not found!</color>");

        if (_aimPivot == null)
            GameLogger.LogError("<color=red>PlayerAim: Aim Pivot not assigned!</color>");
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

    // Fallback: runs every frame when UpdateManager wasn't available at OnEnable.
    // Keeps aiming functional regardless of which scene we loaded from.
    private void Update()
    {
        // Lazy-subscribe if we missed the UpdateManager at OnEnable (scene transition timing)
        if (!_usingUpdateManager && UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
            _usingUpdateManager = true;
            return; // OnUpdateTick will fire via UpdateManager from next frame
        }

        if (!_usingUpdateManager)
        {
            // Lazy-init camera in case it was null at Awake (scene still loading)
            if (_mainCamera == null) _mainCamera = Camera.main;
            OnUpdateTick();
        }
    }

    private void OnDisable()
    {
        if (_usingUpdateManager && UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;

        _usingUpdateManager = false;
    }
    #endregion

    #region Input & Aiming
    private void OnUpdateTick()
    {
        if (_aimPivot == null || _mainCamera == null) return;

        Vector2 aimDir = GetAimDirection();
        if (aimDir.sqrMagnitude < 0.01f) return;

        bool  isFacingLeft = aimDir.x < 0;
        float angle        = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        _aimPivot.rotation   = Quaternion.Euler(0, 0, angle);
        _aimPivot.localScale = Vector3.one;

        if (_weaponRenderer) _weaponRenderer.flipY = isFacingLeft;

        _aimPivot.position = (Vector2)transform.position + aimDir.normalized * _weaponDistance;
    }

    private Vector2 GetAimDirection()
    {
        // Right stick via InputHandler (avoids direct axis calls + handles missing axes gracefully)
        if (InputHandler.Instance != null)
        {
            Vector2 stick = InputHandler.Instance.RightStick;
            if (stick.magnitude > _stickDeadzone)
                return stick.normalized;

            return InputHandler.Instance.AimWorldPosition - (Vector2)transform.position;
        }

        // Fallback: mouse only
        Vector2 mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorld - (Vector2)transform.position;
    }
    #endregion
}
