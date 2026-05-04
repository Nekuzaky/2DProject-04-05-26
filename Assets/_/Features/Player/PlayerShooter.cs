using UnityEngine;


public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Stats")]
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime = 0f;

    private void Update()
    {
        bool fire = Input.GetMouseButton(0) || Input.GetButton("Fire1");
        if (fire && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.Launch(firePoint.right);
        }
        else if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = firePoint.right * 10f;
        }
    }
}