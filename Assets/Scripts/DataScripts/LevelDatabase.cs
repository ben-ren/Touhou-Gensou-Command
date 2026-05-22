using System.Collections.Generic;
using UnityEngine;

public static class LevelDatabase
{

    private static readonly LevelCard[] scenes =
    {
        new LevelCard("MapView_1", null),
        new LevelCard("MapScene_2", null),
        new LevelCard("MapScene_3", null)
    };

    public static string GetLevelName(int index)
    {
        if (index < 0 || index >= scenes.Length)
            return scenes[0].levelName; // fallback

        return scenes[index].levelName;
    }

    public static Sprite GetLevelImage(int index)
    {
        if (index < 0 || index >= scenes.Length)
            return scenes[0].levelSprite; // fallback
        return scenes[index].levelSprite;
    }

    public static int GetLevelCount()
    {
        return scenes.Length;
    }

    public static void SetIcons(Sprite[] sprites)
    {
        for(int i = 0; i < sprites.Length; i++)
        {
            scenes[i].levelSprite = sprites[i];
        }
    }

    public static LevelCard[] GetLevels()
    {
        return scenes;
    }
}

public class LevelCard
{
    public string levelName;
    public Sprite levelSprite;

    public LevelCard(string name, Sprite sprite)
    {
        levelName = name;
        levelSprite = sprite;
    }
}