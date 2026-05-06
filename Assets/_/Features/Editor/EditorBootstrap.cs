using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorBootstrap
{
    static EditorBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        if (EditorBuildSettings.scenes.Length == 0) return;

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
    }
}
