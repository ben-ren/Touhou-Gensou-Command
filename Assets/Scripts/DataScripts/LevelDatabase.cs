public static class LevelDatabase
{
    private static readonly string[] scenes =
    {
        "MapView_1",
        "MapScene_2",
        "MapScene_3"
    };

    public static string GetSceneName(int index)
    {
        if (index < 0 || index >= scenes.Length)
            return scenes[0]; // fallback

        return scenes[index];
    }

    public static int GetLevelCount()
    {
        return scenes.Length;
    }
}