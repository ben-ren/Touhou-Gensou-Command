using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{   
    public List<PrefabStruct> terrainPrefabs;
    Terrain terrain;
    public TileData[,] tileGrid = new TileData[3, 3];
    public List<TerrainTextureRules> textureRules;
    private Dictionary<int, int> tileToLayer = new();
    int res;

    float blendStart;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        res = terrain.terrainData.heightmapResolution;
        
        ResetTerrainHeight();
        PopulateData();
        BuildTileToLayerLookup();

        ApplyHeightmap();

        ApplyTexturesToGrid();

        if(terrainPrefabs!= null) SpawnPrefabs();
        SpawnKeyEnemies();
    }
    
    // -------------------
    // Populate Dataset
    // -------------------
    public void PopulateData()
    {
        int[,] grid = GameState.Instance.Data.currentTileGrid;
        
        if(grid == null) grid = DefaultGrid(); //create grid default override

        tileGrid = new TileData[3, 3];

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tileGrid[x, y] = ConvertIndexToTileData(grid[x, y]);
            }
        }

        terrainPrefabs = GameState.Instance.Data.prefabStructs;
        textureRules = GameState.Instance.Data.textureRules;
    }

    private int[,] DefaultGrid()
    {
        return new int[3, 3]
        {
            { 0, 1, 3},
            { 1, 2, 2},
            { 2, 3, 2}
        };
    }

    void BuildTileToLayerLookup()
    {
        tileToLayer.Clear();

        var layers = terrain.terrainData.terrainLayers;

        foreach (var rule in textureRules)
        {
            if (rule.terrainLayer == null) continue;

            int layerIndex = Array.IndexOf(layers, rule.terrainLayer);

            if (layerIndex >= 0)
            {
                tileToLayer[rule.tileTypeIndex] = layerIndex;
            }
        }

        
    }

    //converts tile index to corresponding TileData
    private TileData ConvertIndexToTileData(int index)
    {
        int baseIndex = index % 4;  //loop through heightmaps for eahc tile type.
        return baseIndex switch
        {
            0 => TileData.Water(),
            1 => TileData.Plains(),
            2 => TileData.Hills(),
            3 => TileData.Mountain(),
            _ => TileData.Plains(),// fallback default
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

                // Compress blending to edge regions
                // ===================================
                float blendTx = EdgeBlend(tx);
                float blendTy = EdgeBlend(ty);

                // Sample heights
                float h00 = tileGrid[x0, y0].height;
                float h10 = tileGrid[x1, y0].height;
                float h01 = tileGrid[x0, y1].height;
                float h11 = tileGrid[x1, y1].height;

                // Bilinear interpolation for height
                float baseHeight =
                    Mathf.Lerp(
                        Mathf.Lerp(h00, h10, blendTx),
                        Mathf.Lerp(h01, h11, blendTx),
                        blendTy
                    );

                // Sample noise values
                float n00 = tileGrid[x0, y0].noise;
                float n10 = tileGrid[x1, y0].noise;
                float n01 = tileGrid[x0, y1].noise;
                float n11 = tileGrid[x1, y1].noise;

                // Bilinear interpolation for noise
                float blendedNoise =
                    Mathf.Lerp(
                        Mathf.Lerp(n00, n10, blendTx),
                        Mathf.Lerp(n01, n11, blendTx),
                        blendTy
                    );

                // Apply blended noise
                baseHeight += Mathf.PerlinNoise(x * blendedNoise, y * blendedNoise) * blendedNoise;

                mesh[y, x] = Mathf.Clamp01(baseHeight);
            }
        }
        terrain.terrainData.SetHeights(0, 0, mesh);
    }

    // --------------------------
    // Apply textures to terrain
    // --------------------------
    void ApplyTexturesToGrid()
    {
        var grid = GameState.Instance.Data.currentTileGrid;

        if (grid == null || terrain == null)
            return;

        int gridX = grid.GetLength(0);
        int gridY = grid.GetLength(1);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                int tileIndex = grid[x, y];

                if (!tileToLayer.TryGetValue(tileIndex, out int layerIndex))
                    layerIndex = 0;

                ApplyTextures(x, y, layerIndex);
            }
        }
    }

    void ApplyTextures(int _x_coord, int _y_coord, int layerIndex)
    {
        if (textureRules == null || terrain == null) return;

        TerrainData data = terrain.terrainData;

        int alphamapWidth = data.alphamapWidth;
        int alphamapHeight = data.alphamapHeight;
        int layers = data.terrainLayers.Length;

        // size of one tile in alphamap space
        int tileWidth = alphamapWidth / 3;
        int tileHeight = alphamapHeight / 3;

        int xStart = tileWidth * _x_coord;
        int yStart = tileHeight * _y_coord;

        // get FULL alphamap (this is the key fix)
        float[,,] map = data.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        for (int y = yStart; y < yStart + tileHeight; y++)
        {
            for (int x = xStart; x < xStart + tileWidth; x++)
            {
                // safety (prevents edge overflow if grid sizes ever change)
                if (x >= alphamapWidth || y >= alphamapHeight)
                    continue;

                // clear existing layers at this pixel
                for (int l = 0; l < layers; l++)
                    map[y, x, l] = 0f;

                // apply chosen texture
                map[y, x, layerIndex] = 1f;
            }
        }

        data.SetAlphamaps(0, 0, map);
    }

    // -------------------
    // Blend Compression
    // -------------------
    private float EdgeBlend(float t)
    {
        blendStart = 0.35f; // The percentage of tile edge's that are blended by type

        if (t < blendStart)
            return 0f;

        return Mathf.SmoothStep(
            0f,
            1f,
            (t - blendStart) / (1f - blendStart)
        );
    }

    public void SetTerrainBlend(float blendVal)
    {
        blendStart = blendVal;
    }

    public float GetTerrainBlend()
    {
        return blendStart;
    }

    // -------------------
    // RESET
    // -------------------
    void ResetTerrainHeight()
    {
        var mesh = new float[res, res];
        terrain.terrainData.SetHeights(0, 0, mesh);
    }
    
    // ------------------------
    // OBJECT/ENEMY SPAWNING
    // ------------------------
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

    void SpawnKeyEnemies()
    {
        var enemies = GameState.Instance.Data.enemiesList;
        if (enemies == null || enemies.Count == 0) return;

        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        foreach( var enemy in enemies)
        {
            float randomX = UnityEngine.Random.Range(terrainPos.x, terrainPos.x + terrainSize.x);
            float randomZ = UnityEngine.Random.Range(terrainPos.z, terrainPos.z + terrainSize.z);
            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPos.y;
            float randomY = UnityEngine.Random.Range(y + 10, terrainSize.y/3);
            Vector3 randomPos = new Vector3(randomX, randomY, randomZ);
            Instantiate(enemy, randomPos, Quaternion.identity);
        }
    }
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

// -------------------
// Texture struct
// -------------------
[System.Serializable]
public class TerrainTextureRules
{
    [Tooltip("Tile index from EncounterManager tileLookup")]
    public int tileTypeIndex;

    [Tooltip("Terrain Layer used for this tile")]
    public TerrainLayer terrainLayer;
}