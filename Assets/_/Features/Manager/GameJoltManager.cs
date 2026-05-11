using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

// Standalone GameJolt API integration — no external package required.
// Uses UnityWebRequest + MD5 signing as per GameJolt API v1.2 spec.
//
// SETUP:
//   1. Attach this script to a persistent GameObject (e.g. GameManager).
//   2. Fill _gameId and _privateKey from your GameJolt dashboard:
//      https://gamejolt.com/dashboard → your game → API Settings
//   3. Trophies and score tables must be created in the dashboard first.
//
// HOW IT WORKS (WebGL on GameJolt):
//   When a player opens your game on GameJolt.com, the URL contains
//   ?gjapi_username=xxx&gjapi_token=xxx — this script reads those
//   automatically and authenticates the player silently.
//   If the game is run outside GameJolt (downloaded build), auth is
//   skipped gracefully — features are simply disabled.
public class GameJoltManager : MonoBehaviour
{
    #region Singleton
    public static GameJoltManager Instance { get; private set; }
    #endregion

    #region Inspector Settings
    [Header("<color=cyan><b><size=15>GameJolt Credentials</size></b></color>")]
    [Tooltip("Your Game ID from GameJolt Dashboard → API Settings.")]
    [SerializeField] private int    _gameId     = 0;

    [Tooltip("Your Private Key from GameJolt Dashboard → API Settings.")]
    [SerializeField] private string _privateKey = "";

    [Header("<color=yellow><b><size=15>Session</size></b></color>")]
    [Tooltip("Seconds between session pings (keep < 120 or GameJolt closes the session).")]
    [SerializeField] private float _sessionPingInterval = 30f;
    #endregion

    #region Constants
    private const string BASE_URL = "https://api.gamejolt.com/api/game/v1_2";
    #endregion

    #region State
    private string _username;
    private string _userToken;
    private bool   _isAuthenticated;
    private bool   _sessionOpen;
    #endregion

    #region Properties
    public bool   IsAuthenticated => _isAuthenticated;
    public string Username        => _username;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_gameId == 0 || string.IsNullOrEmpty(_privateKey))
        {
            GameLogger.LogWarning("GameJoltManager: Game ID or Private Key not set — fill them in the Inspector.");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL on GameJolt.com: credentials come from the URL query string.
        StartCoroutine(InitFromURL());
#else
        // Standalone: show a login dialog so the player can authenticate.
        GameJoltLoginUI.Show(this, OnStandaloneAuth);
#endif
    }

    // Called by GameJoltLoginUI once the player authenticates (or skips).
    private void OnStandaloneAuth(bool authenticated)
    {
        if (!authenticated)
        {
            GameLogger.Log("GameJoltManager: Playing as guest — GameJolt features disabled.");
            return;
        }

        GameLogger.Log($"GameJoltManager: Authenticated as '{_username}'.");
        StartCoroutine(OpenSession());
        InvokeRepeating(nameof(PingSession), _sessionPingInterval, _sessionPingInterval);
    }

    private void OnApplicationQuit()
    {
        if (_sessionOpen)
            StartCoroutine(CloseSession());
    }
    #endregion

    #region Initialization
    // In WebGL on GameJolt.com, credentials are passed as URL query params.
    private IEnumerator InitFromURL()
    {
        string url      = Application.absoluteURL;
        string username = GetURLParam(url, "gjapi_username");
        string token    = GetURLParam(url, "gjapi_token");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            GameLogger.Log("GameJoltManager: No credentials in URL — game may be running outside GameJolt.");
            yield break;
        }

        yield return StartCoroutine(AuthenticateCoroutine(username, token, success =>
        {
            if (!success) return;

            GameLogger.Log($"GameJoltManager: Authenticated as '{_username}'.");
            StartCoroutine(OpenSession());
            InvokeRepeating(nameof(PingSession), _sessionPingInterval, _sessionPingInterval);
        }));
    }

    private static string GetURLParam(string url, string paramName)
    {
        int queryStart = url.IndexOf('?');
        if (queryStart < 0) return null;

        string[] pairs = url.Substring(queryStart + 1).Split('&');
        foreach (string pair in pairs)
        {
            string[] kv = pair.Split('=');
            if (kv.Length == 2 && kv[0] == paramName)
                return UnityWebRequest.UnEscapeURL(kv[1]);
        }
        return null;
    }
    #endregion

    #region Authentication
    public void Authenticate(string username, string token, System.Action<bool> callback)
        => StartCoroutine(AuthenticateCoroutine(username, token, callback));

    private IEnumerator AuthenticateCoroutine(string username, string token, System.Action<bool> callback)
    {
        string url = BuildURL("/users/auth/", $"username={username}&user_token={token}");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        bool success = req.result == UnityWebRequest.Result.Success
                    && req.downloadHandler.text.Contains("\"success\":\"true\"");

        if (success)
        {
            _username        = username;
            _userToken       = token;
            _isAuthenticated = true;
        }

        callback?.Invoke(success);
    }
    #endregion

    #region Trophies
    // Call after a player achievement — trophyId from your GameJolt dashboard.
    public void UnlockTrophy(int trophyId)
    {
        if (!_isAuthenticated) return;
        StartCoroutine(UnlockTrophyCoroutine(trophyId));
    }

    private IEnumerator UnlockTrophyCoroutine(int trophyId)
    {
        string url = BuildURL("/trophies/add-achieved/",
            $"username={_username}&user_token={_userToken}&trophy_id={trophyId}");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        GameLogger.Log($"GameJoltManager: Trophy {trophyId} → {req.downloadHandler.text}");
    }
    #endregion

    #region Scores
    // score     = numeric sort value (e.g. kill count)
    // scoreText = display string shown on leaderboard (e.g. "42 kills")
    // tableId   = 0 → primary table; set to a specific ID for secondary tables
    public void SubmitScore(int score, string scoreText, int tableId = 0)
    {
        if (!_isAuthenticated) return;
        StartCoroutine(SubmitScoreCoroutine(score, scoreText, tableId));
    }

    private IEnumerator SubmitScoreCoroutine(int score, string scoreText, int tableId)
    {
        string tableParam = tableId > 0 ? $"&table_id={tableId}" : "";
        string url = BuildURL("/scores/add/",
            $"username={_username}&user_token={_userToken}" +
            $"&score={UnityWebRequest.EscapeURL(scoreText)}&sort={score}{tableParam}");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        GameLogger.Log($"GameJoltManager: Score '{scoreText}' ({score}) submitted → {req.downloadHandler.text}");
    }
    #endregion

    #region Sessions
    private IEnumerator OpenSession()
    {
        string url = BuildURL("/sessions/open/",
            $"username={_username}&user_token={_userToken}");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        _sessionOpen = req.result == UnityWebRequest.Result.Success;
        GameLogger.Log($"GameJoltManager: Session open → {req.downloadHandler.text}");
    }

    private void PingSession()
    {
        if (_isAuthenticated && _sessionOpen)
            StartCoroutine(PingSessionCoroutine());
    }

    private IEnumerator PingSessionCoroutine()
    {
        string url = BuildURL("/sessions/ping/",
            $"username={_username}&user_token={_userToken}&status=active");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
    }

    private IEnumerator CloseSession()
    {
        string url = BuildURL("/sessions/close/",
            $"username={_username}&user_token={_userToken}");

        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        _sessionOpen = false;
    }
    #endregion

    #region URL Signing
    // GameJolt API v1.2: append all params, then append MD5(full_url + private_key)
    private string BuildURL(string endpoint, string parameters)
    {
        string url  = $"{BASE_URL}{endpoint}?game_id={_gameId}&{parameters}&format=json";
        string sig  = ComputeMD5(url + _privateKey);
        return $"{url}&signature={sig}";
    }

    private static string ComputeMD5(string input)
    {
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new StringBuilder(32);
        foreach (byte b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
    #endregion
}
