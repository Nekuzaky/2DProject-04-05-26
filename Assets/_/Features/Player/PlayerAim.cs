using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform _aimPivot;

    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private float _weaponDistance = 0.5f;

    [Header("<color=yellow><b><size=15>Input</size></b></color>")]
    [SerializeField] private float _stickDeadzone = 0.2f;

    private Camera _mainCamera;

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

    private void Update()
    {
        if (_aimPivot == null || _mainCamera == null) return;
        
        Vector2 aimDir = GetAimDirection();
        if (aimDir.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        
        bool isFacingLeft = aimDir.x < 0;
        
        _aimPivot.rotation = Quaternion.Euler(0, 0, angle);
        
        
        float parentScaleX = transform.localScale.x;
        Vector3 gunScale = _aimPivot.localScale;
        gunScale.x = 1f / Mathf.Abs(parentScaleX); 
        gunScale.y = (isFacingLeft ? -1 : 1) / Mathf.Abs(parentScaleX); 
        _aimPivot.localScale = gunScale;
        
        Vector2 orbitPosition = (Vector2)transform.position + (aimDir.normalized * _weaponDistance);
        _aimPivot.position = orbitPosition;
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
}