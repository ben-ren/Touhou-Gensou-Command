using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // For returning to main menu

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    public UIDocument _userInterface;
    private UIController UIC;
    private UIDocument _document;

    private List<Button> _buttons;
    private Dictionary<string, System.Action> _buttonActions;

    //-------------- Menu elements --------------------
    private VisualElement _pauseScreen;
    private VisualElement _pauseMenu;
    private VisualElement _settingsMenu;
    private VisualElement _confirmQuit;

    //-------------- Menu Scripts --------------------
    SettingsMenu settingsMenuUI;

    //--------------Pause Controls---------------------
    private bool isPaused;
    private bool pauseConsumed;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        // Get root element
        var root = _document.rootVisualElement;

        //Get UI control reference
        settingsMenuUI = GetComponent<SettingsMenu>();

        //get UI visual elements reference
        _pauseScreen    = root.Q<VisualElement>("PauseContainer");
        _pauseMenu      = root.Q<VisualElement>("PauseMenu");
        _settingsMenu   = root.Q<VisualElement>("SettingsMenu");
        _confirmQuit    = root.Q<VisualElement>("ConfirmQuit");

        settingsMenuUI.enabled = false;

        // Cache all buttons
        _buttons = root.Query<Button>().ToList();

        // Map buttons to actions
        _buttonActions = new Dictionary<string, System.Action>()
        {
            { "ResumeButton", OnResumeClicked },
            { "SettingsButton", () => OnSettingsClicked() },
            { "QuitButton", () => OnQuitClicked() },
            { "YesButton", () => OnYesClicked() },
            { "NoButton", () => OnNoClicked() },
            { "BackButton", () => OnBackClicked() },
        };

        // Register the same callback for all buttons
        foreach (var button in _buttons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }

        _pauseScreen.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        UIC = UIController.instance;

        //-----pause control------
        isPaused = false;
        pauseConsumed = false;
    }

    void Update()
    {
        PauseGame();
        if(_userInterface != null){
            _userInterface.enabled = !isPaused;
        }
    }

    // ==========================================================
    //                  Callback Operations 
    // ==========================================================
    private void OnDisable()
    {
        foreach (var button in _buttons)
        {
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    // Central button click handler
    private void OnAllButtonsClick(ClickEvent e)
    {
        //_audioSource?.Play();

        if (e.currentTarget is Button button && _buttonActions.TryGetValue(button.name, out var action))
        {
            action.Invoke();
        }
    }

    // Hides all menu containers
    private void HideAllMenus()
    {
        _pauseMenu.AddToClassList("hidden");
        _settingsMenu.AddToClassList("hidden");
        _confirmQuit.AddToClassList("hidden");
    }

    // Shows the selected menu and handles Back button visibility
    public void ShowMenu(VisualElement menu)
    {
        HideAllMenus();
        menu.RemoveFromClassList("hidden");
    }

    // ==========================================================
    //                  Pause Functions 
    // ==========================================================

    public bool IsPaused()
    {
        return isPaused;
    }

    void PauseGame()
    {
        if (UIC.GetPause() > 0 && !pauseConsumed)
        {
            TogglePause();
            pauseConsumed = true;
        }
        else if (UIC.GetPause() <= 0)
        {
            pauseConsumed = false;
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        _pauseScreen.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;

        if (isPaused){
            ShowMenu(_pauseMenu);
        }
    }

    // ==========================================================
    //                  Callback Functions 
    // ==========================================================

    void OnResumeClicked()
    {
        TogglePause();
    }
    
    void OnSettingsClicked()
    {
        settingsMenuUI.enabled = true;
        ShowMenu(_settingsMenu);
        settingsMenuUI.SetReturnMenu(_pauseMenu);
    }

    void OnQuitClicked()
    {
        ShowMenu(_confirmQuit);
    }

    void OnYesClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void OnNoClicked()
    {
        ShowMenu(_pauseMenu);
    }

    void OnBackClicked()
    {
        ShowMenu(_pauseMenu);
    }
}
