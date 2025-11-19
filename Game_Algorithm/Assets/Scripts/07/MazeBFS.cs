using System.Collections.Generic;
using UnityEngine;

public class MazePathFinder : MonoBehaviour
{
    public int[,] map = new int[,]
    {
        {1, 1, 1, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 1},
        {1, 0, 1, 0, 0, 0, 1},
        {1, 0, 1, 0, 1, 0, 1},
        {1, 0, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 0, 1},
        {1, 1, 1, 1, 1, 1, 1},
    };

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal = new Vector2Int(5, 5);
    bool[,] visited;
    Vector2Int[,] parent;
    Vector2Int[] dirs = new Vector2Int[]
    {
        new Vector2Int(1, 0),     
        new Vector2Int(-1, 0),    
        new Vector2Int(0, 1),    
        new Vector2Int(0, -1),  
    };

    void Start()
    {
        List<Vector2Int> path = FindPathBFS();

        if (path != null)
        {
            Debug.Log("최단 경로 발견. 길이: " + path.Count);
        }
        else
        {
            Debug.Log("경로 없음");
        }
    }
    List<Vector2Int> FindPathBFS()
    {
        int w = map.GetLength(0); 
        int h = map.GetLength(1); 
        visited = new bool[w, h];
        parent = new Vector2Int[w, h];

        Queue<Vector2Int> q = new Queue<Vector2Int>();

        q.Enqueue(start);
        visited[start.x, start.y] = true;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();

            if (cur == goal)
            {
                Debug.Log("BFS: 목표 도착!");
                return ReconstructPath(cur);
            }

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;
                if (!InBounds(nx, ny) || map[nx, ny] == 1) continue;
                if (visited[nx, ny]) continue;
                visited[nx, ny] = true;
                parent[nx, ny] = cur;
                q.Enqueue(new Vector2Int(nx, ny)); 
            }
        }
        Debug.Log("BFS: 경로 없음!");
        return null;
    }
    bool InBounds(int x, int y)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);
        return x >= 0 && y >= 0 && x < w && y < h;
    }
    List<Vector2Int> ReconstructPath(Vector2Int cur)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cur != start)
        {
            path.Add(cur);
            cur = parent[cur.x, cur.y];
        }
        path.Add(start); 
 
        path.Reverse();
        Debug.Log($"경로 길이: {path.Count}");
        string p = "경로: ";
        foreach (var node in path)
        {
            p += $"({node.x},{node.y}) -> ";
        }
        Debug.Log(p.TrimEnd(' ', '-', '>'));

        return path;
    }
}