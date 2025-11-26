using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 21;
    public int height = 21;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public GameObject forestPrefab;
    public GameObject mudPrefab;
    public GameObject pathMarker;

    private Node[,] grid;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        GenerateValidMap();
    }

    public void OnClickGenerateMap()
    {
        GenerateValidMap();
    }

    public void OnClickFindPath()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("PathMarker");
        foreach (var m in markers) Destroy(m);

        Dijkstra(grid[0, 0], grid[width - 1, height - 1]);
    }

    void GenerateValidMap()
    {
        int attempt = 0;
        while (true)
        {
            attempt++;
            CreateRandomData();

            if (DFS_CheckPath(grid[0, 0], grid[width - 1, height - 1]))
            {
                Debug.Log($"맵 생성 성공! (시도 횟수: {attempt})");
                DrawMap();
                break;
            }
        }
    }

    void CreateRandomData()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileType type;
                int rand = Random.Range(0, 100);

                if (rand < 30) type = TileType.Wall;
                else if (rand < 60) type = TileType.Ground;
                else if (rand < 80) type = TileType.Forest;
                else type = TileType.Mud;

                grid[x, y] = new Node(x, y, type);
            }
        }

        grid[0, 0] = new Node(0, 0, TileType.Ground);
        grid[width - 1, height - 1] = new Node(width - 1, height - 1, TileType.Ground);
    }

    void DrawMap()
    {
        foreach (var obj in spawnedObjects) Destroy(obj);
        spawnedObjects.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                GameObject prefab = null;

                switch (grid[x, y].type)
                {
                    case TileType.Wall: prefab = wallPrefab; pos.y = 1; break;
                    case TileType.Ground: prefab = groundPrefab; break;
                    case TileType.Forest: prefab = forestPrefab; break;
                    case TileType.Mud: prefab = mudPrefab; break;
                }

                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                    spawnedObjects.Add(go);
                }
            }
        }
    }

    bool DFS_CheckPath(Node start, Node target)
    {
        Stack<Node> stack = new Stack<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();
            if (current == target) return true;

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.isWall && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }
        return false;
    }

    void Dijkstra(Node start, Node target)
    {
        SimplePriorityQueue<Node> pq = new SimplePriorityQueue<Node>();

        foreach (Node n in grid) n.gCost = int.MaxValue;

        start.gCost = 0;
        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            Node current = pq.Dequeue();

            if (current == target) break;

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (neighbor.isWall) continue;

                int newCost = current.gCost + neighbor.cost;
                if (newCost < neighbor.gCost)
                {
                    neighbor.gCost = newCost;
                    neighbor.parent = current;
                    pq.Enqueue(neighbor, newCost);
                }
            }
        }

        if (target.parent == null && target != start)
        {
            Debug.Log("경로를 찾을 수 없습니다 (오류).");
            return;
        }

        StartCoroutine(VisualizePathRoutine(target));
    }

    IEnumerator VisualizePathRoutine(Node current)
    {
        while (current != null)
        {
            Vector3 pos = new Vector3(current.x, 1.5f, current.y);
            GameObject p = Instantiate(pathMarker, pos, Quaternion.identity);
            p.tag = "PathMarker";

            current = current.parent;
            yield return new WaitForSeconds(0.05f);
        }
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i];
            int ny = node.y + dy[i];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                neighbors.Add(grid[nx, ny]);
            }
        }
        return neighbors;
    }
}