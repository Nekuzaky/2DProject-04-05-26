using UnityEngine;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Settings - Movement
    [Header("<color=orange><b><size=15>Movement</size></b></color>")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runningMultiplier = 1.5f;
    #endregion

    #region Inspector Settings - Visual & Camera
    [Header("<color=cyan><b><size=15>Visual</size></b></color>")]
    [SerializeField] private Transform _playerSprite;

    [Header("<color=grey><b><size=15>Camera</size></b></color>")]
    [SerializeField] private float _camSmoothing = 6f;
    [SerializeField] private Vector3 _camOffset = new Vector3(0f, 0f, -10f);
    #endregion

    #region State
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Transform _cam;
    private Camera _mainCamera;
    private PlayerStats _playerStats;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _cam = Camera.main?.transform;
        _mainCamera = Camera.main;
        _playerStats = GetComponent<PlayerStats>();
    }
    private void Start()
    {
        Debug.Log("<color=green><b>PlayerController:</b></color> Ready");
    }
    #endregion

    #region Input & Movement
    private void Update()
    {
        OnUpdateTick();
    }

    private void FixedUpdate()
    {
        OnFixedUpdateTick();
    }

    private void OnUpdateTick()
    {
        OnRun();

        _moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    private void OnFixedUpdateTick()
    {
        float speed = _moveSpeed;
        if (_playerStats != null)
            speed *= _playerStats.MoveSpeedMultiplier;
        _rb.linearVelocity = _moveInput * speed;
    }
    #endregion

    #region Camera
    private void LateUpdate()
    {
        OnLateUpdateTick();
    }

    private void OnLateUpdateTick()
    {
        if (_cam == null) return;
        Vector3 target = transform.position + _camOffset;
        _cam.position = Vector3.Lerp(_cam.position, target, _camSmoothing * Time.deltaTime);
    }
    #endregion

    #region Running
    private void OnRun()
    {
        bool sprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);
        _moveSpeed = sprinting ? 5f * _runningMultiplier : 5f;
    }
    #endregion
}