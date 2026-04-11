using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{   
    public List<PrefabStruct> terrainPrefabs;
    Terrain terrain;
    public TileData[,] tileGrid = new TileData[3, 3];
    int res;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        res = terrain.terrainData.heightmapResolution;

        ResetTerrainHeight();
        PopulateData();
        ApplyHeightmap();
        SpawnPrefabs();
    }
    
    // -------------------
    // Populate Dataset
    // -------------------
    // TODO - Populate terrainPrefabs with data obtained by EncounterManager, including tileIndex.
    // TODO - Cross reference index of grid array with PrefabStruct's tileIndex.
    // TODO - Randomly spread enemy units around scene. 
    public void PopulateData()
    {
        int[,] grid = GameState.Instance.Data.currentTileGrid;

        tileGrid = new TileData[3, 3];

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tileGrid[x, y] = ConvertIndexToTileData(grid[x, y]);
            }
        }

        terrainPrefabs = GameState.Instance.Data.prefabStructs;
    }

    //converts tile index to corresponding TileData
    private TileData ConvertIndexToTileData(int index)
    {
        int baseIndex = index % 4;  //loop through heightmaps for eahc tile type.
        return baseIndex switch
        {
            0 => TileData.Plains(),
            1 => TileData.Hills(),
            2 => TileData.Mountain(),
            3 => TileData.Water(),
            _ => TileData.Plains(),// fallback
        };
    }

    // ----------------------------------------
    // GENERATE & APPLY HEIGHTMAP FROM DATASET
    // ----------------------------------------
    void ApplyHeightmap()
    {
        float[,] mesh = new float[res, res];

        int gridX = tileGrid.GetLength(0);
        int gridY = tileGrid.GetLength(1);

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                // Convert terrain → grid space
                float gx = (float)x / res * (gridX - 1);
                float gy = (float)y / res * (gridY - 1);

                int x0 = Mathf.FloorToInt(gx);
                int y0 = Mathf.FloorToInt(gy);
                int x1 = Mathf.Min(x0 + 1, gridX - 1);
                int y1 = Mathf.Min(y0 + 1, gridY - 1);

                float tx = gx - x0;
                float ty = gy - y0;

                // Sample heights
                float h00 = tileGrid[x0, y0].height;
                float h10 = tileGrid[x1, y0].height;
                float h01 = tileGrid[x0, y1].height;
                float h11 = tileGrid[x1, y1].height;

                // Bilinear interpolation for height
                float baseHeight =
                    Mathf.Lerp(
                        Mathf.Lerp(h00, h10, tx),
                        Mathf.Lerp(h01, h11, tx),
                        ty
                    );

                // Sample noise values
                float n00 = tileGrid[x0, y0].noise;
                float n10 = tileGrid[x1, y0].noise;
                float n01 = tileGrid[x0, y1].noise;
                float n11 = tileGrid[x1, y1].noise;

                // Bilinear interpolation for noise
                float blendedNoise =
                    Mathf.Lerp(
                        Mathf.Lerp(n00, n10, tx),
                        Mathf.Lerp(n01, n11, tx),
                        ty
                    );

                // Apply blended noise
                baseHeight += Mathf.PerlinNoise(x * blendedNoise, y * blendedNoise) * blendedNoise;

                mesh[y, x] = Mathf.Clamp01(baseHeight);
            }
        }
        terrain.terrainData.SetHeights(0, 0, mesh);
    }

    // -------------------
    // RESET
    // -------------------
    void ResetTerrainHeight()
    {
        var mesh = new float[res, res];
        terrain.terrainData.SetHeights(0, 0, mesh);
    }
    
    // -------------------
    // SPAWNING (CLEAN)
    // -------------------
    void SpawnPrefabs()
    {
        int gridX = tileGrid.GetLength(0);
        int gridY = tileGrid.GetLength(1);
        Vector3 terrainSize = terrain.terrainData.size;

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                int tileTypeIndex = GameState.Instance.Data.currentTileGrid[x, y]; // ✅ ADDED

                foreach (var prefabStruct in terrainPrefabs)
                {
                    if (prefabStruct.prefab == null) continue;

                    // ✅ MATCH TILE TYPE INSTEAD OF POSITION
                    if (prefabStruct.tileTypeIndex != tileTypeIndex) continue;

                    for (int i = 0; i < prefabStruct.spawnCount; i++)
                    {
                        float px = x;
                        float pz = y;
                        float rotY = 0f;

                        if (prefabStruct.applyRandomTransform)
                        {
                            px += UnityEngine.Random.value;
                            pz += UnityEngine.Random.value;
                            rotY = UnityEngine.Random.Range(0f, 360f);
                        }

                        px = px / gridX * terrainSize.x;
                        pz = pz / gridY * terrainSize.z;
                        float py = terrain.SampleHeight(new Vector3(px, 0, pz));

                        Vector3 pos = new Vector3(px, py, pz);
                        Quaternion rot = Quaternion.Euler(0, rotY, 0);

                        Instantiate(prefabStruct.prefab, pos, rot);
                    }
                }
            }
        }
    }
    // void SpawnPrefabs()
    // {
    //     int gridX = tileGrid.GetLength(0);
    //     int gridY = tileGrid.GetLength(1);
    //     Vector3 terrainSize = terrain.terrainData.size;

    //     foreach (var prefabStruct in terrainPrefabs)
    //     {
    //         if (prefabStruct.prefab == null || prefabStruct.tileIndex < 1 || prefabStruct.tileIndex > gridX * gridY)
    //             continue;

    //         // Convert 1-9 index to x, y coordinates
    //         int index = prefabStruct.tileIndex - 1;
    //         int x = index % gridX;
    //         int y = index / gridX;
    //         TileData tile = tileGrid[x, y];

    //         for (int i = 0; i < prefabStruct.spawnCount; i++)
    //         {
    //             float px = x;
    //             float pz = y;
    //             float rotY = 0f;

    //             if (prefabStruct.applyRandomTransform)
    //             {
    //                 px += UnityEngine.Random.value;
    //                 pz += UnityEngine.Random.value;
    //                 rotY = UnityEngine.Random.Range(0f, 360f);
    //             }

    //             px = px / gridX * terrainSize.x;
    //             pz = pz / gridY * terrainSize.z;
    //             float py = terrain.SampleHeight(new Vector3(px, 0, pz));

    //             Vector3 pos = new Vector3(px, py, pz);
    //             Quaternion rot = Quaternion.Euler(0, rotY, 0);

    //             Instantiate(prefabStruct.prefab, pos, rot);
    //         }
    //     }
    // }
}

// -------------------
// Prefab struct
// -------------------
[System.Serializable]
public class PrefabStruct
{
    public GameObject prefab;
    [Tooltip("Tile index (1-9) where this prefab should spawn")]
    public int tileTypeIndex;
    [Tooltip("The number of prefabs spawned on the tile")]
    public int spawnCount;
    public bool applyRandomTransform = true;
}