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
    public int[,] currentTileGrid;
    public List<PrefabStruct> prefabStructs;
    public int currentLevelIndex;
    public int turnNumber = 0;      //turn counter. Starts at turn 0.

    // Which levels are unlocked
    public List<LevelUnlockData> unlockedLevels = new();

    // Entity positions on the map scene saved by ID
    public Dictionary<string, Vector3> entityPositions = new();

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
    Dictionary<CharacterData, bool> allCharacters = new ();
}
