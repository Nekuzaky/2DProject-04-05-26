using UnityEngine;

[RequireComponent(typeof(PickupItemDrugs))]
[RequireComponent(typeof(CircleCollider2D))]
public class Lucy : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=magenta><b><size=15>Visual Effect</size></b></color>")]
    [Tooltip("Duration of the visual effects in seconds (can differ from the buff/malus duration set on PickupItemDrugs).")]
    [SerializeField] private float _visualDuration = 9f;
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        LucyEffect effect = LucyEffect.Instance != null ? LucyEffect.Instance : FindFirstObjectByType<LucyEffect>();

        if (effect != null)
            effect.Trigger(_visualDuration, other.transform);
        else
            Debug.LogWarning("<color=yellow><b>Lucy:</b></color> LucyEffect not found — add the component to your player camera.");
    }
    #endregion
}
