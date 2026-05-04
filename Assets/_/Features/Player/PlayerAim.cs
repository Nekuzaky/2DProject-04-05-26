using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Pivot")]
    [SerializeField] private Transform aimPivot;

    [Header("Controller")]
    [SerializeField] private float stickDeadzone = 0.2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 aimDir = GetAimDirection();
        if (aimDir.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 GetAimDirection()
    {

        float rx = Input.GetAxisRaw("RightStickHorizontal");
        float ry = Input.GetAxisRaw("RightStickVertical");
        Vector2 stick = new Vector2(rx, ry);
        if (stick.magnitude > stickDeadzone)
            return stick.normalized;
  
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorld - (Vector2)aimPivot.position;
    }
}