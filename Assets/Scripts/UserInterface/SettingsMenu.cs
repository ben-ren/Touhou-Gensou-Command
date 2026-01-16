using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer _audioMixer;
    [Header("Optional")]
    private PlayerController player; // ⚡ optional

    private UIDocument _document;
    private SettingsData _settings;

    // ---------- Menu Containers ----------
    private VisualElement _graphicsSettings;
    private VisualElement _audioSettings;
    private VisualElement _controlsSettings;

    // ---------- Buttons ----------
    private Button _graphicsButton;
    private Button _audioButton;
    private Button _controlsButton;
    private Button _applyButton;
    private Button _resetButton;

    // ---------- Graphics ----------
    private DropdownField _displayResolution;
    private DropdownField _displayQuality;
    private DropdownField _displayMode;

    // ---------- Audio ----------
    private SliderInt _masterVolume;
    private SliderInt _sfxVolume;
    private SliderInt _musicVolume;
    private SliderInt _UIVolume;

    // ---------- Controls ----------
    private DropdownField _controlScheme;
    private Slider _mouseSensitivity;
    private Slider _joystickSensitivity;

    // ---------- Universal Return ----------
    private VisualElement _returnMenu; // the menu to return to after apply/reset

    // ======================================================
    void Awake()
    {
        if (player == null){
            player = FindFirstObjectByType<PlayerController>();
        }
        _document = GetComponent<UIDocument>();

        var root = _document.rootVisualElement;

        // Containers
        _graphicsSettings = root.Q<VisualElement>("GraphicsSettings");
        _audioSettings    = root.Q<VisualElement>("AudioSettings");
        _controlsSettings = root.Q<VisualElement>("ControlSettings");

        // Buttons
        _graphicsButton = root.Q<Button>("GraphicsSettingsButton");
        _audioButton    = root.Q<Button>("AudioSettingsButton");
        _controlsButton = root.Q<Button>("ControlSettingsButton");
        _applyButton    = root.Q<Button>("ApplyButton");
        _resetButton    = root.Q<Button>("ResetButton");

        // Graphics
        _displayResolution = root.Q<DropdownField>("DisplayResolution");
        _displayQuality    = root.Q<DropdownField>("DisplayQuality");
        _displayMode       = root.Q<DropdownField>("DisplayMode");

        // Audio
        _masterVolume = root.Q<SliderInt>("MasterVolumeSlider");
        _sfxVolume    = root.Q<SliderInt>("SFXVolumeSlider");
        _musicVolume  = root.Q<SliderInt>("MusicVolumeSlider");
        _UIVolume     = root.Q<SliderInt>("UIVolumeSlider");

        // Controls
        _controlScheme = root.Q<DropdownField>("ControlScheme");
        _mouseSensitivity = root.Q<Slider>("MouseSensitivitySlider");
        _joystickSensitivity = root.Q<Slider>("JoystickSensitivitySlider");

        RegisterButtonCallbacks();
        InitGraphicsSettings();
        InitControlSettings();

        ShowSettings(_graphicsSettings);
    }

    void Start()
    {
        LoadSettings();
        ApplyAll(); // apply immediately on startup
    }

    // ======================================================
    // MENU NAV
    // ======================================================
    private void RegisterButtonCallbacks()
    {
        _graphicsButton.clicked += () => ShowSettings(_graphicsSettings);
        _audioButton.clicked    += () => ShowSettings(_audioSettings);
        _controlsButton.clicked += () => ShowSettings(_controlsSettings);
        _applyButton.clicked    += ApplyChanges;
        _resetButton.clicked    += ResetToDefaults;
    }

    private void ShowSettings(VisualElement menu)
    {
        _graphicsSettings.style.display = DisplayStyle.None;
        _audioSettings.style.display    = DisplayStyle.None;
        _controlsSettings.style.display = DisplayStyle.None;

        menu.style.display = DisplayStyle.Flex;
    }

    public void SetReturnMenu(VisualElement returnToMenu = null)
    {
        _returnMenu = returnToMenu;
    }

    // ======================================================
    // GRAPHICS
    // ======================================================
    private void InitGraphicsSettings()
    {
        var resolutions = Screen.resolutions;

        _displayResolution.choices = resolutions
            .Select(r => $"{r.width}x{r.height}")
            .ToList();

        _displayQuality.choices = QualitySettings.names.ToList();

        _displayMode.choices = new List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        };
    }

    private void ApplyGraphics()
    {
        var res = Screen.resolutions[_settings.resolutionIndex];

        FullScreenMode mode = _settings.displayModeIndex switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.Windowed
        };

        Screen.SetResolution(res.width, res.height, mode);
        QualitySettings.SetQualityLevel(_settings.qualityIndex, true);
    }

    // ======================================================
    // AUDIO
    // ======================================================
    private void ApplyAudio()
    {
        _audioMixer.SetFloat("MasterVolume", _settings.masterVolume);
        _audioMixer.SetFloat("SFXVolume",    _settings.sfxVolume);
        _audioMixer.SetFloat("MusicVolume",  _settings.musicVolume);
        _audioMixer.SetFloat("UIVolume",     _settings.uiVolume);
    }

    // ======================================================
    // CONTROLS
    // ======================================================
    private void InitControlSettings()
    {
        _controlScheme.choices = new()
        {
            "Relative Input",   // 0
            "Cursor Follow"     // 1
        };
    }

    private void ApplyControls()
    {
        PlayerPrefs.SetInt("ControlScheme", _settings.controlSchemeIndex);
        PlayerPrefs.SetFloat("MouseSensitivity", _settings.mouseSensitivity);
        PlayerPrefs.SetFloat("JoystickSensitivity", _settings.joystickSensitivity);

        PlayerPrefs.Save();

        //Refresh player settings if PlayerController exists
        player?.RefreshSettings();
    }

    // ======================================================
    // APPLY + SAVE
    // ======================================================
    private void ApplyAll()
    {
        ApplyGraphics();
        ApplyAudio();
        ApplyControls();
    }

    private void ApplyChanges()
    {
        // Pull UI → data
        _settings.resolutionIndex = _displayResolution.index;
        _settings.qualityIndex    = _displayQuality.index;
        _settings.displayModeIndex= _displayMode.index;

        _settings.masterVolume = _masterVolume.value;
        _settings.sfxVolume    = _sfxVolume.value;
        _settings.musicVolume  = _musicVolume.value;
        _settings.uiVolume     = _UIVolume.value;

        _settings.controlSchemeIndex = _controlScheme.index;
        _settings.mouseSensitivity     = _mouseSensitivity.value;
        _settings.joystickSensitivity = _joystickSensitivity.value;

        ApplyAll();
        SettingsStorage.Save(_settings);

        player?.RefreshSettings();  //Safe player refresh

        // Return to parent menu if set
        if (_returnMenu != null)
        {
            _returnMenu.style.display = DisplayStyle.Flex;
        }
    }

    // ======================================================
    // LOAD
    // ======================================================
    private void LoadSettings()
    {
        _settings = SettingsStorage.Load();

        // Push data → UI
        _displayResolution.index = _settings.resolutionIndex;
        _displayQuality.index    = _settings.qualityIndex;
        _displayMode.index       = _settings.displayModeIndex;

        _masterVolume.value = _settings.masterVolume;
        _sfxVolume.value    = _settings.sfxVolume;
        _musicVolume.value  = _settings.musicVolume;
        _UIVolume.value     = _settings.uiVolume;

        _controlScheme.index = _settings.controlSchemeIndex;
        _mouseSensitivity.value = _settings.mouseSensitivity;
        _joystickSensitivity.value = _settings.joystickSensitivity;
    }

    private void ResetToDefaults()
    {
        _settings = SettingsData.Default();

        // Push defaults → UI
        _displayResolution.index = _settings.resolutionIndex;
        _displayQuality.index    = _settings.qualityIndex;
        _displayMode.index       = _settings.displayModeIndex;

        _masterVolume.value = _settings.masterVolume;
        _sfxVolume.value    = _settings.sfxVolume;
        _musicVolume.value  = _settings.musicVolume;
        _UIVolume.value     = _settings.uiVolume;

        _controlScheme.index      = _settings.controlSchemeIndex;
        _mouseSensitivity.value  = _settings.mouseSensitivity;
        _joystickSensitivity.value= _settings.joystickSensitivity;

        ApplyAll();
        SettingsStorage.Save(_settings);

        //Refresh player settings if PlayerController exists
        player?.RefreshSettings();

        if (_returnMenu != null)
        {
            _returnMenu.style.display = DisplayStyle.Flex;
        }
    }
}