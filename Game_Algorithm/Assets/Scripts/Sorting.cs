using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI; // 레거시 UI Text

public class Sorting : MonoBehaviour
{
    public Text resultText; // UI에 결과 출력용 Text
    private int[] data;

    void Start()
    {
        // 초기 랜덤 데이터 생성
        data = GenerateRandomArray(10000);
    }

    // 선택 정렬 버튼
    public void OnSelectionSort()
    {
        int[] arr = (int[])data.Clone();
        long elapsed = MeasureTime(() => SelectionSort(arr));
        string message = $"Selection Sort: {elapsed} ms";
        resultText.text = message;
        UnityEngine.Debug.Log(message);
    }

    // 버블 정렬 버튼
    public void OnBubbleSort()
    {
        int[] arr = (int[])data.Clone();
        long elapsed = MeasureTime(() => BubbleSort(arr));
        string message = $"Bubble Sort: {elapsed} ms";
        resultText.text = message;
        UnityEngine.Debug.Log(message);
    }

    // 퀵 정렬 버튼
    public void OnQuickSort()
    {
        int[] arr = (int[])data.Clone();
        long elapsed = MeasureTime(() => QuickSort(arr, 0, arr.Length - 1));
        string message = $"Quick Sort: {elapsed} ms";
        resultText.text = message;
        UnityEngine.Debug.Log(message);
    }

    // 시간 측정 함수
    private long MeasureTime(Action sortMethod)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        sortMethod.Invoke();
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    // 랜덤 배열 생성
    private int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, 100000);
        }
        return arr;
    }

    #region Sorting Algorithms

    private void SelectionSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIdx = i;
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j] < arr[minIdx])
                    minIdx = j;
            }
            int temp = arr[minIdx];
            arr[minIdx] = arr[i];
            arr[i] = temp;
        }
    }

    private void BubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }
    }

    private void QuickSort(int[] arr, int left, int right)
    {
        if (left >= right) return;

        int pivot = arr[right];
        int low = left;
        int high = right - 1;

        while (low <= high)
        {
            while (low <= high && arr[low] <= pivot) low++;
            while (low <= high && arr[high] >= pivot) high--;
            if (low < high)
            {
                int temp = arr[low];
                arr[low] = arr[high];
                arr[high] = temp;
            }
        }

        int tempPivot = arr[low];
        arr[low] = arr[right];
        arr[right] = tempPivot;

        QuickSort(arr, left, low - 1);
        QuickSort(arr, low + 1, right);
    }

    #endregion
}
