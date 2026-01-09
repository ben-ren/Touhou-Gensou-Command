public static class SettingsManager
{
    private static SettingsData _cached;

    public static SettingsData Current
    {
        get
        {
            if (_cached == null)
                _cached = SettingsStorage.Load();
            return _cached;
        }
    }

    public static void Refresh()
    {
        _cached = SettingsStorage.Load();
    }
}
