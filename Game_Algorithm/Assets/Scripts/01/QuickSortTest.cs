using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class QuickSortTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    public void StartQuickSort1()
    {
        int[] data = GenerateRandomArray(10000);
        Stopwatch sw = new Stopwatch();
        sw.Reset();
        sw.Start();
        StartQuickSort(data, 0, data.Length - 1);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;

        string result = "QuickSort : " + selectionTime + " ms";
        Debug.Log(result);

        if (resultText != null) resultText.text = result;
    }

    int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, 10000);
        }
        return arr;
    }

    public static void StartQuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);

            StartQuickSort(arr, low, pivotIndex - 1);
            StartQuickSort(arr, pivotIndex + 1, high);
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        int temp2 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp2;

        return i + 1;
    }
}
  