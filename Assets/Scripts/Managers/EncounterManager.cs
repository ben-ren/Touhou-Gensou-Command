using UnityEngine;
using UnityEngine.Tilemaps; 
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class EncounterManager : MonoBehaviour
{
    [Header("References")]
    public Tilemap tilemap;

    [Header("Tile Assignment")]
    public List<TileBase> tileLookup;   //Manually assign tile index values
    [Tooltip("Assign Prefabs to relavant tiles via their index. For Example; A prefab with 'Tile Index' of 1 will spawn on ALL terrain segments associated with Tile Lookup's Element 1")]
    public List<PrefabStruct> tilesAssignedToPrefabs;

    String currentLevelMap;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        currentLevelMap = SceneManager.GetActiveScene().name;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only restore when returning to map
        if (scene.name == currentLevelMap)
        {
            RebindMapReferences();
            RestoreMapState();
        }
    }

    void Update()
    {
        EndEncounterCheck();
    }

    private void RebindMapReferences()
    {
        GameObject go = GameObject.FindGameObjectWithTag("visual_tilemap");

        if (go == null)
        {
            Debug.LogError("Visual Tilemap not found. Check tag assignment.");
            return;
        }

        tilemap = go.GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Tagged object does not contain a Tilemap component.");
        }
    }

    /*
    * Fills the tile grid data in GameState when called.
    * Then loads 3D scene. 
    */
    public void StartEncounter(TokenController selectedToken)
    {
        SaveMapState();

        int [,] grid = BuildTileGrid(selectedToken);

        GameState.Instance.Data.currentTileGrid = grid;
        GameState.Instance.Data.prefabStructs = tilesAssignedToPrefabs;

        // ✅ Build prefab list + orb total
        int totalOrbs = 0;
        List<GameObject> prefabList = new();

        foreach (var enemy in selectedToken.enemiesList)
        {
            if (enemy == null) continue;

            totalOrbs += enemy.orbCount;
            prefabList.Add(enemy.enemyPrefab);
        }

        GameState.Instance.Data.enemiesList = prefabList;
        GameState.Instance.Data.selectedCharacter = selectedToken.GetCharacterData();
        //Set requiredEncounterOrbs
        GameState.Instance.Data.requiredEncounterOrbs = totalOrbs;
        GameState.Instance.Data.orbs = 0;
        SceneManager.LoadScene("3D_Game_View");
    }

    public void EndEncounterCheck()
    {
        if (GameState.Instance.Data.orbs == GameState.Instance.Data.requiredEncounterOrbs 
        && SceneManager.GetActiveScene().name != currentLevelMap)
        {
            GameState.Instance.Data.encounterCompletedSuccessfully = true;
            GameState.Instance.Data.totalRequiredOrbs -= GameState.Instance.Data.orbs;

            SceneManager.LoadScene(currentLevelMap);
        }
    }

    public int[,] BuildTileGrid(TokenController token)
    {
        int[,] tileIndices = new int[3, 3];

        Vector3Int playerTile = tilemap.WorldToCell(token.transform.position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int tilePos = new Vector3Int(
                    playerTile.x + x,
                    playerTile.y + y,
                    0
                );

                TileBase tile = tilemap.GetTile(tilePos);

                int index = ConvertTileToIndex(tile); // convert tile → index

                tileIndices[x + 1, y + 1] = index; // store in 3x3 grid
            }
        }

        return tileIndices;
    }

    private int ConvertTileToIndex(TileBase tile)
    {
        return tileLookup.IndexOf(tile);
    }

    private void SaveMapState()
    {
        var data = GameState.Instance.Data;
        data.entityStates.Clear();

        var saveableObjects = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);

        foreach (var s in saveableObjects)
        {
            Transform t = s.transform;

            data.entityStates.Add(new EntityStateData
            {
                prefabName = t.name,
                position = t.position,
                rotation = t.rotation,
                isActive = t.gameObject.activeSelf
            });
        }
    }

    public void RestoreMapState()
    {
        var data = GameState.Instance.Data;

        if (data.entityStates == null || data.entityStates.Count == 0)
            return;

        var lookup = new Dictionary<string, EntityStateData>();

        foreach (var e in data.entityStates)
        {
            lookup[e.prefabName] = e;
        }

        var saveableObjects = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);

        foreach (var s in saveableObjects)
        {
            Transform t = s.transform;

            if (!lookup.TryGetValue(t.name, out var saved))
                continue;

            t.position = saved.position;
            t.rotation = saved.rotation;
            t.gameObject.SetActive(saved.isActive);

            if (data.encounterCompletedSuccessfully)
            {
                bool isEnemy = s.TryGetComponent<AI_EnemyToken>(out _);

                if (isEnemy)
                {
                    t.gameObject.SetActive(false);
                    continue;
                }
            }

            t.gameObject.SetActive(saved.isActive);
        }
        // reset flag after applying
        data.encounterCompletedSuccessfully = false;
    }
}
