using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform aimPivot;

    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private float weaponDistance = 0.5f;

    [Header("<color=yellow><b><size=15>Input</size></b></color>")]
    [SerializeField] private float stickDeadzone = 0.2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
            Debug.LogError("<color=red>PlayerAim: Main Camera not found!</color>");
        
        if (aimPivot == null)
            Debug.LogError("<color=red>PlayerAim: Aim Pivot not assigned!</color>");
        else
            Debug.Log("<color=green>PlayerAim: Ready - Aim Pivot assigned</color>");
    }

    private void Update()
    {
        if (aimPivot == null || mainCamera == null) return;
        
        Vector2 aimDir = GetAimDirection();
        if (aimDir.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        
        bool isFacingLeft = aimDir.x < 0;
        
        aimPivot.rotation = Quaternion.Euler(0, 0, angle);
        
        
        float parentScaleX = transform.localScale.x;
        Vector3 gunScale = aimPivot.localScale;
        gunScale.x = 1f / Mathf.Abs(parentScaleX); 
        gunScale.y = (isFacingLeft ? -1 : 1) / Mathf.Abs(parentScaleX); 
        aimPivot.localScale = gunScale;
        
        Vector2 orbitPosition = (Vector2)transform.position + (aimDir.normalized * weaponDistance);
        aimPivot.position = orbitPosition;
    }

    private Vector2 GetAimDirection()
    {
        
        try
        {
            float rx = Input.GetAxisRaw("RightStickHorizontal");
            float ry = Input.GetAxisRaw("RightStickVertical");
            Vector2 stick = new Vector2(rx, ry);
            if (stick.magnitude > stickDeadzone)
                return stick.normalized;
        }
        catch (System.ArgumentException)
        {
            
        }
  
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorld - (Vector2)transform.position;
    }
}