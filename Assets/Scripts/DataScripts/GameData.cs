using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    //--------------------------------------
    // Party Members (dynamic)
    //--------------------------------------
    public List<CharacterData> partyMembers = new();
    //--------------------------------------
    // Persistent resources
    //--------------------------------------
    public int money;
    public int fuel;
    public int orbs;
    public int missiles;
    public int lives; // YES — include if your design wants persistence

    //--------------------------------------
    // World / map state
    //--------------------------------------
    public int currentLevelIndex;

    // Which levels are unlocked
    public List<LevelUnlockData> unlockedLevels = new();

    // Entity positions on the map scene saved by ID
    public List<EntityPositionData> entityPositions = new();

    // Pickups / items state
    public List<ItemStateData> itemStates = new();
    
    //--------------------------------------
    // Settings
    //--------------------------------------
    public SettingsData settingsData = new();

    //--------------------------------------
    //          Character Data
    //--------------------------------------
    //Global storage of CharacterData list
    List<CharacterData> allCharacters = new List<CharacterData>();
}
