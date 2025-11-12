using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    [Range(0, 1)]
    public float wallProbability = 0.65f;

    public GameObject wall;
    public GameObject floor;
    public GameObject path;

    int[,] map;
    bool[,] visited;
    Vector2Int goal;
    readonly Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    List<Vector2Int> solvedPath = new List<Vector2Int>();

    void Start()
    {
        GenerateNewSolvableMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewSolvableMaze();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowPath();
        }
    }

    void GenerateNewSolvableMaze()
    {
        ClearAllVisuals();

        while (true)
        {
            CreateMapData();

            visited = new bool[width, height];

            if (CheckSolvable(1, 1))
            {
                break;
            }
        }

        DrawMaze();

        visited = new bool[width, height];
        solvedPath.Clear();
        FindPath(1, 1);
    }

    void CreateMapData()
    {
        map = new int[width, height];
        goal = new Vector2Int(width - 2, height - 2);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else if ((x == 1 && y == 1) || (x == goal.x && y == goal.y))
                    map[x, y] = 0;
                else
                    map[x, y] = Random.value < wallProbability ? 1 : 0;
            }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (map[x, y] == 1) ? wall : floor;
                if (prefab != null)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                    go.name = "MazeBlock";
                    go.transform.SetParent(this.transform);
                }
            }
    }

    void ShowPath()
    {
        ClearPathVisuals();

        foreach (var p in solvedPath)
        {
            if (path != null)
            {
                Vector3 pos = new Vector3(p.x, 0.1f, p.y);
                GameObject go = Instantiate(path, pos, Quaternion.identity);
                go.name = "PathBlock";
                go.transform.SetParent(this.transform);
            }
        }
    }

    void ClearAllVisuals()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ClearPathVisuals()
    {
        foreach (Transform child in this.transform)
        {
            if (child.name == "PathBlock")
            {
                Destroy(child.gameObject);
            }
        }
    }

    bool CheckSolvable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        visited[x, y] = true;

        if (x == goal.x && y == goal.y) return true;

        foreach (var d in dirs)
            if (CheckSolvable(x + d.x, y + d.y)) return true;

        return false;
    }

    bool FindPath(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        visited[x, y] = true;

        if (x == goal.x && y == goal.y)
        {
            solvedPath.Add(new Vector2Int(x, y));
            return true;
        }

        foreach (var d in dirs)
        {
            if (FindPath(x + d.x, y + d.y))
            {
                solvedPath.Add(new Vector2Int(x, y));
                return true;
            }
        }

        return false;
    }
}