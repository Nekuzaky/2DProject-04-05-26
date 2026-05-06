using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    #region Inspector Settings - References
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform _aimPivot;
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
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
            Debug.LogError("<color=red>PlayerAim: Main Camera not found!</color>");
        
        if (_aimPivot == null)
            Debug.LogError("<color=red>PlayerAim: Aim Pivot not assigned!</color>");
        else
            Debug.Log("<color=green>PlayerAim: Ready - Aim Pivot assigned</color>");
    }

    private void Start()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }
    #endregion

    #region Input & Aiming
    private void OnUpdateTick()
    {
        if (_aimPivot == null || _mainCamera == null) return;

        Vector2 aimDir = GetAimDirection();
        if (aimDir.sqrMagnitude < 0.01f) return;

        bool isFacingLeft = aimDir.x < 0;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        _aimPivot.rotation = Quaternion.Euler(0, 0, angle);
        _aimPivot.localScale = Vector3.one;

        if (_weaponRenderer) _weaponRenderer.flipY = isFacingLeft;

        _aimPivot.position = (Vector2)transform.position + aimDir.normalized * _weaponDistance;
    }

    private Vector2 GetAimDirection()
    {
        Vector2 stick = new Vector2(
            Input.GetAxisRaw("RightStickHorizontal"),
            Input.GetAxisRaw("RightStickVertical")
        );
        if (stick.magnitude > _stickDeadzone)
            return stick.normalized;

        Vector2 mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorld - (Vector2)transform.position;
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
