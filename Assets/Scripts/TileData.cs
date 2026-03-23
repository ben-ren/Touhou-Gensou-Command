using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public e_TileType type;
    public float height;
    public float noise;

    public static TileData Mountain() => new TileData 
    { 
        type = e_TileType.Mountain, 
        height = .8f, 
        noise = 0.1f
    };
    public static TileData Hills() => new TileData 
    { 
        type = e_TileType.Hills, 
        height = 0.04f, 
        noise = 0.03f
    };
    public static TileData Plains() => new TileData 
    { 
        type = e_TileType.Plains, 
        height = 0.01f, 
        noise = 0.01f
    };
    public static TileData Water() => new TileData 
    { 
        type = e_TileType.Water, 
        height = 0f, 
        noise = 0f
    };
}
