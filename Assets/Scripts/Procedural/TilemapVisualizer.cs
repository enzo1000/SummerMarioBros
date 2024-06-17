using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Class servant à renseigner toutes les tiles nécessaires à la construction d'un niveau (visuellement)
public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTilemap, backgroundTilemap;

    [SerializeField]
    private TileBase fullDarkTile;

    [SerializeField]
    private TileBase groundTopTiles, groundLeftTiles, groundRightTiles;

    [SerializeField]
    private TileBase[] grassTopTiles, grassLeftTiles, grassRightTiles;

    // Start is called before the first frame update
    public void PaintGroundTiles(IEnumerable<Vector2Int> floorPosition)
    {
        PaintTiles(floorPosition, groundTilemap, fullDarkTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    //Fonction permettant de peindre une Tile à une position donnée sur une tilemap sélectionnée
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        //Convert the position of the tile to the position of the world
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    //Réutilisation paramétrée de PaintSingleTile
    public void PainSingleGrassTile(Vector2Int position, Vector2Int direction)
    {
        int rand = Random.Range(0, 3);
        if (direction == Vector2Int.up)
        {
            Vector2Int newPosition = position + Vector2Int.down;
            PaintSingleTile(backgroundTilemap, grassTopTiles[rand], position);
            PaintSingleTile(groundTilemap, groundTopTiles, newPosition);
        }
        else if (direction == Vector2Int.left)
        {
            Vector2Int newPosition = position + Vector2Int.right;
            PaintSingleTile(backgroundTilemap, grassLeftTiles[rand], position);
            PaintSingleTile(groundTilemap, groundLeftTiles, newPosition);
        }
        else if (direction == Vector2Int.right)
        {
            Vector2Int newPosition = position + Vector2Int.left;
            PaintSingleTile(backgroundTilemap, grassLeftTiles[rand], position);
            PaintSingleTile(groundTilemap, groundRightTiles, newPosition);
        }
    }

    public void Clear()
    {
        groundTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();
    }
}

