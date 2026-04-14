using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform spawnPoint;

    [Header("Character Prefabs")]
    public List<GameObject> characterPrefabs;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        CharacterData selectedCharacter =
            GameState.Instance.Data.selectedCharacter;

        if (selectedCharacter == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        int id = selectedCharacter.characterID;

        if (id < 0 || id >= characterPrefabs.Count)
        {
            Debug.LogError("Invalid character ID.");
            return;
        }

        Instantiate(
            characterPrefabs[id],
            spawnPoint.position,
            Quaternion.identity
        );
    }
}
