using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=green><b><size=15>Health Pickup</size></b></color>")]
    [SerializeField] private int _healAmount = 25;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out EntityHealth playerHealth))
        {
            playerHealth.Heal(_healAmount);
            Destroy(gameObject);
        }
    }
    #endregion
}
