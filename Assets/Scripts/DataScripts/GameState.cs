using UnityEngine;

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

    private void BootstrapParty()
    {
        Data.partyMembers.Add(new CharacterData
        {
            characterName = "Reimu",
            isUnlocked = true,
            healthData = 100,
            powerData = 100,
            grazeData = 0,
            bombsData = 3
        });

        Data.partyMembers.Add(new CharacterData
        {
            characterName = "Marisa",
            isUnlocked = true,
            healthData = 50,
            powerData = 2,
            grazeData = 10,
            bombsData = 2
        });
    }
}
