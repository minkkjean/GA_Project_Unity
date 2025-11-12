using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BubbleSortTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    public void StartBubbleSort1()
    {
        int[] data = GenerateRandomArray(10000);

        Stopwatch sw = new Stopwatch();
        sw.Reset();
        sw.Start();
        StartBubbleSort(data);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;

        string result = "BubbleSort : " + selectionTime + " ms";
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

    public static void StartBubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n-i-1; j++)
            {
                if (arr[j] < arr[j+1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }
}
