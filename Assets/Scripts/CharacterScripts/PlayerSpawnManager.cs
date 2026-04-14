using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform spawnPoint;

    [Header("Character Prefabs")]
    public List<GameObject> characterPrefabs;

    [Header("Camera Prefabs")]
    public GameObject cameraRigPrefab;

    [Header("UI Access")]
    public GameViewUserInterface UI;

    [Header("Debug")]
    public bool useDebugCharacter = false;
    public int debugCharacterID = 0;

    private GameObject spawnedCharacter;

    void Start()
    {
        SpawnPlayer();
        SpawnCamera();
        BindUI();
    }

    void BindUI()
    {
        if (UI == null)
        {
            Debug.LogWarning("UI reference not set in inspector.");
            return;
        }

        EntitySystems entity = spawnedCharacter.GetComponent<EntitySystems>();

        if (entity == null)
        {
            Debug.LogError("Spawned character is missing EntitySystems component.");
            return;
        }

        UI.SetPlayer(entity);
    }

    void SpawnPlayer()
    {
        int id;

        // -----------------------------
        // Determine character source
        // -----------------------------
        if (useDebugCharacter)
        {
            id = debugCharacterID;
            Debug.Log($"[DEBUG] Forcing character ID: {id}");
        }
        else
        {
            CharacterData selectedCharacter =
                GameState.Instance.Data.selectedCharacter;

            if (selectedCharacter == null)
            {
                Debug.LogError("No selected character found.");
                return;
            }

            id = selectedCharacter.characterID;
        }

        // -----------------------------
        // Validate ID
        // -----------------------------
        if (id < 0 || id >= characterPrefabs.Count)
        {
            Debug.LogError($"Invalid character ID: {id}");
            return;
        }

        // -----------------------------
        // Spawn player
        // -----------------------------
        spawnedCharacter = Instantiate(
            characterPrefabs[id],
            spawnPoint.position,
            Quaternion.identity
        );
    }

    void SpawnCamera()
    {
        if (spawnedCharacter == null)
        {
            Debug.LogError("Player not spawned, cannot spawn camera.");
            return;
        }

        GameObject cam = Instantiate(
            cameraRigPrefab,
            spawnedCharacter.transform.position + new Vector3(0f, 1f, -10f),
            spawnedCharacter.transform.rotation
        );

        CameraController controller = cam.GetComponent<CameraController>();

        controller.SetPlayer(spawnedCharacter.transform);
    }
}