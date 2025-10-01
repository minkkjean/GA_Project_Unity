using UnityEngine;
using System.Collections.Generic; // Queue�� ����ϱ� ���� �ʿ�

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1.0f; // �� ���� �̵��� �Ÿ�
    private CharacterController controller;

    // �̵� ����� ������ ť(Queue)
    private Queue<Vector3> commandQueue = new Queue<Vector3>();

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // GameManager�� ȣ���� �Է� ó�� �Լ�
    public void RecordInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) commandQueue.Enqueue(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) commandQueue.Enqueue(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) commandQueue.Enqueue(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) commandQueue.Enqueue(Vector3.right);
    }

    // ť���� ����� �ϳ� ���� �����ϴ� �Լ�
    public void ExecuteNextCommand()
    {
        if (commandQueue.Count > 0)
        {
            Vector3 moveDirection = commandQueue.Dequeue();
            controller.Move(moveDirection * moveDistance);
        }
    }

    // ���� ť�� ���� ��� ���� ��ȯ�ϴ� �Լ�
    public int GetQueueCount()
    {
        return commandQueue.Count;
    }

    // ť�� �����ϰ� ���� �Լ�
    public void ClearQueue()
    {
        commandQueue.Clear();
    }
}