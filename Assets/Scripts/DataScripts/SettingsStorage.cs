using UnityEngine;

public static class SettingsStorage
{
    private const string KEY = "SETTINGS_DATA";

    public static void Save(SettingsData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static SettingsData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return SettingsData.Default(); // defaults

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<SettingsData>(json);
    }
}
