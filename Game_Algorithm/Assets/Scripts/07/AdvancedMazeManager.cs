using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMazeManager : MonoBehaviour
{
    // ====== 미로 및 캐릭터 설정 변수 ======
    [Header("Maze & Player Settings")]
    public int width = 21; // 홀수 추천
    public int height = 21; // 홀수 추천
    [Range(0.0f, 1.0f)]
    public float wallProbability = 0.65f; // 벽 생성 확률
    public float moveSpeed = 5f; // 캐릭터 이동 속도

    // ====== 프리팹 설정 변수 (Unity Inspector에서 할당) ======
    [Header("Prefabs (Assign in Inspector)")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject pathPrefab;          // 최단 경로 시각화용 (R키, 흰색/기본)
    public GameObject furthestNodePrefab;  // 가장 먼 칸 시각화용 (F키, 다른 색상)
    public GameObject playerPrefab;

    // ====== 알고리즘 내부 상태 변수 ======
    private int[,] map;
    private int[,] distance; // 시작점으로부터의 거리를 저장
    private Vector2Int startPos = new Vector2Int(1, 1);
    private Vector2Int goalPos;
    private bool[,] visited;
    private Vector2Int[,] parent;
    private List<Vector2Int> shortestPath = new List<Vector2Int>();
    private readonly Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private GameObject currentPlayer;
    private bool isMoving = false;

    void Start()
    {
        width = width % 2 == 0 ? width + 1 : width;
        height = height % 2 == 0 ? height + 1 : height;

        goalPos = new Vector2Int(width - 2, height - 2);

        GenerateNewSolvableMaze();
        InstantiatePlayer();
    }

    void Update()
    {
        // Space: 미로 재생성 (Goal 자동 설정 및 경로 계산)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewSolvableMaze();
            InstantiatePlayer();
        }

        // D: 가장 먼 칸 중 하나를 새로운 Goal로 지정하고 경로 재계산 (화면 정리 포함)
        if (Input.GetKeyDown(KeyCode.D))
        {
            GenerateNewGoalAndPath();
        }

        // R: 최단 경로 시각화 (기존 시각화 유지하고 중첩)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowPath();
        }

        // A: 캐릭터 자동 이동 시작 (R/F로 표시된 경로 유지)
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartAutoMove();
        }

        // F: 플레이어부터 가장 먼 칸들 시각화 (기존 시각화 유지하고 중첩)
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowFurthestNodes();
        }
    }

    // =========================================================
    // 1. 미로 생성 및 Goal 설정 로직
    // =========================================================

    void GenerateNewSolvableMaze()
    {
        ClearAllVisuals();
        ClearPathVisuals(); // 초기화 시에는 모두 정리

        while (true)
        {
            CreateMapData();
            SetFurthestGoal();
            visited = new bool[width, height];

            if (CheckSolvable(startPos.x, startPos.y))
            {
                break;
            }
        }

        DrawMaze();
        shortestPath.Clear();
        shortestPath = FindPathBFS();

        if (shortestPath != null)
        {
            Debug.Log($"[Space] 최단 경로 탐색 성공. 경로 길이: {shortestPath.Count}");
        }
    }

    // D 키: Goal 재설정 및 경로 재계산 (기존 시각화 정리)
    void GenerateNewGoalAndPath()
    {
        ClearPathVisuals(); // 새로운 경로 계산 시 화면 정리

        SetFurthestGoal();

        shortestPath.Clear();
        shortestPath = FindPathBFS();

        if (shortestPath != null)
        {
            Debug.Log($"[D Key] 새로운 Goal ({goalPos.x}, {goalPos.y})까지의 최단 경로를 계산했습니다. 이제 R 또는 A를 누르세요.");
        }
    }

    void CreateMapData()
    {
        map = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else if (x == startPos.x && y == startPos.y)
                    map[x, y] = 0;
                else
                    map[x, y] = Random.value < wallProbability ? 1 : 0;
            }
    }

    int CalculateAllDistances()
    {
        distance = new int[width, height];
        visited = new bool[width, height];
        Queue<Vector2Int> q = new Queue<Vector2Int>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distance[x, y] = -1;

        q.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;
        distance[startPos.x, startPos.y] = 0;

        int maxDistance = 0;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();
            int curDist = distance[cur.x, cur.y];

            if (curDist > maxDistance)
            {
                maxDistance = curDist;
            }

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny) || map[nx, ny] == 1 || visited[nx, ny]) continue;

                visited[nx, ny] = true;
                distance[nx, ny] = curDist + 1;
                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
        return maxDistance;
    }

    void SetFurthestGoal()
    {
        int maxDistance = CalculateAllDistances();

        Vector2Int furthestPos = startPos;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (distance[x, y] == maxDistance && map[x, y] == 0)
                {
                    furthestPos = new Vector2Int(x, y);
                    break;
                }
            }
        }

        goalPos = furthestPos;
        map[goalPos.x, goalPos.y] = 0;

        Debug.Log($"[Goal Set] Goal 설정 완료: ({goalPos.x}, {goalPos.y}), 최단 거리: {maxDistance}");
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (map[x, y] == 1) ? wallPrefab : floorPrefab;
                if (prefab != null)
                {
                    Vector3 pos = new Vector3(x, 0, y);
                    Instantiate(prefab, pos, Quaternion.identity, this.transform).name = "MazeBlock";
                }
            }
    }

    bool CheckSolvable(int x, int y)
    {
        if (!InBounds(x, y) || map[x, y] == 1 || visited[x, y]) return false;

        visited[x, y] = true;

        if (x == goalPos.x && y == goalPos.y) return true;

        foreach (var d in dirs)
            if (CheckSolvable(x + d.x, y + d.y)) return true;

        return false;
    }


    // =========================================================
    // 2. 최단 거리 탐색
    // =========================================================

    List<Vector2Int> FindPathBFS()
    {
        visited = new bool[width, height];
        parent = new Vector2Int[width, height];
        Queue<Vector2Int> q = new Queue<Vector2Int>();

        q.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();

            if (cur == goalPos)
            {
                return ReconstructPath(cur);
            }

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny) || map[nx, ny] == 1 || visited[nx, ny]) continue;

                visited[nx, ny] = true;
                parent[nx, ny] = cur;
                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
        return null;
    }

    List<Vector2Int> ReconstructPath(Vector2Int cur)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cur != startPos)
        {
            path.Add(cur);
            cur = parent[cur.x, cur.y];
        }
        path.Add(startPos);

        path.Reverse();
        return path;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }


    // =========================================================
    // 3. 캐릭터 및 시각화 시스템
    // =========================================================

    void InstantiatePlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer);
        if (playerPrefab != null)
        {
            Vector3 pos = new Vector3(startPos.x, 1.0f, startPos.y);
            currentPlayer = Instantiate(playerPrefab, pos, Quaternion.identity, this.transform);
            currentPlayer.name = "Player";
        }
    }

    // 'R' 키 (최단 경로 시각화) - 기존 시각화 유지 (ClearPathVisuals 제거)
    void ShowPath()
    {
        // ClearPathVisuals(); // <-- 제거: 시각화 누적

        if (pathPrefab == null || shortestPath == null || shortestPath.Count < 2)
        {
            Debug.LogWarning("최단 경로를 찾지 못했거나 경로 프리팹이 없습니다.");
            return;
        }

        for (int i = 1; i < shortestPath.Count - 1; i++)
        {
            var p = shortestPath[i];
            Vector3 pos = new Vector3(p.x, 0.1f, p.y);
            Instantiate(pathPrefab, pos, Quaternion.identity, this.transform).name = "PathBlock";
        }
        Debug.Log("최단 경로 시각화 완료.");
    }

    // 'F' 키 (가장 먼 칸들 시각화) - 기존 시각화 유지 (ClearPathVisuals 제거)
    void ShowFurthestNodes()
    {
        // ClearPathVisuals(); // <-- 제거: 시각화 누적

        if (furthestNodePrefab == null)
        {
            Debug.LogWarning("Furthest Node Prefab이 할당되지 않아 F 키 기능을 사용할 수 없습니다.");
            return;
        }

        int maxDistance = CalculateAllDistances();
        int nodesCount = 0;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (distance[x, y] == maxDistance && map[x, y] == 0)
                {
                    Vector3 pos = new Vector3(x, 0.1f, y);
                    Instantiate(furthestNodePrefab, pos, Quaternion.identity, this.transform).name = "FurthestBlock";
                    nodesCount++;
                }
            }

        Debug.Log($"[F Key] 최대 거리 {maxDistance}를 가진 칸 {nodesCount}개를 시각화했습니다. (색상 변경 적용)");
    }

    // 'A' 키 (자동 이동 시작)
    public void StartAutoMove()
    {
        if (isMoving) return;

        if (currentPlayer == null || shortestPath == null || shortestPath.Count < 2)
        {
            Debug.LogWarning("캐릭터나 경로가 준비되지 않았습니다.");
            return;
        }

        isMoving = true;
        StartCoroutine(MoveAlongPath(shortestPath));
    }

    IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPosition = new Vector3(path[i].x, currentPlayer.transform.position.y, path[i].y);

            while (Vector3.Distance(currentPlayer.transform.position, targetPosition) > 0.01f)
            {
                currentPlayer.transform.position = Vector3.MoveTowards(
                    currentPlayer.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPlayer.transform.position = targetPosition;
        }

        isMoving = false;
        Debug.Log("자동 이동 완료! 🎉 목표 지점에 도착했습니다.");
    }

    // 시각화 오브젝트 정리
    void ClearAllVisuals()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ClearPathVisuals()
    {
        // PathBlock 또는 FurthestBlock만 제거
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.name == "PathBlock" || child.name == "FurthestBlock")
            {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (var go in toDestroy)
        {
            Destroy(go);
        }
    }
}