using UnityEngine;

[RequireComponent(typeof(PickupItemDrugs))]
public class Weed : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=lime><b><size=15>Visual Effect</size></b></color>")]
    [Tooltip("Duration of the visual effects in seconds (can differ from the buff duration).")]
    [SerializeField] private float _visualDuration = 15f;
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        WeedEffect effect = WeedEffect.Instance != null ? WeedEffect.Instance : FindFirstObjectByType<WeedEffect>();

        if (effect != null)
            effect.Trigger(_visualDuration);
        else
            GameLogger.LogWarning("<color=yellow><b>Weed:</b></color> WeedEffect not found — add the component to your player camera.");
    }
    #endregion
}
