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
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Event Invocation
    private void Update()
    {
        OnUpdate?.Invoke();
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
