using UnityEngine;
using UnityEngine.Tilemaps; 
using UnityEngine.SceneManagement;
using System.Collections.Generic; 

public class EncounterManager : MonoBehaviour
{
    [Header("References")]
    public Tilemap tilemap;
    public TokenController token;
    //get Enemies list from token to determine enemy spawns

    [Header("Tile Assignment")]
    public List<TileBase> tileLookup;   //Manually assign tile index values
    [Tooltip("Assign Prefabs to relavant tiles via their index. For Example; A prefab with 'Tile Index' of 1 will spawn on ALL terrain segments associated with Tile Lookup's Element 1")]
    public List<PrefabStruct> tilesAssignedToPrefabs;

    /*
    * Fills the tile grid data in GameState when called.
    * Then loads 3D scene. 
    */
    public void StartEncounter()
    {
        int [,] grid = BuildTileGrid();
        GameState.Instance.Data.currentTileGrid = grid;
        GameState.Instance.Data.prefabStructs = tilesAssignedToPrefabs;
        GameState.Instance.Data.enemiesList = token.enemiesList;
        SceneManager.LoadScene("3D_Game_View");
    }


    public int[,] BuildTileGrid()
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
}
