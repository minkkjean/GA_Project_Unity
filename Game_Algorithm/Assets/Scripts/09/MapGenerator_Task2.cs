using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator_Task2 : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 21;
    public int height = 21;
    [Range(0, 100)] public int wallPercent = 30;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public GameObject forestPrefab;
    public GameObject mudPrefab;
    public GameObject pathPrefab;
    public GameObject enemyPrefab;

    [Header("Enemy Settings")]
    [Range(1, 10)] public int enemyCount = 3; // [NEW] 생성할 적의 개수
    public int enemyDangerRadius = 4; // 적을 회피할 반경
    public int enemyPenaltyWeight = 20; // 적 근처일 때 부여할 가중치

    // 내부 데이터 구조
    public enum TileType { Wall, Ground, Forest, Mud }

    public class Node
    {
        public int x, y;
        public TileType type;
        public int gCost;
        public int hCost;
        public Node parent;

        public int fCost { get { return gCost + hCost; } }

        public Node(int _x, int _y, TileType _type)
        {
            x = _x; y = _y; type = _type;
        }

        public int GetBaseCost()
        {
            switch (type)
            {
                case TileType.Ground: return 1;
                case TileType.Forest: return 3;
                case TileType.Mud: return 5;
                default: return 999;
            }
        }
    }

    private Node[,] grid;
    private List<GameObject> mapObjects = new List<GameObject>();
    private List<GameObject> pathObjects = new List<GameObject>();

    private Node startNode, endNode;
    private List<Node> enemyNodes = new List<Node>(); // [NEW] 적 리스트

    void Start()
    {
        GenerateValidMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateValidMap();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindPathAStar_AvoidEnemy();
        }
    }

    void GenerateValidMap()
    {
        ClearPath();

        bool solvable = false;
        int attempt = 0;

        while (!solvable)
        {
            attempt++;
            CreateRawMap();

            startNode = grid[1, 1];
            endNode = grid[width - 2, height - 2];

            startNode.type = TileType.Ground;
            endNode.type = TileType.Ground;

            solvable = CheckIsSolvableDFS(startNode, endNode);

            if (attempt > 100) { Debug.LogError("맵 생성 실패"); break; }
        }

        SpawnEnemies(); // [NEW] 여러 적 생성
        DrawMap();
        Debug.Log($"맵 생성 완료 (시도: {attempt})");
    }

    void CreateRawMap()
    {
        grid = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileType type = TileType.Ground;

                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    type = TileType.Wall;
                }
                else
                {
                    int roll = Random.Range(0, 100);
                    if (roll < wallPercent) type = TileType.Wall;
                    else if (roll < wallPercent + 15) type = TileType.Forest;
                    else if (roll < wallPercent + 25) type = TileType.Mud;
                    else type = TileType.Ground;
                }
                grid[x, y] = new Node(x, y, type);
            }
        }
    }

    // [NEW] 적 여러 명 생성 함수
    void SpawnEnemies()
    {
        enemyNodes.Clear(); // 기존 적 리스트 초기화
        List<Node> validSpots = new List<Node>();

        // 적 생성 가능한 모든 위치 수집
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (grid[x, y].type != TileType.Wall && grid[x, y] != startNode && grid[x, y] != endNode)
                {
                    validSpots.Add(grid[x, y]);
                }
            }
        }

        // 설정한 개수(enemyCount)만큼 랜덤 배치
        int spawnCount = Mathf.Min(enemyCount, validSpots.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, validSpots.Count);
            enemyNodes.Add(validSpots[randomIndex]);
            validSpots.RemoveAt(randomIndex); // 중복 위치 방지
        }
    }

    bool CheckIsSolvableDFS(Node start, Node end)
    {
        Stack<Node> stack = new Stack<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();
            if (current == end) return true;

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && neighbor.type != TileType.Wall)
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }
        return false;
    }

    void FindPathAStar_AvoidEnemy()
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.type == TileType.Wall || closedSet.Contains(neighbor)) continue;

                int movementCost = currentNode.gCost + neighbor.GetBaseCost();

                if (movementCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = movementCost;
                    neighbor.parent = currentNode;

                    // ---------------------------------------------------------
                    // [핵심 로직] 모든 적에 대해 거리를 체크하고 가장 큰 위험도를 적용
                    // ---------------------------------------------------------
                    int distToGoal = GetManhattanDistance(neighbor, endNode);
                    int maxPenalty = 0;

                    foreach (Node enemy in enemyNodes)
                    {
                        int distToEnemy = GetManhattanDistance(neighbor, enemy);

                        // 위험 반경 안에 있는 적이 하나라도 있으면 페널티 계산
                        if (distToEnemy <= enemyDangerRadius)
                        {
                            int penalty = (enemyDangerRadius - distToEnemy + 1) * enemyPenaltyWeight;
                            // 여러 적에게 겹칠 경우 가장 큰 위험도(또는 합산)를 적용
                            if (penalty > maxPenalty) maxPenalty = penalty;
                        }
                    }

                    neighbor.hCost = distToGoal + maxPenalty;
                    // ---------------------------------------------------------

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(Node start, Node end)
    {
        ClearPath();

        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        foreach (var node in path)
        {
            GameObject p = Instantiate(pathPrefab, new Vector3(node.x, 1, node.y), Quaternion.identity);
            p.transform.parent = this.transform;
            pathObjects.Add(p);
        }
        Debug.Log("최단 경로 생성 완료");
    }

    void ClearPath()
    {
        foreach (var obj in pathObjects) Destroy(obj);
        pathObjects.Clear();
    }

    int GetManhattanDistance(Node a, Node b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[] xDir = { 0, 0, 1, -1 };
        int[] yDir = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int checkX = node.x + xDir[i];
            int checkY = node.y + yDir[i];

            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }
        return neighbors;
    }

    void DrawMap()
    {
        foreach (var obj in mapObjects) Destroy(obj);
        mapObjects.Clear();
        ClearPath();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefabToSpawn = groundPrefab;
                Vector3 spawnPos = new Vector3(x, 0, y);
                Vector3 spawnScale = new Vector3(1, 0.1f, 1);

                switch (grid[x, y].type)
                {
                    case TileType.Wall:
                        prefabToSpawn = wallPrefab;
                        spawnPos = new Vector3(x, 0.5f, y);
                        spawnScale = new Vector3(1, 1, 1);
                        break;
                    case TileType.Forest: prefabToSpawn = forestPrefab; break;
                    case TileType.Mud: prefabToSpawn = mudPrefab; break;
                }

                GameObject go = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                go.transform.localScale = spawnScale;
                go.transform.parent = this.transform;
                mapObjects.Add(go);
            }
        }

        // [NEW] 모든 적 시각화
        foreach (Node enemy in enemyNodes)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, new Vector3(enemy.x, 1.0f, enemy.y), Quaternion.identity);
            enemyObj.transform.parent = this.transform;
            mapObjects.Add(enemyObj);
        }
    }
}