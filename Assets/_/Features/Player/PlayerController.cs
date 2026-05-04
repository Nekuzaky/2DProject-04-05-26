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

        OnRun();

        _moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

  
        if (_playerSprite != null && _mainCamera != null)
        {
            Vector2 aimDirection = GetAimDirection();
            
            if (aimDirection.x > 0)
                _playerSprite.localScale = new Vector3(1, 1, 1);
            else if (aimDirection.x < 0)
                _playerSprite.localScale = new Vector3(-1, 1, 1);
        }
    }

    private Vector2 GetAimDirection()
    {
        
        try
        {
            float rx = Input.GetAxisRaw("RightStickHorizontal");
            float ry = Input.GetAxisRaw("RightStickVertical");
            Vector2 stick = new Vector2(rx, ry);
            if (stick.magnitude > 0.2f)
                return stick.normalized;
        }
        catch (System.ArgumentException)
        {
           
        }

      
        Vector2 mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorld - (Vector2)transform.position;
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