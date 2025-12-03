using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 21;  // 홀수 추천
    public int height = 21; // 홀수 추천
    [Range(0, 100)] public int wallPercent = 30; // 벽 생성 확률

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject groundPrefab; // Cost 1
    public GameObject forestPrefab; // Cost 3
    public GameObject mudPrefab;    // Cost 5
    public GameObject pathPrefab;   // 경로 표시용

    // 내부 데이터 구조
    public enum TileType { Wall, Ground, Forest, Mud }

    public class Node
    {
        public int x, y;
        public TileType type;
        public int gCost;
        public int hCost;
        public Node parent;
        public bool isNearWall; // 벽 근처인지 여부

        public int fCost { get { return gCost + hCost; } }

        public Node(int _x, int _y, TileType _type)
        {
            x = _x; y = _y; type = _type;
            isNearWall = false;
        }

        // 지형별 기본 코스트 반환
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
    private Node startNode, endNode;

    void Start()
    {
        GenerateValidMap();
    }

    void Update()
    {
        // 스페이스바를 누르면 맵 재생성
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateValidMap();
        }
        // 엔터키를 누르면 길찾기 실행
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindPathAStar();
        }
    }

    // 1~3. 맵 생성 및 DFS 검증 루프
    void GenerateValidMap()
    {
        bool solvable = false;
        int attempt = 0;

        while (!solvable)
        {
            attempt++;
            CreateRawMap();

            // 시작점(좌하단)과 도착점(우상단) 설정
            startNode = grid[1, 1];
            endNode = grid[width - 2, height - 2];

            // 시작과 끝은 무조건 땅으로 설정
            startNode.type = TileType.Ground;
            endNode.type = TileType.Ground;

            // 벽 근처 플래그 설정 (A* 휴리스틱용)
            MarkNearWallNodes();

            // DFS로 탈출 가능 여부 확인
            solvable = CheckIsSolvableDFS(startNode, endNode);

            if (attempt > 100) { Debug.LogError("맵 생성 실패: 조건을 만족하는 맵을 찾기 어렵습니다."); break; }
        }

        Debug.Log($"맵 생성 완료 (시도 횟수: {attempt})");
        DrawMap();
    }

    // 랜덤 맵 데이터 생성
    void CreateRawMap()
    {
        grid = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileType type = TileType.Ground;

                // 외곽은 무조건 벽
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    type = TileType.Wall;
                }
                else
                {
                    int roll = Random.Range(0, 100);
                    if (roll < wallPercent) type = TileType.Wall;
                    else if (roll < wallPercent + 15) type = TileType.Forest; // 15% 확률 숲
                    else if (roll < wallPercent + 25) type = TileType.Mud;    // 10% 확률 진흙
                    else type = TileType.Ground;                              // 나머지 땅
                }
                grid[x, y] = new Node(x, y, type);
            }
        }
    }

    // 벽 근처 노드 마킹 (과제 요구사항: 벽 회피 코스트 부여용)
    void MarkNearWallNodes()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (grid[x, y].type == TileType.Wall) continue;

                // 상하좌우 확인
                if (grid[x + 1, y].type == TileType.Wall || grid[x - 1, y].type == TileType.Wall ||
                    grid[x, y + 1].type == TileType.Wall || grid[x, y - 1].type == TileType.Wall)
                {
                    grid[x, y].isNearWall = true;
                }
            }
        }
    }

    // 2. DFS를 이용한 탈출 가능 여부 파악 (코스트 무시)
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

    // A* 알고리즘 구현 (핵심 과제)
    void FindPathAStar()
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // F cost가 가장 낮은 노드 찾기
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

                // ** 핵심 로직: 이동 비용 계산 **
                // 기본 코스트 + (벽 근처라면 2 추가)
                int movementCostToNeighbor = currentNode.gCost + neighbor.GetBaseCost();
                if (neighbor.isNearWall)
                {
                    movementCostToNeighbor += 2; // 벽 회피 가중치 부여
                }

                if (movementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = movementCostToNeighbor;
                    neighbor.hCost = GetManhattanDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        // 경로 시각화
        foreach (var node in path)
        {
            GameObject p = Instantiate(pathPrefab, new Vector3(node.x, 1, node.y), Quaternion.identity);
            mapObjects.Add(p);
        }
        Debug.Log("최단 거리 경로 표시 완료");
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
        // 기존 맵 오브젝트 삭제
        foreach (var obj in mapObjects) Destroy(obj);
        mapObjects.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefabToSpawn = groundPrefab;
                switch (grid[x, y].type)
                {
                    case TileType.Wall: prefabToSpawn = wallPrefab; break;
                    case TileType.Forest: prefabToSpawn = forestPrefab; break;
                    case TileType.Mud: prefabToSpawn = mudPrefab; break;
                }

                GameObject go = Instantiate(prefabToSpawn, new Vector3(x, 0, y), Quaternion.identity);
                go.transform.parent = this.transform;
                mapObjects.Add(go);
            }
        }
    }
}