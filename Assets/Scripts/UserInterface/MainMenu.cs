using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // For loading next scene

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    private UIDocument _document;
    private List<Button> _buttons;
    private Dictionary<string, System.Action> _buttonActions;
    private AudioSource _audioSource;

    //----------Menu Containers---------------
    private VisualElement _mainMenu;
    private VisualElement _loadMenu;
    private VisualElement _settingsMenu;
    private VisualElement _galleryMenu;
    private VisualElement _confirmQuitMenu;
    private Button _backButton;

    private void Awake()
    {
        // Get components
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        // Get root element
        var root = _document.rootVisualElement;

        // Cache all menu containers
        _mainMenu        = root.Q<VisualElement>("MainMenu");
        _loadMenu        = root.Q<VisualElement>("LoadMenu");
        _settingsMenu    = root.Q<VisualElement>("SettingsMenu");
        _galleryMenu     = root.Q<VisualElement>("Gallery");
        _confirmQuitMenu = root.Q<VisualElement>("ConfirmQuit");
        _backButton      = root.Q<Button>("BackButton");

        // Cache all buttons
        _buttons = root.Query<Button>().ToList();

        // Map buttons to actions
        _buttonActions = new Dictionary<string, System.Action>()
        {
            { "PlayButton", OnPlayClicked },
            { "LoadButton", () => ShowMenu(_loadMenu) },
            { "SettingsButton", () => ShowMenu(_settingsMenu) },
            { "GalleryButton", () => ShowMenu(_galleryMenu) },
            { "QuitButton", () => ShowMenu(_confirmQuitMenu) },

            { "BackButton", () => ShowMenu(_mainMenu) },
            { "ConfirmQuitButton", OnQuitClicked }
        };

        // Register the same callback for all buttons
        foreach (var button in _buttons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }

        // Ensure starting state
        ShowMenu(_mainMenu);
    }

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
        _audioSource?.Play();

        if (e.currentTarget is Button button && _buttonActions.TryGetValue(button.name, out var action))
        {
            action.Invoke();
        }
    }

    // Hides all menu containers
    private void HideAllMenus()
    {
        _mainMenu.AddToClassList("hidden");
        _loadMenu.AddToClassList("hidden");
        _settingsMenu.AddToClassList("hidden");
        _galleryMenu.AddToClassList("hidden");
        _confirmQuitMenu.AddToClassList("hidden");
    }

    // Shows the selected menu and handles Back button visibility
    private void ShowMenu(VisualElement menu)
    {
        HideAllMenus();
        menu.RemoveFromClassList("hidden");

        // Only show Back button if not MainMenu
        if (menu == _mainMenu)
            _backButton.AddToClassList("hidden");
        else
            _backButton.RemoveFromClassList("hidden");
    }

    //---------- Button Actions -------------

    private void OnPlayClicked()
    {
        // Example: load next scene (replace with your scene index or name)
        SceneManager.LoadScene("GameScene");
    }

    private void OnLoadClicked()
    {
        
    }

    private void OnSettingsClicked()
    {
        
    }

    private void OnGalleryClicked()
    {
        
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
