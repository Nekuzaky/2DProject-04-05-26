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

    #region Trophy IDs (GameJolt Dashboard → API → Trophies)
    private const int TrophyBronze   = 299407; // Rat Slayer      – Kill 10 enemies
    private const int TrophySilver   = 299408; // Exterminator    – Kill 50 enemies
    private const int TrophyGold     = 299409; // Nightmare Fuel  – Reach difficulty level 5
    private const int TrophyPlatinum = 299410; // Pure Nightmare  – Kill 100 enemies
    #endregion

    #region State
    private int _killCount;
    private int _difficultyLevel;
    private bool _gameOver;
    private bool _pendingGameOver;
    private float _elapsedTime;

    // Trophy unlock guards — each trophy is submitted at most once per run
    private bool _trophyBronzeUnlocked;
    private bool _trophySilverUnlocked;
    private bool _trophyGoldUnlocked;
    private bool _trophyPlatinumUnlocked;
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
            Destroy(this);
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

        if (_pendingGameOver)
        {
            _pendingGameOver = false;
            GameSceneManager.Instance?.LoadGameOver();
        }
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

        CheckTrophies();
    }

    private void CheckTrophies()
    {
        if (GameJoltManager.Instance == null || !GameJoltManager.Instance.IsAuthenticated) return;

        if (!_trophyBronzeUnlocked && _killCount >= 10)
        {
            _trophyBronzeUnlocked = true;
            GameJoltManager.Instance.UnlockTrophy(TrophyBronze);
            GameLogger.Log("<color=orange><b>GameManager:</b></color> Trophy unlocked — Rat Slayer (Bronze)");
        }
        if (!_trophySilverUnlocked && _killCount >= 50)
        {
            _trophySilverUnlocked = true;
            GameJoltManager.Instance.UnlockTrophy(TrophySilver);
            GameLogger.Log("<color=orange><b>GameManager:</b></color> Trophy unlocked — Exterminator (Silver)");
        }
        if (!_trophyGoldUnlocked && _difficultyLevel >= 5)
        {
            _trophyGoldUnlocked = true;
            GameJoltManager.Instance.UnlockTrophy(TrophyGold);
            GameLogger.Log("<color=orange><b>GameManager:</b></color> Trophy unlocked — Nightmare Fuel (Gold)");
        }
        if (!_trophyPlatinumUnlocked && _killCount >= 100)
        {
            _trophyPlatinumUnlocked = true;
            GameJoltManager.Instance.UnlockTrophy(TrophyPlatinum);
            GameLogger.Log("<color=orange><b>GameManager:</b></color> Trophy unlocked — Pure Nightmare (Platinum)");
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

        GameLogger.Log($"<color=orange><b>GameManager:</b></color> Difficulty level {_difficultyLevel} - Multiplier: {difficultyMultiplier:F2} - MaxEnemies: {newMaxEnemies}, SpawnInterval: {newSpawnInterval:F2}s");
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
        _gameOver        = true;
        _pendingGameOver = true;
        EnemyManager.Instance?.StopSpawning();
        GameRunSummary.Save(_killCount, _difficultyLevel, Timer);
        GameLogger.Log($"<color=red><b>GameManager:</b></color> Game Over - Kills: {_killCount}, Difficulty: {_difficultyLevel}");

        SubmitGameJoltScore();
    }

    private void SubmitGameJoltScore()
    {
        if (GameJoltManager.Instance == null || !GameJoltManager.Instance.IsAuthenticated) return;

        string scoreText = $"{_killCount} kills (lvl {_difficultyLevel})";
        GameJoltManager.Instance.SubmitScore(_killCount, scoreText);
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
