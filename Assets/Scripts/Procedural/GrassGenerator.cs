using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Classe servant à générer les bonnes tiles de sols du niveau (en fonction des voisins des tiles)
public static class GrassGenerator
{
    public static void CreateGrass(HashSet<Vector2Int> groundPosition, TilemapVisualizer tilemapVisualizer)
    {
        Dictionary<Vector2Int, Vector2Int> basicGrassPositions = FindGrassInDirections(groundPosition, Direction2D.cardinalDirectionsList);

        foreach (var position in basicGrassPositions)
        {
            tilemapVisualizer.PainSingleGrassTile(position.Key, position.Value);
        }
    }

    //On parcours tout notre HashSet de positions et on regarde en fonction des voisins de chaque tile la position de l'herbe
    private static Dictionary<Vector2Int, Vector2Int> FindGrassInDirections(HashSet<Vector2Int> groundPositions, List<Vector2Int> directionList)
    {
        Dictionary<Vector2Int, Vector2Int> grassPositions = new Dictionary<Vector2Int, Vector2Int>();
        foreach (Vector2Int position in groundPositions)
        {
            foreach (Vector2Int direction in directionList)
            {
                var neighborPosition = position + Vector2Int.up;
                if (groundPositions.Contains(neighborPosition) == false)
                {
                    //Modifier la tile à position aussi
                    //Clé : position de la case, Valeur : la direction de vers où on l'a posée
                    if (!grassPositions.ContainsKey(neighborPosition))
                    {
                        grassPositions.Add(neighborPosition, direction);
                    }
                }
            }
        }
        return grassPositions;
    }

    public static void CreatePlatformGrass(HashSet<Vector2Int> platformPositions, TilemapVisualizer tilemapVisualizer)
    {
        List<Vector2Int> directionToCheck = new List<Vector2Int> { Vector2Int.left, Vector2Int.right };

        foreach (var position in platformPositions)
        {
            foreach (var direction in directionToCheck)
            {
                var neighborPosition = position + direction;
                if (platformPositions.Contains(neighborPosition) == false && direction == Vector2Int.left)
                {
                    tilemapVisualizer.PaintSinglePlatformTile(position, Vector2Int.left);
                    break;  //Nécessaire car sinon est paint en Vector2Int.up pour le deuxième passage de la boucle 
                }
                else if (platformPositions.Contains(neighborPosition) == false && direction == Vector2Int.right)
                {
                    tilemapVisualizer.PaintSinglePlatformTile(position, Vector2Int.right);
                }
                else
                {
                    tilemapVisualizer.PaintSinglePlatformTile(position, Vector2Int.up);
                }
            }
        }
    }
}

