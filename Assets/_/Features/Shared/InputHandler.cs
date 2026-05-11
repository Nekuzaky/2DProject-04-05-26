using UnityEngine;

// Single source of truth for ALL player input — keyboard and gamepad.
// Runs at execution order -20 so it always updates before PlayerController (-10),
// PlayerAim, and PlayerShooter.
//
// ─── GAMEPAD TROUBLESHOOTING ───────────────────────────────────────────────
// If your controller stopped working, check both of the following:
//
// 1. Edit → Project Settings → Player → Active Input Handling
//    Must be set to "Both" or "Input Manager (Old)".
//    If it's "Input System Package (New)", ALL Input.GetAxis/GetKey return 0.
//
// 2. Edit → Project Settings → Input Manager — these custom axes must exist:
//    Name                   | Type  | Axis          | Joy Num
//    ───────────────────────┼───────┼───────────────┼─────────
//    RightStickHorizontal   | Joystick Axis | 4th Axis | All
//    RightStickVertical     | Joystick Axis | 5th Axis | All
//    RightTrigger           | Joystick Axis | 3rd Axis | All
//    (exact axis numbers vary by controller — use Window → Analysis → Input Debugger to verify)
// ───────────────────────────────────────────────────────────────────────────

[DefaultExecutionOrder(-20)]
public class InputHandler : MonoBehaviour
{
    #region Singleton
    public static InputHandler Instance { get; private set; }
    #endregion

    #region Properties — Movement
    public Vector2 MoveDirection { get; private set; }
    public bool    IsSprinting   { get; private set; }
    #endregion

    #region Properties — Aim
    public Vector2 AimWorldPosition { get; private set; }
    public Vector2 RightStick       { get; private set; }
    #endregion

    #region Properties — Combat
    public bool IsFiring      { get; private set; }
    public bool FireDown      { get; private set; }
    public bool ReloadPressed { get; private set; }
    #endregion

    #region State
    private Camera _mainCamera;
    private bool   _hasRightStickAxes;
    private bool   _hasRightTriggerAxis;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        _mainCamera = Camera.main;

        // Check once if custom axes exist — avoids try/catch every frame
        _hasRightStickAxes   = AxisExists("RightStickHorizontal") && AxisExists("RightStickVertical");
        _hasRightTriggerAxis = AxisExists("RightTrigger");

        if (!_hasRightStickAxes)
            GameLogger.LogWarning("InputHandler: 'RightStickHorizontal' / 'RightStickVertical' missing in Input Manager. Gamepad aiming disabled.");

        if (!_hasRightTriggerAxis)
            GameLogger.LogWarning("InputHandler: 'RightTrigger' missing in Input Manager. Gamepad fire trigger disabled.");
    }

    // Uses regular Update (not UpdateManager) so input is always captured
    // before any subscriber processes it, regardless of UpdateManager state.
    private void Update()
    {
        ReadMovement();
        ReadAim();
        ReadCombat();
    }
    #endregion

    #region Input Reading
    private void ReadMovement()
    {
        // Left stick / WASD — Unity default axes, always present
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        MoveDirection = new Vector2(h, v).normalized;

        // Keyboard: Left Shift | Gamepad: LB/L1 (JoystickButton4)
        IsSprinting = Input.GetKey(KeyCode.LeftShift)
                   || Input.GetKey(KeyCode.JoystickButton4);
    }

    private void ReadAim()
    {
        if (_hasRightStickAxes)
        {
            float rx = Input.GetAxisRaw("RightStickHorizontal");
            float ry = Input.GetAxisRaw("RightStickVertical");
            RightStick = new Vector2(rx, ry);
        }
        else
        {
            RightStick = Vector2.zero;
        }

        // Lazy-init camera — Camera.main can be null at Awake during scene transitions
        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_mainCamera != null)
        {
            Vector3 world = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            AimWorldPosition = world;
        }
    }

    private void ReadCombat()
    {
        float trigger = _hasRightTriggerAxis ? Input.GetAxisRaw("RightTrigger") : 0f;

        IsFiring = Input.GetMouseButton(0)  || trigger > 0.1f;
        FireDown = Input.GetMouseButtonDown(0);

        // Reload: R key | Y / Triangle button (JoystickButton3)
        ReloadPressed = Input.GetKeyDown(KeyCode.R)
                     || Input.GetKeyDown(KeyCode.JoystickButton3);
    }
    #endregion

    #region Helpers
    private static bool AxisExists(string axisName)
    {
        try   { Input.GetAxisRaw(axisName); return true; }
        catch (System.ArgumentException)   { return false; }
    }
    #endregion
}
