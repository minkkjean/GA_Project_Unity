using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameState { Idle, Recording, Playing, Reversing }
    public GameState currentState = GameState.Idle;

    [Header("연결 요소")]
    public PlayerController playerController;
    public Text queueCountText;
    public Material defaultMaterial;
    public Material playbackMaterial;

    [Header("설정")]
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

    // --- [수정된 부분 1] ---
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

        // 재생 또는 역재생 중이 아닐 때만 총 카운트를 표시하도록 변경
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

        // --- [수정된 부분 2] --- (사용자 경험을 위해 정방향 재생에도 카운트 추가)
        for (int i = 0; i < commandHistory.Count; i++)
        {
            // 숫자가 1부터 올라가도록 표시
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

        // --- [수정된 부분 3] ---
        for (int i = commandHistory.Count - 1; i >= 0; i--)
        {
            // 숫자가 하나씩 거꾸로 내려가도록 표시
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