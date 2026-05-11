using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    #region Singleton
    public static UpdateManager Instance { get; private set; }
    #endregion

    #region Events
    public event System.Action OnUpdate;
    public event System.Action OnLateUpdate;
    public event System.Action OnFixedUpdate;
    // Fires every 3 frames — use for distance culling, non-critical AI, ambient effects, etc.
    public event System.Action OnSlowUpdate;
    #endregion

    #region State
    private int _frameCount;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Event Invocation
    private void Update()
    {
        OnUpdate?.Invoke();

        if ((_frameCount++ % 3) == 0)
            OnSlowUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
    #endregion
}
