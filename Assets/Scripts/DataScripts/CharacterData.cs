using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public int characterID;
    public string characterName;
    public Sprite characterIcon;

    public int healthData;
    public int powerData;
    public int grazeData;
    public int bombsData;

    //Primary Constructor
    public CharacterData(CharacterData other)
    {
        characterID = other.characterID;
        characterName = other.characterName;
        healthData = other.healthData;
        powerData = other.powerData;
        grazeData = other.grazeData;
        bombsData = other.bombsData;
    }

    //Constructor Override backup
    public CharacterData() { }
}
