using UnityEngine;



[DefaultExecutionOrder(-10)]
public class PlayerController : MonoBehaviour
{
    [Header("<color=orange><b><size=15>Movement</size></b></color>")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runningMultiplier = 1.5f;

    [Header("<color=grey><b><size=15>Camera</size></b></color>")]
    [SerializeField] private float _camSmoothing = 6f;
    [SerializeField] private Vector3 _camOffset = new Vector3(0f, 0f, -10f);

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Transform _cam;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _cam = Camera.main?.transform;
    }
    private void Start()
    {
        Debug.Log("<color=green><b>PlayerController:</b></color> Ready");
    }
    private void Update()
    {

        OnRun();

        _moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

       
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }

    private void LateUpdate()
    {
        if (_cam == null) return;
        Vector3 target = transform.position + _camOffset;
        _cam.position = Vector3.Lerp(_cam.position, target, _camSmoothing * Time.deltaTime);
    }

    private void OnRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _moveSpeed = 5f * _runningMultiplier;
        }
        else
        {
            _moveSpeed = 5f;

        }
    }
}