using UnityEngine;
using System.Collections.Generic; // Queue를 사용하기 위해 필요

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1.0f; // 한 번에 이동할 거리
    private CharacterController controller;

    // 이동 명령을 저장할 큐(Queue)
    private Queue<Vector3> commandQueue = new Queue<Vector3>();

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // GameManager가 호출할 입력 처리 함수
    public void RecordInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) commandQueue.Enqueue(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) commandQueue.Enqueue(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) commandQueue.Enqueue(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) commandQueue.Enqueue(Vector3.right);
    }

    // 큐에서 명령을 하나 꺼내 실행하는 함수
    public void ExecuteNextCommand()
    {
        if (commandQueue.Count > 0)
        {
            Vector3 moveDirection = commandQueue.Dequeue();
            controller.Move(moveDirection * moveDistance);
        }
    }

    // 현재 큐에 쌓인 명령 수를 반환하는 함수
    public int GetQueueCount()
    {
        return commandQueue.Count;
    }

    // 큐를 깨끗하게 비우는 함수
    public void ClearQueue()
    {
        commandQueue.Clear();
    }
}