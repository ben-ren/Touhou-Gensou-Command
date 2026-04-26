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
}
