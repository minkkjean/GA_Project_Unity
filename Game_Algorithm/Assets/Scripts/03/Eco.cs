using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Queue<Vector3> moveQueue;
    private bool isMoving = false;
    private Vector3 targetPos;

    private Stack<PositionRecord> moveHistory;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material rewindMaterial;
    private Renderer objectRenderer;
    private bool isRewinding = false;
    private float rewindStartTime;

    void Start()
    {
        moveQueue = new Queue<Vector3>();
        moveHistory = new Stack<PositionRecord>();
        objectRenderer = GetComponent<Renderer>();

        targetPos = transform.position;
        moveHistory.Push(new PositionRecord(transform.position, Time.time));

        if (objectRenderer != null)
        {
            objectRenderer.material = defaultMaterial;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && moveHistory.Count > 1)
        {
            isRewinding = true;
            isMoving = false;
            moveQueue.Clear();

            rewindStartTime = Time.time;

            if (objectRenderer != null && rewindMaterial != null)
            {
                objectRenderer.material = rewindMaterial;
            }
        }

        if (isRewinding)
        {
            float cutoffTime = rewindStartTime - rewindDuration;

            if (moveHistory.Count > 1 && moveHistory.Peek().timestamp > cutoffTime)
            {
                transform.position = moveHistory.Pop().position;
            }
            else
            {
                isRewinding = false;
                if (objectRenderer != null && defaultMaterial != null)
                {
                    objectRenderer.material = defaultMaterial;
                }
                targetPos = transform.position;
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
        else
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x != 0 || y != 0)
            {
                Vector3 move = new Vector3(x, y, 0).normalized * speed * Time.deltaTime;
                targetPos += move;
                moveQueue.Enqueue(targetPos);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (moveQueue.Count > 0)
                {
                    isMoving = true;
                }
            }
        }
    }
}