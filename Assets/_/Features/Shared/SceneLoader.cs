using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [Header("<color=orange><b><size=15>Scene Names</size></b></color>")]
    [SerializeField] private string _gameScene = "SampleScene";
    [SerializeField] private string _mainMenuScene = "MainMenu";
    [SerializeField] private string _gameOverScene = "GameOver";
    public void LoadGameScene()
    { 
        LoadScene(_gameScene);
    }

    public void LoadMainMenuScene()
    {
        LoadScene(_mainMenuScene);
        
    }

    public void LoadGameOverScene()
    {
        LoadScene(_gameOverScene);
    }

    private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

