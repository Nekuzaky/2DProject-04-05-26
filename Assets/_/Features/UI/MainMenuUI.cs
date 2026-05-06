using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private void Awake()
    {
        if (_playButton != null)
            _playButton.onClick.AddListener(() => GameSceneManager.Instance.PlayGame());
            Debug.Log("<color=green><b>MainMenuUI:</b></color> Play Button Initialized");

        if (_quitButton != null)
            _quitButton.onClick.AddListener(() => GameSceneManager.Instance.QuitGame());
            
    }
}
