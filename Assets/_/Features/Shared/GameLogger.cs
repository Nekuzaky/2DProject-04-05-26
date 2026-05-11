// Zero-cost production logger.
// The [Conditional] attribute strips the CALL SITE at compile time:
//   → no string allocation, no function call, zero overhead in release builds.
// Active in:  Unity Editor + builds with the DEVELOPMENT_BUILD flag checked.
// Stripped in: standard release builds (GameJolt / itch.io / Steam).
//
// Usage:
//   GameLogger.Log("...");        // editor + dev builds only
//   GameLogger.LogWarning("..."); // editor + dev builds only
//   GameLogger.LogError("...");   // always visible (real runtime errors)
public static class GameLogger
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message) => UnityEngine.Debug.Log(message);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(string message) => UnityEngine.Debug.LogWarning(message);

    // Errors always show — they indicate real problems that must be fixed.
    public static void LogError(string message) => UnityEngine.Debug.LogError(message);
}
