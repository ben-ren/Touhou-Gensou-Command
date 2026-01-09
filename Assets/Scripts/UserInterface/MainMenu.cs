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
    [HideInInspector] public VisualElement _mainMenu;
    private VisualElement _loadMenu;
    private VisualElement _settingsMenu;
    private VisualElement _galleryMenu;
    private VisualElement _confirmQuitMenu;
    private Button _backButton;

    //------------Menu Scripts-----------------
    LoadMenu loadMenuUI;
    SettingsMenu settingsMenuUI;
    GalleryUI galleryUI;


    private void Awake()
    {
        // Get components
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        // Get root element
        var root = _document.rootVisualElement;

        //Get menu references
        loadMenuUI = GetComponent<LoadMenu>();
        settingsMenuUI = GetComponent<SettingsMenu>();
        galleryUI = GetComponent<GalleryUI>();

        //disable menus
        loadMenuUI.enabled = false;
        settingsMenuUI.enabled = false;
        galleryUI.enabled = false;

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
            { "LoadButton", () => OnLoadClicked() },
            { "SettingsButton", () => OnSettingsClicked() },
            { "GalleryButton", () => OnGalleryClicked() },
            { "QuitButton", () => ShowMenu(_confirmQuitMenu) },

            { "BackButton", () => OnBackClicked() },
            { "ConfirmQuitButton", OnQuitClicked }
        };

        // Register the same callback for all buttons
        foreach (var button in _buttons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnStart()
    {
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
    public void ShowMenu(VisualElement menu)
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
        SceneManager.LoadScene("TestRoom");
    }

    private void OnLoadClicked()
    {
        loadMenuUI.enabled = true;
        ShowMenu(_loadMenu);
    }

    private void OnSettingsClicked()
    {
        settingsMenuUI.enabled = true;
        ShowMenu(_settingsMenu); 
    }

    private void OnGalleryClicked()
    {
        galleryUI.enabled = true;
        ShowMenu(_galleryMenu);
    }

    private void OnBackClicked()
    {
        loadMenuUI.enabled = false;
        settingsMenuUI.enabled = false;
        galleryUI.enabled = false;
        ShowMenu(_mainMenu);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
