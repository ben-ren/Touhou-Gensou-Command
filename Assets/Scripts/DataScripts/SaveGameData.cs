using System;
using System.Collections.Generic;


/// <summary>
/// SAVE FILE ONLY
/// This is the serializable version of persistent game progress.
///
/// IMPORTANT:
/// This is NOT runtime GameData.
/// This is ONLY what should be written to disk.
///
/// We intentionally exclude:
/// - GameObjects
/// - Terrain prefabs
/// - Scene references
/// - Runtime-only encounter generation data
/// </summary>
[Serializable]
public class SaveGameData
{
    public string saveDateTime;
    
    //--------------------------------------
    // Persistent Resources
    //--------------------------------------
    public int money;
    public int fuel;
    public int orbs;
    public int missiles;
    public int lives;

    //--------------------------------------
    // World Progression
    //--------------------------------------
    public int turnNumber;
    public int currentLevelIndex;

    public int totalRequiredOrbs;
    public int requiredEncounterOrbs;

    //--------------------------------------
    // Persistent Player Progress
    //--------------------------------------
    public List<CharacterData> partyMembers = new();
    public List<LevelUnlockData> unlockedLevels = new();
    public List<EntityStateData> entityStates = new();
    public List<ItemStateData> itemStates = new();
}