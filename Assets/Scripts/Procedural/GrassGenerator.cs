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

    private static Dictionary<Vector2Int, Vector2Int> FindGrassInDirections(HashSet<Vector2Int> groundPositions, List<Vector2Int> directionList)
    {
        Dictionary<Vector2Int, Vector2Int> grassPositions = new Dictionary<Vector2Int, Vector2Int>();
        foreach (Vector2Int position in groundPositions)
        {
            foreach (var direction in directionList)
            {
                var neighborPosition = position + direction;
                if (groundPositions.Contains(neighborPosition) == false)
                {
                    //Modifier la tile à position aussi
                    //Clé : position de la case, Valeur : la direction de vers où on l'a posée
                    grassPositions.Add(neighborPosition, direction);
                }
            }
        }
        return grassPositions;
    }
}

