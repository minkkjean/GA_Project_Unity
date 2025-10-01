using UnityEngine;
using UnityEngine.UI; // 레거시 UI Text를 사용하기 위해 필요
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameState { Idle, Recording, Playing }
    public GameState currentState = GameState.Idle;

    [Header("연결 요소")]
    public PlayerController playerController;
    public Text queueCountText; // 레거시 Text

    [Header("설정")]
    public float playbackDelay = 0.5f; // 각 명령 실행 사이의 시간 간격

    private Vector3 startPosition;

    void Update()
    {
        // '녹화 중'일 때만 키보드 입력을 받음
        if (currentState == GameState.Recording)
        {
            playerController.RecordInput();
        }

        // 항상 UI 텍스트를 현재 큐 카운트로 업데이트
        queueCountText.text = "Queue Count : " + playerController.GetQueueCount();
    }

    // 'Record' 버튼을 눌렀을 때 실행될 함수
    public void OnRecordButtonPressed()
    {
        // 녹화 시작
        currentState = GameState.Recording;

        // 현재 위치를 시작 위치로 저장하고, 이전 명령은 모두 삭제
        startPosition = playerController.transform.position;
        playerController.ClearQueue();

        Debug.Log("녹화를 시작합니다. 큐브는 움직이지 않고 입력만 받습니다.");
    }

    // 'Play' 버튼을 눌렀을 때 실행될 함수
    public void OnPlayButtonPressed()
    {
        // 녹화 중이거나 재생할 명령이 없으면 실행하지 않음
        if (currentState == GameState.Playing || playerController.GetQueueCount() == 0)
        {
            return;
        }

        // 녹화 모드를 멈추고 재생 시작
        currentState = GameState.Playing;

        // 플레이어를 녹화 시작 위치로 되돌림
        playerController.transform.position = startPosition;

        StartCoroutine(ReplayCommandsRoutine());
        Debug.Log("녹화된 명령을 재생합니다.");
    }

    // 기록된 명령을 순서대로 재생하는 코루틴
    IEnumerator ReplayCommandsRoutine()
    {
        // 큐에 명령이 남아있는 동안 반복
        while (playerController.GetQueueCount() > 0)
        {
            playerController.ExecuteNextCommand();
            yield return new WaitForSeconds(playbackDelay); // 지정된 시간만큼 대기
        }

        // 재생이 끝나면 다시 대기 상태로
        currentState = GameState.Idle;
        Debug.Log("재생 완료.");
    }
}