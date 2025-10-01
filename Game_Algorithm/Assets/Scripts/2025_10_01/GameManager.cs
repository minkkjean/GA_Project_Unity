using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameState { Idle, Recording, Playing, Reversing }
    public GameState currentState = GameState.Idle;

    [Header("���� ���")]
    public PlayerController playerController;
    public Text queueCountText;
    public Material defaultMaterial;
    public Material playbackMaterial;

    [Header("����")]
    public float pauseBetweenMoves = 0.1f;

    private Vector3 startPosition;
    private Renderer playerRenderer;
    private List<Vector3> commandHistory = new List<Vector3>();

    void Start()
    {
        if (playerController != null)
        {
            playerRenderer = playerController.GetComponent<Renderer>();
        }
    }

    // --- [������ �κ� 1] ---
    void Update()
    {
        if (currentState == GameState.Recording)
        {
            HandleRecordingInput();
        }

        if (currentState == GameState.Idle && Input.GetKeyDown(KeyCode.R))
        {
            OnReverseButtonPressed();
        }

        // ��� �Ǵ� ����� ���� �ƴ� ���� �� ī��Ʈ�� ǥ���ϵ��� ����
        if (currentState != GameState.Playing && currentState != GameState.Reversing)
        {
            queueCountText.text = "Queue Count : " + commandHistory.Count;
        }
    }

    private void HandleRecordingInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) commandHistory.Add(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S)) commandHistory.Add(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.A)) commandHistory.Add(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D)) commandHistory.Add(Vector3.right);
    }

    public void OnRecordButtonPressed()
    {
        currentState = GameState.Recording;
        startPosition = playerController.transform.position;
        commandHistory.Clear();
    }

    public void OnPlayButtonPressed()
    {
        if (currentState == GameState.Playing || currentState == GameState.Reversing || commandHistory.Count == 0)
        {
            return;
        }

        currentState = GameState.Playing;
        StartCoroutine(ReplayCommandsRoutine());
    }

    public void OnReverseButtonPressed()
    {
        if (currentState == GameState.Playing || currentState == GameState.Reversing || commandHistory.Count == 0)
        {
            return;
        }

        currentState = GameState.Reversing;
        StartCoroutine(ReversePlaybackRoutine());
    }

    IEnumerator ReplayCommandsRoutine()
    {
        if (playerRenderer != null) playerRenderer.material = playbackMaterial;
        playerController.ResetPosition(startPosition);
        yield return new WaitForSeconds(1f);

        // --- [������ �κ� 2] --- (����� ������ ���� ������ ������� ī��Ʈ �߰�)
        for (int i = 0; i < commandHistory.Count; i++)
        {
            // ���ڰ� 1���� �ö󰡵��� ǥ��
            queueCountText.text = "Queue Count : " + (i + 1);
            Vector3 moveDirection = commandHistory[i];
            yield return StartCoroutine(playerController.ExecuteMoveSmoothly(moveDirection));
            yield return new WaitForSeconds(pauseBetweenMoves);
        }

        if (playerRenderer != null) playerRenderer.material = defaultMaterial;
        currentState = GameState.Idle;
    }

    IEnumerator ReversePlaybackRoutine()
    {
        if (playerRenderer != null) playerRenderer.material = playbackMaterial;
        yield return new WaitForSeconds(0.5f);

        // --- [������ �κ� 3] ---
        for (int i = commandHistory.Count - 1; i >= 0; i--)
        {
            // ���ڰ� �ϳ��� �Ųٷ� ���������� ǥ��
            queueCountText.text = "Queue Count : " + i;
            Vector3 reverseDirection = -commandHistory[i];
            yield return StartCoroutine(playerController.ExecuteMoveSmoothly(reverseDirection));
            yield return new WaitForSeconds(pauseBetweenMoves);
        }

        if (playerRenderer != null) playerRenderer.material = defaultMaterial;
        playerController.ResetPosition(startPosition);
        currentState = GameState.Idle;
    }
}