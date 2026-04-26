using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class CharacterDatabase
{
    private static readonly Dictionary<int, CharacterData> characters = new()
    {
        {0, new CharacterData
            {
                characterID = 0,
                characterName = "Reimu",
                healthData = 100,
                powerData = 100,
                grazeData = 0,
                bombsData = 3
            }},
        {1, new CharacterData
            {
                characterID = 1,
                characterName = "Marisa",
                healthData = 50,
                powerData = 2,
                grazeData = 10,
                bombsData = 2
            }},
    };
    
    //function that returns CharacterData based on id input
    public static CharacterData GetCharacter(int id)
    {
        if (characters.TryGetValue(id, out var character))
            return new CharacterData(character);

        Debug.LogWarning($"Character {id} not found");
        return null;
    }

    public static IReadOnlyDictionary<int, CharacterData> GetAllCharacters()
    {
        return characters;
    }
}
