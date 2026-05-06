using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=orange><b><size=15>Spawn Base Settings</size></b></color>")]
    [SerializeField] private int _baseMaxEnemiesPerSpawner = 10;

    [Header("<color=orange><b><size=15>Difficulty</size></b></color>")]
    [SerializeField] private int _killsPerDifficultyStep = 10;
    [SerializeField] private int _enemyCountIncreasePerStep = 5;
    [SerializeField] private float _spawnIntervalDecreasePerStep = 0.3f;
    [SerializeField] private float _minSpawnInterval = 0.5f;
    [SerializeField] private AnimationCurve _difficultyMultiplier =
        AnimationCurve.Linear(0f, 1f, 20f, 2f);
    [SerializeField] private float _minDifficultyMultiplier = 0.5f;
    [SerializeField] private float _maxDifficultyMultiplier = 5f;
    #endregion

    #region State
    private int _killCount;
    private int _difficultyLevel;
    private bool _gameOver;
    private float _elapsedTime;
    #endregion

    #region Properties
    public int KillCount => _killCount;
    public int DifficultyLevel => _difficultyLevel;
    public bool IsGameOver => _gameOver;
    public System.TimeSpan Timer => System.TimeSpan.FromSeconds(_elapsedTime);
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

    private void Start()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SetBaseMaxEnemies(_baseMaxEnemiesPerSpawner);
            EnemyManager.Instance.OnKillCountChanged += OnKillCountChanged;
        }

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null && player.TryGetComponent(out EntityHealth health)) // we listen for the player's death event so we can end the game when they die
            health.OnDeath.AddListener(OnPlayerDied);

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;

        ApplyDifficulty();
    }

    #endregion

    #region Game State
    private void OnUpdateTick()
    {
        if (!_gameOver)
            _elapsedTime += Time.deltaTime;
    }

    private void OnKillCountChanged(int totalKills)
    {
        _killCount = totalKills;

        int newDifficulty = _killCount / _killsPerDifficultyStep;
        if (newDifficulty > _difficultyLevel)
        {
            _difficultyLevel = newDifficulty;
            ApplyDifficulty();
        }
    }
    #endregion

    #region Difficulty System
    private void ApplyDifficulty() // this is where we calculate the new difficulty settings and apply them to the EnemyManager
    {
        if (EnemyManager.Instance == null) return;

        float difficultyMultiplier = GetDifficultyMultiplier(_difficultyLevel);
        int newMaxEnemies = Mathf.RoundToInt(_enemyCountIncreasePerStep * _difficultyLevel * difficultyMultiplier);  // we add to the base max enemies in EnemySpawner, so we only calculate the increase here
        float newSpawnInterval = Mathf.Max(
            _minSpawnInterval,
            EnemyManager.Instance.BaseSpawnInterval - (_spawnIntervalDecreasePerStep * _difficultyLevel * difficultyMultiplier)
        );

        EnemyManager.Instance.SetDifficulty(newMaxEnemies, newSpawnInterval);

        Debug.Log($"<color=orange><b>GameManager:</b></color> Difficulty level {_difficultyLevel} - Multiplier: {difficultyMultiplier:F2} - MaxEnemies: {newMaxEnemies}, SpawnInterval: {newSpawnInterval:F2}s");
    }

    private float GetDifficultyMultiplier(int difficultyLevel)
    {
        if (_difficultyMultiplier == null || _difficultyMultiplier.length == 0)
            return 1f;

        float rawMultiplier = _difficultyMultiplier.Evaluate(difficultyLevel);
        return Mathf.Clamp(rawMultiplier, _minDifficultyMultiplier, _maxDifficultyMultiplier);
    }
    #endregion
    #region Player Death 
    private void OnPlayerDied()
    {
        _gameOver = true;
        EnemyManager.Instance?.StopSpawning();
        GameRunSummary.Save(_killCount, _difficultyLevel, Timer);
        Debug.Log($"<color=red><b>GameManager:</b></color> Game Over - Kills: {_killCount}, Difficulty: {_difficultyLevel}");
        
        // Load game over scene with cached run summary
        if (GameSceneManager.Instance != null)
            GameSceneManager.Instance.LoadGameOver();
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.OnKillCountChanged -= OnKillCountChanged;

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion
}
