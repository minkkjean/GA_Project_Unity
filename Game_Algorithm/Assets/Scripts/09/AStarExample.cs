using System.Collections.Generic;
using UnityEngine;

public class AStarExample : MonoBehaviour
{
    // 0: 벽(이동불가), 1: 일반 땅, 2: 늪(비용3), 3: 물(비용5) 등으로 가정
    int[,] map =
    {
        {0,0,0,0,0},
        {0,1,1,1,0},
        {0,1,2,1,0},
        {0,1,3,1,0},
        {0,0,0,0,0},
    };

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal = new Vector2Int(3, 3);

    void Start()
    {
        var path = AStar(map, start, goal);
        if (path == null)
        {
            Debug.Log("경로 없음");
        }
        else
        {
            Debug.Log("A* 경로:");
            foreach (var p in path)
            {
                Debug.Log(p);
            }
        }
    }

    // --- A* 핵심 알고리즘 (이미지 1 내용) ---
    List<Vector2Int> AStar(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] gCost = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        // 초기화
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
                gCost[x, y] = int.MaxValue;
        }

        gCost[start.x, start.y] = 0;

        List<Vector2Int> open = new List<Vector2Int>();
        open.Add(start);

        // 상하좌우 방향
        Vector2Int[] dirs =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (open.Count > 0)
        {
            // F값이 가장 작은 노드 찾기 (PriorityQueue 대신 리스트 순회 사용)
            int bestIndex = 0;
            int bestF = F(open[0], gCost, goal);

            for (int i = 1; i < open.Count; i++)
            {
                int f = F(open[i], gCost, goal);
                if (f < bestF)
                {
                    bestF = f;
                    bestIndex = i;
                }
            }

            Vector2Int cur = open[bestIndex];
            open.RemoveAt(bestIndex);

            // 이미 방문했으면 스킵
            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            // 도착 확인
            if (cur == goal)
                return Reconstruct(parent, start, goal);

            // 이웃 체크
            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue; // 0은 벽이라고 가정
                if (visited[nx, ny]) continue;

                int moveCost = TileCost(map[nx, ny]);
                int newG = gCost[cur.x, cur.y] + moveCost;

                if (newG < gCost[nx, ny]) // 더 좋은 경로 발견
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    if (!open.Contains(new Vector2Int(nx, ny)))
                        open.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return null;
    }

    // --- 헬퍼 함수들 (이미지 2 내용) ---

    // F = G + H
    int F(Vector2Int pos, int[,] gCost, Vector2Int goal)
    {
        return gCost[pos.x, pos.y] + H(pos, goal);
    }

    // 휴리스틱 (맨해튼 거리)
    int H(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // 타일별 이동 비용
    int TileCost(int tile)
    {
        switch (tile)
        {
            case 1: return 1;
            case 2: return 3;
            case 3: return 5;
            default: return int.MaxValue;
        }
    }

    // 맵 범위 체크
    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < map.GetLength(0) &&
               y < map.GetLength(1);
    }

    // 경로 역추적 (도착점 -> 시작점)
    List<Vector2Int> Reconstruct(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }
}