using UnityEngine;
using UnityEngine.UI; // ���Ž� UI Text�� ����ϱ� ���� �ʿ�
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameState { Idle, Recording, Playing }
    public GameState currentState = GameState.Idle;

    [Header("���� ���")]
    public PlayerController playerController;
    public Text queueCountText; // ���Ž� Text

    [Header("����")]
    public float playbackDelay = 0.5f; // �� ��� ���� ������ �ð� ����

    private Vector3 startPosition;

    void Update()
    {
        // '��ȭ ��'�� ���� Ű���� �Է��� ����
        if (currentState == GameState.Recording)
        {
            playerController.RecordInput();
        }

        // �׻� UI �ؽ�Ʈ�� ���� ť ī��Ʈ�� ������Ʈ
        queueCountText.text = "Queue Count : " + playerController.GetQueueCount();
    }

    // 'Record' ��ư�� ������ �� ����� �Լ�
    public void OnRecordButtonPressed()
    {
        // ��ȭ ����
        currentState = GameState.Recording;

        // ���� ��ġ�� ���� ��ġ�� �����ϰ�, ���� ����� ��� ����
        startPosition = playerController.transform.position;
        playerController.ClearQueue();

        Debug.Log("��ȭ�� �����մϴ�. ť��� �������� �ʰ� �Է¸� �޽��ϴ�.");
    }

    // 'Play' ��ư�� ������ �� ����� �Լ�
    public void OnPlayButtonPressed()
    {
        // ��ȭ ���̰ų� ����� ����� ������ �������� ����
        if (currentState == GameState.Playing || playerController.GetQueueCount() == 0)
        {
            return;
        }

        // ��ȭ ��带 ���߰� ��� ����
        currentState = GameState.Playing;

        // �÷��̾ ��ȭ ���� ��ġ�� �ǵ���
        playerController.transform.position = startPosition;

        StartCoroutine(ReplayCommandsRoutine());
        Debug.Log("��ȭ�� ����� ����մϴ�.");
    }

    // ��ϵ� ����� ������� ����ϴ� �ڷ�ƾ
    IEnumerator ReplayCommandsRoutine()
    {
        // ť�� ����� �����ִ� ���� �ݺ�
        while (playerController.GetQueueCount() > 0)
        {
            playerController.ExecuteNextCommand();
            yield return new WaitForSeconds(playbackDelay); // ������ �ð���ŭ ���
        }

        // ����� ������ �ٽ� ��� ���·�
        currentState = GameState.Idle;
        Debug.Log("��� �Ϸ�.");
    }
}