using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("<color=cyan><b><size=15>Game State</size></b></color>")]
    [SerializeField] private bool isPaused = false;

    private float elapsedTime = 0f;
    private bool isGameActive = true;

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
        if (!isGameActive || isPaused) return;

        elapsedTime += Time.deltaTime;
        Timer.UpdateTime(elapsedTime);
    }
    #region game control methods
    public void StartGame()
    {
        isGameActive = true;
        elapsedTime = 0f;
        Timer.Reset();
    }

    public void StopGame()
    {
        isGameActive = false;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
#endregion 
#region timer class
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

    public override string ToString()
    {
        return $"{Minutes:00}:{Seconds:00}:{Milliseconds:00}";
    }
    #endregion 
}
