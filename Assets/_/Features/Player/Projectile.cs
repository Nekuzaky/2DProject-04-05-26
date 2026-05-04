using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{

    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private float _speed = 10f;


    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    public void Launch(Vector2 direction)
    {
        if (_rb != null)
        {
            _rb.linearVelocity = direction * _speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject)
            {
              
            }

            Destroy(gameObject, 3.0f);

        }
    }
}