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
            characterID = 0,
            characterName = "Reimu",
            healthData = 100,
            powerData = 100,
            grazeData = 0,
            bombsData = 3
        });

        Data.partyMembers.Add(new CharacterData
        {
            characterID = 1,
            characterName = "Marisa",
            healthData = 50,
            powerData = 2,
            grazeData = 10,
            bombsData = 2
        });
    }
}
