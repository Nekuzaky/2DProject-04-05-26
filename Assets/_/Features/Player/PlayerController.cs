using UnityEngine;

[DefaultExecutionOrder(-10)]
public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Camera")]
    [SerializeField] private float camSmoothing = 6f;
    [SerializeField] private Vector3 camOffset = new Vector3(0f, 0f, -10f);

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Transform cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        cam = Camera.main?.transform;
    }

    private void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (moveInput.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        Vector3 target = transform.position + camOffset;
        cam.position = Vector3.Lerp(cam.position, target, camSmoothing * Time.deltaTime);
    }
}