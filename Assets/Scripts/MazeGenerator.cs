using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Dimensions")]
    public int width = 10;
    public int height = 10;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject pathPrefab;

    [Header("Customization")]
    public Vector2 cellSize = new Vector2(1, 1);
    public bool createEntranceAndExit = true;

    private int[,] maze;
    private System.Random rand = new System.Random();

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // Initialize the maze with walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; // 1 represents a wall
            }
        }

        // Start carving paths
        CarvePath(1, 1);

        if (createEntranceAndExit)
        {
            maze[1, 0] = 0; // Entrance
            maze[width - 2, height - 1] = 0; // Exit
        }
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = 0; // 0 represents a path

        // Randomize directions
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        Shuffle(directions);

        foreach (var dir in directions)
        {
            int newX = x + dir.x * 2;
            int newY = y + dir.y * 2;

            if (IsInBounds(newX, newY) && maze[newX, newY] == 1)
            {
                maze[x + dir.x, y + dir.y] = 0; // Carve path between cells
                CarvePath(newX, newY);
            }
        }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize.x, 0, y * cellSize.y);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else if (pathPrefab != null)
                {
                    Instantiate(pathPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width && y > 0 && y < height;
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void RegenerateMaze()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GenerateMaze();
        DrawMaze();
    }
}
