using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class BruteForceSample : MonoBehaviour
{
    // 필드 선언 (CS0103 방지)
    [Header("UI 연결")]
    public Button startButton;

    [Header("PIN 정보")]
    [SerializeField]
    private string secretPin;

    private Coroutine runningRoutine;

    private bool isFound = false; // 중복 로그 방지 플래그

    // [수정된 Start() 메서드]
    void Start()
    {
        secretPin = UnityEngine.Random.Range(0, 10000).ToString("D4");
        UnityEngine.Debug.Log($"[Auth] 생성된 PIN = **{secretPin}**");

        // 이 밑에 StartCoroutine(BruteForceRoutine()); 줄이 있다면 **반드시 삭제하세요.**
        // runningRoutine = StartCoroutine(BruteForceRoutine());  <-- **이런 코드를 삭제!**
    }

    // OnStartButtonClicked() 메서드는 그대로 두세요.
    public void OnStartButtonClicked()
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }

        isFound = false;

        //runningRoutine = StartCoroutine(BruteForceRoutine());
    }

    IEnumerator BruteForceRoutine()
    {
        UnityEngine.Debug.Log("[Brute] 시뮬레이션 시작");

        Stopwatch sw = new Stopwatch();
        sw.Start();

        int tryCount = 0;
        int max = 10000; // CS0103 방지 (메서드 내 로컬 변수)

        for (int i = 0; i < max; i++)
        {
            // isFound 플래그 확인 (중복 방지)
            if (isFound)
            {
                yield break;
            }

            string tryString = i.ToString("D4"); // CS0103 방지 (메서드 내 로컬 변수)
            tryCount++;

            if (tryString == secretPin)
            {
                sw.Stop();
                double seconds = sw.Elapsed.TotalSeconds;

                isFound = true; // 찾았음을 표시

                UnityEngine.Debug.Log($"[Brute] **성공!** PIN=**{tryString}** 시도수={tryCount} 소요={seconds:F3}초");

                runningRoutine = null;
                yield break;
            }

            if (i % 100 == 0)
            {
                yield return null;
            }
        }

        sw.Stop();
        UnityEngine.Debug.Log($"[Brute] 모든 조합 시도 완료 (**발견 실패**). 소요={sw.Elapsed.TotalSeconds:F3}초");
        runningRoutine = null;
    }
}