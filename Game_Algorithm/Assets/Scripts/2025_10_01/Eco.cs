using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public struct PositionRecord
{
    public Vector3 position;
    public float timestamp;

    public PositionRecord(Vector3 pos, float time)
    {
        position = pos;
        timestamp = time;
    }
}

public class Eco : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public float rewindDuration = 2.0f;

    [Header("UI Elements")]
    public Text queueCountText;

    private Queue<Vector3> moveQueue;
    private bool isMoving = false;
    private Vector3 targetPos;

    private Stack<PositionRecord> moveHistory;
    private List<Vector3> rewindPath;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material rewindMaterial;
    private Renderer objectRenderer;
    private bool isRewinding = false;

    void Start()
    {
        moveQueue = new Queue<Vector3>();
        moveHistory = new Stack<PositionRecord>();
        objectRenderer = GetComponent<Renderer>();
        rewindPath = new List<Vector3>();

        targetPos = transform.position;
        moveHistory.Push(new PositionRecord(transform.position, Time.time));

        if (objectRenderer != null)
        {
            objectRenderer.material = defaultMaterial;
        }
    }

    void Update()
    {
        if (queueCountText != null)
        {
            queueCountText.text = "Queue Count : " + moveQueue.Count;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRewindButtonPressed();
        }

        if (isRewinding)
        {
            // --- [핵심 수정] 되감기 도중 WASD 입력이 있는지 확인 ---
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // 만약 WASD 입력이 들어왔다면, 자동 되감기를 즉시 중단합니다.
            if (horizontalInput != 0 || verticalInput != 0)
            {
                isRewinding = false;
                rewindPath.Clear(); // 남아있는 자동 경로 제거
                if (objectRenderer != null && defaultMaterial != null)
                {
                    objectRenderer.material = defaultMaterial; // 원래 모습으로 복귀
                }
                targetPos = transform.position; // 현재 위치를 새로운 시작점으로
                Debug.Log("자동 되감기를 중단하고 수동 조작으로 전환합니다.");
            }
            // WASD 입력이 없다면, 기존 자동 되감기를 계속 진행합니다.
            else if (rewindPath.Count > 0)
            {
                Vector3 nextRewindPos = rewindPath[0];
                transform.position = nextRewindPos;
                rewindPath.RemoveAt(0);

                // 되돌아가는 움직임을 계속 녹화합니다.
                moveHistory.Push(new PositionRecord(transform.position, Time.time));
            }
            else
            {
                // 자동 되감기 경로가 모두 소진되면 되감기를 종료합니다.
                isRewinding = false;
                if (objectRenderer != null && defaultMaterial != null)
                {
                    objectRenderer.material = defaultMaterial;
                }
                targetPos = transform.position;
                Debug.Log("자동 되감기가 완료되었습니다.");
            }
        }
        else if (isMoving)
        {
            if (moveQueue.Count > 0)
            {
                Vector3 nextPos = moveQueue.Dequeue();
                transform.position = nextPos;
                moveHistory.Push(new PositionRecord(transform.position, Time.time));
            }
            else
            {
                isMoving = false;
                targetPos = transform.position;
            }
        }
        else // isRewinding과 isMoving이 모두 false일 때 (기본 상태)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            if (x != 0 || z != 0)
            {
                Vector3 move = new Vector3(x, 0, z).normalized * speed * Time.deltaTime;
                targetPos += move;
                moveQueue.Enqueue(targetPos);
            }
        }
    }

    public void OnRecordButtonPressed()
    {
        moveQueue.Clear();
        Debug.Log("명령 큐가 초기화되었습니다. 다시 경로를 입력하세요.");
    }

    public void OnPlayButtonPressed()
    {
        if (moveQueue.Count > 0 && !isMoving && !isRewinding)
        {
            isMoving = true;
            Debug.Log("경로 이동을 시작합니다.");
        }
    }

    public void OnRewindButtonPressed()
    {
        if (moveHistory.Count > 1 && !isRewinding && !isMoving)
        {
            isMoving = false;
            moveQueue.Clear();
            rewindPath.Clear();

            float rewindCutoffTime = Time.time - rewindDuration;

            while (moveHistory.Count > 1 && moveHistory.Peek().timestamp > rewindCutoffTime)
            {
                rewindPath.Add(moveHistory.Pop().position);
            }

            if (rewindPath.Count > 0)
            {
                isRewinding = true;
                if (objectRenderer != null && rewindMaterial != null)
                {
                    objectRenderer.material = rewindMaterial;
                }
                Debug.Log("자동 되감기를 시작합니다.");
            }
            else
            {
                Debug.Log("지난 " + rewindDuration + "초 동안의 기록이 없어 되돌아갈 수 없습니다.");
            }
        }
    }
}