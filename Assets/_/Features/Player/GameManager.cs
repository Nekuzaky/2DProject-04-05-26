using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("<color=cyan><b><size=15>Game State</size></b></color>")]
    [SerializeField] private bool _isPaused = false;

    private float _elapsedTime = 0f;
    private bool _isGameActive = true;

    public GameTimer Timer { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Timer = new GameTimer();
    }

    private void Start()
    {
        Debug.Log("<color=green><b>GameManager:</b></color> Ready");
        StartGame();
    }

    private void Update()
    {
        if (!_isGameActive || _isPaused) return;

        _elapsedTime += Time.deltaTime;
        Timer.UpdateTime(_elapsedTime);
    }

    #region Game Control
    public void StartGame()
    {
        _isGameActive = true;
        _elapsedTime = 0f;
        Timer.Reset();
    }

    public void StopGame()
    {
        _isGameActive = false;
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
    }

    public float GetElapsedTime() => _elapsedTime;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion
}

#region Timer Class
public class GameTimer
{
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public int Milliseconds { get; private set; }

    public void UpdateTime(float totalSeconds)
    {
        Minutes = Mathf.FloorToInt(totalSeconds / 60f);
        Seconds = Mathf.FloorToInt(totalSeconds % 60f);
        Milliseconds = Mathf.FloorToInt((totalSeconds * 100f) % 100f);
    }

    public void Reset()
    {
        Minutes = 0;
        Seconds = 0;
        Milliseconds = 0;
    }

    public override string ToString() => $"{Minutes:00}:{Seconds:00}:{Milliseconds:00}";
}
#endregion
