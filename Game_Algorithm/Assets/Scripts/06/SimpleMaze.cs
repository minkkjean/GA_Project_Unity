using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMaze : MonoBehaviour
{
    int[,] map =
    {
        {1,1,1,1,1 },
        {1,0,0,0,1 },
        {1,0,1,0,1 },
        {1,0,0,0,1 },
        {1,1,1,0,1 }
    };

    bool[,] visited;
    Vector2Int goal = new Vector2Int(4, 3);
    Vector2Int[] dirs = { new(1, 0), new(-1,0), new(0, 1), new(0,-1) };

    void Start()
    {
        visited = new bool[map.GetLength(0), map.GetLength(1)];
        bool ok = SearchMaze(1, 1);
        Debug.Log(ok ? "출구 찾음!" : "출구 없음.");
    }

    bool SearchMaze(int x, int y)
    {
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        visited[x, y] = true;
        Debug.Log($"이동 : ({x},{y})");

        if (x == goal.x && y == goal.y) return true;

        foreach (var d in dirs)
            if (SearchMaze(x + d.x, y + d.y)) return true;

        Debug.Log($"되돌아감 : ({x},{y})");
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
