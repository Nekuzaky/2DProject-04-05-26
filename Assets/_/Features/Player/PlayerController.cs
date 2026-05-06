using UnityEngine;



[DefaultExecutionOrder(-10)]
public class PlayerController : MonoBehaviour
{
    [Header("<color=orange><b><size=15>Movement</size></b></color>")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runningMultiplier = 1.5f;

    [Header("<color=cyan><b><size=15>Visual</size></b></color>")]
    [SerializeField] private Transform _playerSprite;

    [Header("<color=grey><b><size=15>Camera</size></b></color>")]
    [SerializeField] private float _camSmoothing = 6f;
    [SerializeField] private Vector3 _camOffset = new Vector3(0f, 0f, -10f);

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Transform _cam;
    private Camera _mainCamera;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _cam = Camera.main?.transform;
        _mainCamera = Camera.main;
    }
    private void Start()
    {
        Debug.Log("<color=green><b>PlayerController:</b></color> Ready");
    }

    private void Update()
    {
        OnUpdateTick();
    }

    private void FixedUpdate()
    {
        OnFixedUpdateTick();
    }

    private void LateUpdate()
    {
        OnLateUpdateTick();
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
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }

    private void OnLateUpdateTick()
    {
        if (_cam == null) return;
        Vector3 target = transform.position + _camOffset;
        _cam.position = Vector3.Lerp(_cam.position, target, _camSmoothing * Time.deltaTime);
    }

    private void OnRun()
    {
        bool sprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);
        _moveSpeed = sprinting ? 5f * _runningMultiplier : 5f;
    }
}