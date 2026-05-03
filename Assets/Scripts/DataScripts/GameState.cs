using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public GameData Data { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Data = new GameData();

        BootstrapParty();
    }

    public void AdvanceTurn()
    {
        Data.turnNumber++;
        Debug.Log("Turn: " + Data.turnNumber);
    }

    private void BootstrapParty()
    {
        Data.partyMembers.Add(CharacterDatabase.GetCharacter(0));
        Data.partyMembers.Add(CharacterDatabase.GetCharacter(1));
    }

    // =====================================================
    // NEW: CREATE SAVE DATA
    // =====================================================

    /// <summary>
    /// Converts runtime GameData into save-safe SaveGameData
    /// </summary>
    public SaveGameData CreateSaveData()
    {
        return new SaveGameData
        {
            saveDateTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
            money = Data.money,
            fuel = Data.fuel,
            orbs = Data.orbs,
            missiles = Data.missiles,
            lives = Data.lives,

            turnNumber = Data.turnNumber,
            currentLevelIndex = Data.currentLevelIndex,

            totalRequiredOrbs = Data.totalRequiredOrbs,
            requiredEncounterOrbs = Data.requiredEncounterOrbs,

            // Copy lists safely
            partyMembers = new List<CharacterData>(Data.partyMembers),
            unlockedLevels = new List<LevelUnlockData>(Data.unlockedLevels),
            entityStates = new List<EntityStateData>(Data.entityStates),
            itemStates = new List<ItemStateData>(Data.itemStates)
        };
    }

    // =====================================================
    // NEW: LOAD FROM SAVE
    // =====================================================

    /// <summary>
    /// Applies loaded save data into active runtime GameData
    /// </summary>
    public void LoadFromSave(SaveGameData save)
    {
        if (save == null)
            return;

        Data.money = save.money;
        Data.fuel = save.fuel;
        Data.orbs = save.orbs;
        Data.missiles = save.missiles;
        Data.lives = save.lives;

        Data.turnNumber = save.turnNumber;
        Data.currentLevelIndex = save.currentLevelIndex;

        Data.totalRequiredOrbs = save.totalRequiredOrbs;
        Data.requiredEncounterOrbs = save.requiredEncounterOrbs;

        Data.partyMembers = save.partyMembers;
        Data.unlockedLevels = save.unlockedLevels;
        Data.entityStates = save.entityStates;
        Data.itemStates = save.itemStates;

        Debug.Log("Save data loaded into GameState.");
    }

    public void ResetForNewGame()
    {
        Data = new GameData();

        BootstrapParty();

        Debug.Log("Fresh New Game Created");
    }
}
