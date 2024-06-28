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

    [SerializeField]
    private GameObject gumba;

    protected override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();                                      // Clear the tilemap before generating a new dungeon

        HashSet<Vector2Int> floorPositions = RunRandomIteration();                      //Generate a ground floor

        HashSet<Vector2Int> surfaceFloor = SurfaceGround(floorPositions);               //Acquire the surface of the ground
        HashSet<Vector2Int> platformPositions = RunRandomPlatform(surfaceFloor);        //Generate platforms

        tilemapVisualizer.PaintGroundTiles(floorPositions);             // Paint the default ground floor
        tilemapVisualizer.PaintGroundTiles(platformPositions);          // Paint the platforms

        GrassGenerator.CreateGrass(floorPositions, tilemapVisualizer);  // Paint the grass
        GrassGenerator.CreatePlatformGrass(platformPositions, tilemapVisualizer);  // Paint the grass on the platforms

        PopulateMonster(surfaceFloor);                                  // Populate the level with monsters
    }

    //Fonction qui génère un sol aléatoire dans un certains degrès de liberté
    protected HashSet<Vector2Int> RunRandomIteration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        int lastHeight = startPosition.y + 1;
        int sectionMinWidth = 0;

        //Generate a ground from the position.x 0 to the position 194 with a thickness of 2
        for (int i = startPosition.x; i < startPosition.x + 194; i++)
        {
            int randHeight = 3;
            if (lastHeight != -5 && lastHeight != 0) {
                randHeight = new System.Random().Next(2);
            }
            if (sectionMinWidth > 4 && lastHeight == -5 || sectionMinWidth > 4 && randHeight == 1)
            {
                lastHeight++;
                sectionMinWidth = 0;
            }
            else if (sectionMinWidth > 4 && lastHeight == 0 || sectionMinWidth > 4 && randHeight == 0)
            {
                lastHeight--;
                sectionMinWidth = 0;
            }
            sectionMinWidth++;

            for (int j = lastHeight; j >= startPosition.y; j--)
            {
                floorPositions.Add(new Vector2Int(i, j));
            }
        }

        return floorPositions;
    }

    private HashSet<Vector2Int> SurfaceGround(HashSet<Vector2Int> groundTiles)
    {
        HashSet<Vector2Int> surfaceGround = new HashSet<Vector2Int>();
        foreach (Vector2Int tile in groundTiles)
        {
            var neighborPosition = tile + Vector2Int.up;
            if (groundTiles.Contains(neighborPosition) == false)
            {
                surfaceGround.Add(tile);
            }
        }
        return surfaceGround;
    }

    //Fonction qui génère des plateformes aléatoirement
    //vérifie qu'on est sur la tile la plus haute du sol
    private HashSet<Vector2Int> RunRandomPlatform(HashSet<Vector2Int> surfaceTiles)
    {
        HashSet<Vector2Int> platformPositions = new HashSet<Vector2Int>();
        System.Random rand = new System.Random();
        int compt = 0;

        foreach (Vector2Int tile in surfaceTiles)
        {
            //Alors on est sur la tile la plus haute de notre colonne
            int tmp = rand.Next(0, 1000);

            if (tmp < 300)
            {
                compt++;
            }
            if (compt >= 5)
            {
                for (int i = 0; i < 4; i++)
                {
                    platformPositions.Add(new Vector2Int(tile.x + 3 + i, tile.y + 4));
                }
                compt = 0;
            }
        }
        return platformPositions;
    }

    private void PopulateMonster(HashSet<Vector2Int> surfaceTiles)
    {
        float offsetX = 2.8f;
        float offsetY = 2.8f;
        System.Random rand = new System.Random();

        for(int i = 0; i < surfaceTiles.Count; i++)
        {
            Vector2Int SpawnTile = surfaceTiles.ElementAt(i);

            if (rand.Next(0, 100) < 10)
            {
                Vector3 positionEnemy = new Vector3(SpawnTile.x + offsetX, SpawnTile.y + offsetY, 0);
                GameObject enemy = Instantiate(gumba, positionEnemy, Quaternion.identity);
                enemy.transform.parent = GameObject.FindWithTag("Enemy").transform;

                int dist = 0;
                //Waypoint position
                for(int j = i; j < i + 4 && j < surfaceTiles.Count; j++)
                {
                    Vector2Int distTile = surfaceTiles.ElementAt(j);
                    //Si la prochaine tile est à la même hauteur que la tile de spawn
                    if (distTile.y == SpawnTile.y)
                    {
                        if (distTile.x == SpawnTile.x + dist)
                        {
                            dist++;
                        }
                    }
                    //Au bout d'une distance d'au moins 3 on positionne le waypoint
                    if (dist > 3)
                    {
                        Vector2Int waypointTile = surfaceTiles.ElementAt(i + dist - 1);
                        Transform Waypoint2 = enemy.transform.Find("Waypoint2");
                        //Positionnement du waypoint avec des offset spécifiques au prefab
                        Vector3 pos = new Vector3(waypointTile.x + 0.5f, waypointTile.y + 1.7f, 0);
                        Waypoint2.transform.position = pos;
                        Debug.Log("Waypoint2 position: " + pos + " | dist val = " + dist);
                    }
                }
                if (dist <= 3)
                {
                    //Si nous n'avons pas la place de placer un enemy, on le détruit
                    DestroyImmediate(enemy.gameObject);
                }
            }
        }
    }
}

