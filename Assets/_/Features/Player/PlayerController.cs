using UnityEngine;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Settings - Movement
    [Header("<color=orange><b><size=15>Movement</size></b></color>")]
    [SerializeField] private float _moveSpeed        = 5f;
    [SerializeField] private float _runningMultiplier = 1.5f;
    #endregion

    #region Inspector Settings - Camera
    [Header("<color=grey><b><size=15>Camera</size></b></color>")]
    [SerializeField] private float   _camSmoothing = 6f;
    [SerializeField] private Vector3 _camOffset    = new Vector3(0f, 0f, -10f);
    #endregion

    #region State
    private Rigidbody2D  _rb;
    private Vector2      _moveInput;
    private Transform    _cam;
    private PlayerStats  _playerStats;
    private float        _baseSpeed;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _rb              = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _cam             = Camera.main?.transform;
        _playerStats     = GetComponent<PlayerStats>();
        _baseSpeed       = _moveSpeed;
    }
    #endregion

    #region Update Loop
    private void Update()       => ReadInput();
    private void FixedUpdate()  => ApplyMovement();
    private void LateUpdate()   => FollowCamera();
    #endregion

    #region Input
    private void ReadInput()
    {
        if (InputHandler.Instance != null)
        {
            _moveInput = InputHandler.Instance.MoveDirection;
            bool sprinting = InputHandler.Instance.IsSprinting;
            _moveSpeed = sprinting ? _baseSpeed * _runningMultiplier : _baseSpeed;
        }
        else
        {
            // Fallback: direct legacy input (if InputHandler isn't in the scene)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _moveInput = new Vector2(h, v).normalized;

            bool sprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);
            _moveSpeed = sprinting ? _baseSpeed * _runningMultiplier : _baseSpeed;
        }
    }
    #endregion

    #region Movement
    private void ApplyMovement()
    {
        float speed = _moveSpeed;
        if (_playerStats != null)
            speed *= _playerStats.MoveSpeedMultiplier;

        _rb.linearVelocity = _moveInput * speed;
    }
    #endregion

    #region Camera
    private void FollowCamera()
    {
        if (_cam == null) return;
        Vector3 target = transform.position + _camOffset;
        _cam.position  = Vector3.Lerp(_cam.position, target, _camSmoothing * Time.deltaTime);
    }
    #endregion
}
