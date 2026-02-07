using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public string characterName;
    public Sprite characterIcon;

    public bool isUnlocked;

    public int healthData;
    public int powerData;
    public int grazeData;
    public int bombsData;
}
