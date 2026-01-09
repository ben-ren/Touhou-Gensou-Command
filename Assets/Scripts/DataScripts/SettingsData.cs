using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    // Graphics
    public int resolutionIndex;
    public int qualityIndex;
    public int displayModeIndex;

    // Audio (0–100)
    public int masterVolume;
    public int sfxVolume;
    public int musicVolume;
    public int uiVolume;

    // Controls
    public int controlSchemeIndex;
    public float mouseSensitivity;
    public float joystickSensitivity;

    public static SettingsData Default()
    {
        return new SettingsData()
        {
            // Graphics
            resolutionIndex     = 0,                // safe fallback
            qualityIndex        = QualitySettings.GetQualityLevel(),
            displayModeIndex    = 1,                // Borderless

            // Audio (reasonable defaults)
            masterVolume = 0,
            sfxVolume    = -10,
            musicVolume  = -10,
            uiVolume     = -10,

            // Controls
            controlSchemeIndex = 0,              // Cursor Follow
            mouseSensitivity   = 0.5f,
            joystickSensitivity= 0.5f
        };
    }
}
