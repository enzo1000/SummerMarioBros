using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Class servant � renseigner toutes les tiles n�cessaires � la construction d'un niveau (visuellement)
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

    [SerializeField]
    private TileBase platformLeftTile, platformMiddleTile, platformRightTile;

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

    //Fonction permettant de peindre une Tile � une position donn�e sur une tilemap s�lectionn�e
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        //Convert the position of the tile to the position of the world
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    //R�utilisation param�tr�e de PaintSingleTile
    public void PainSingleGrassTile(Vector2Int position, Vector2Int direction)
    {
        //Random afin de s�lectionner l'un des 3 types de tile de grass / autres tiles
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
            //PaintSingleTile(backgroundTilemap, grassTopTiles[rand], position);
            PaintSingleTile(groundTilemap, groundTopTiles, newPosition);
        }
    }

    public void PaintSinglePlatformTile(Vector2Int position, Vector2Int direction)
    {
        if (direction == Vector2Int.left)
        {
            PaintSingleTile(groundTilemap, platformLeftTile, position);
        }
        else if (direction == Vector2Int.right)
        {
            PaintSingleTile(groundTilemap, platformRightTile, position);
        }
        else if (direction == Vector2Int.up)
        {
            PaintSingleTile(groundTilemap, platformMiddleTile, position);
        }
    }

    private void ClearEnemy()
    {
        int numChildren = GameObject.FindWithTag("Enemy").transform.childCount;
        for(int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(GameObject.FindWithTag("Enemy").transform.GetChild(i).gameObject);
        }
    }

    public void Clear()
    {
        groundTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();
        ClearEnemy();
    }
}

