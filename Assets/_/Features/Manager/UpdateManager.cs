using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance { get; private set; }

    public event System.Action OnUpdate;
    public event System.Action OnLateUpdate;
    public event System.Action OnFixedUpdate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
}
