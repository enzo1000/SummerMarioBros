using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SimpleRandomGroundGenerator : AbstractLevelGenerator
{
    [SerializeField]
    private ProfilLevelGenerationParameters parameters;

    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();                                  // Clear the tilemap before generating a new dungeon
        HashSet<Vector2Int> floorPositions = RunRandomIteration();  //Generate a ground floor
        tilemapVisualizer.PaintGroundTiles(floorPositions);         // Paint the ground floor
        GrassGenerator.CreateGrass(floorPositions, tilemapVisualizer); // Paint the grass
    }

    protected HashSet<Vector2Int> RunRandomIteration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        //Generate a ground from the position.x 0 to the position 194 with a thickness of 2
        for (int i = startPosition.x; i < startPosition.x + 194; i++)
        {
            for (int j = startPosition.y; j < startPosition.y + 2; j++)
            {
                floorPositions.Add(new Vector2Int(i, j));
            }
        }

        return floorPositions;
    }
}

