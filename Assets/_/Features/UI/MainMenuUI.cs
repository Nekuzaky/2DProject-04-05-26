using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    #region Inspector Settings
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        EnsureEventSystemInputModule();
        ConfigureNavigation(_playButton);
        ConfigureNavigation(_quitButton);

        if (_playButton != null)
        {
            _playButton.onClick.AddListener(() => GameSceneManager.Instance.PlayGame());
            GameLogger.Log("<color=green><b>MainMenuUI:</b></color> Play Button Initialized");
        }

        if (_quitButton != null)
            _quitButton.onClick.AddListener(() => GameSceneManager.Instance.QuitGame());
    }

    private void Start()
    {
        SelectButton(_playButton != null ? _playButton : _quitButton);
    }

    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            SelectButton(_playButton != null ? _playButton : _quitButton);
    }

    private void EnsureEventSystemInputModule()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        BaseInputModule inputModule = eventSystem.GetComponent<BaseInputModule>();
        if (inputModule == null)
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
    }

    private static void ConfigureNavigation(Selectable selectable)
    {
        if (selectable == null)
            return;

        Navigation navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        selectable.navigation = navigation;
    }

    private static void SelectButton(Button button)
    {
        if (button == null || EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
    #endregion
}
