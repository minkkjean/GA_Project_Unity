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
            // --- [�ٽ� ����] �ǰ��� ���� WASD �Է��� �ִ��� Ȯ�� ---
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // ���� WASD �Է��� ���Դٸ�, �ڵ� �ǰ��⸦ ��� �ߴ��մϴ�.
            if (horizontalInput != 0 || verticalInput != 0)
            {
                isRewinding = false;
                rewindPath.Clear(); // �����ִ� �ڵ� ��� ����
                if (objectRenderer != null && defaultMaterial != null)
                {
                    objectRenderer.material = defaultMaterial; // ���� ������� ����
                }
                targetPos = transform.position; // ���� ��ġ�� ���ο� ����������
                Debug.Log("�ڵ� �ǰ��⸦ �ߴ��ϰ� ���� �������� ��ȯ�մϴ�.");
            }
            // WASD �Է��� ���ٸ�, ���� �ڵ� �ǰ��⸦ ��� �����մϴ�.
            else if (rewindPath.Count > 0)
            {
                Vector3 nextRewindPos = rewindPath[0];
                transform.position = nextRewindPos;
                rewindPath.RemoveAt(0);

                // �ǵ��ư��� �������� ��� ��ȭ�մϴ�.
                moveHistory.Push(new PositionRecord(transform.position, Time.time));
            }
            else
            {
                // �ڵ� �ǰ��� ��ΰ� ��� �����Ǹ� �ǰ��⸦ �����մϴ�.
                isRewinding = false;
                if (objectRenderer != null && defaultMaterial != null)
                {
                    objectRenderer.material = defaultMaterial;
                }
                targetPos = transform.position;
                Debug.Log("�ڵ� �ǰ��Ⱑ �Ϸ�Ǿ����ϴ�.");
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
        else // isRewinding�� isMoving�� ��� false�� �� (�⺻ ����)
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
        Debug.Log("��� ť�� �ʱ�ȭ�Ǿ����ϴ�. �ٽ� ��θ� �Է��ϼ���.");
    }

    public void OnPlayButtonPressed()
    {
        if (moveQueue.Count > 0 && !isMoving && !isRewinding)
        {
            isMoving = true;
            Debug.Log("��� �̵��� �����մϴ�.");
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
                Debug.Log("�ڵ� �ǰ��⸦ �����մϴ�.");
            }
            else
            {
                Debug.Log("���� " + rewindDuration + "�� ������ ����� ���� �ǵ��ư� �� �����ϴ�.");
            }
        }
    }
}