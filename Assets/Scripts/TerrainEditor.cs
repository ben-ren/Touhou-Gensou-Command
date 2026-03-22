using System.CodeDom.Compiler;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{
    Terrain terrain;
    TileData[,] tileGrid = new TileData[3, 3];
    float noiseScale = 0.02f;
    int res;
    void Start()
    {
        terrain = GetComponent<Terrain>();
        res = terrain.terrainData.heightmapResolution;

        ResetTerrainHeight();
        CreateDummyData();
        ApplyHeightmap();
    }
    /**
    1. Create dummy data for 3x3 tile grid for testing
    [hl][mt][mt]
    [pl][hl][mt]
    [wt][pl][hl]
    tile: height, colour, prefabs
    */
    void CreateDummyData()
    {
        tileGrid = new TileData[3, 3]
        {
            {TileData.Mountain(), TileData.Plains(), TileData.Mountain()},
            {TileData.Mountain(), TileData.Plains(), TileData.Mountain()},
            {TileData.Mountain(), TileData.Plains(), TileData.Mountain()}
        };
    }

    /*
    *   Generates a Heightmap using the generated dummy data.
    */
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

    void ResetTerrainHeight()
    {
        var mesh = new float[res, res];
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                mesh[y, x] = 0f;
            }
        }

        terrain.terrainData.SetHeights(0, 0, mesh);
    }
    /*
    5. spawn test prefabs to specified sections
    */
}