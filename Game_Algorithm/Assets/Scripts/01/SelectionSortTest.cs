using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SelectionSortTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;

    public void SelectionSort()
    {
        int[] data = GenerateRandomArray(10000);

        Stopwatch sw = new Stopwatch();
        sw.Reset();
        sw.Start();
        StartSelectionSort(data);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;

        string result = $"SelectionSort : {selectionTime} ms";
        Debug.Log(result);

        if (resultText != null) resultText.text = result;
    }

    int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for(int i = 0; i<size; i++)
        {
            arr[i] = rand.Next(0, 10000);
        }
        return arr;
    }

    public static void StartSelectionSort(int[] arr)
    {
        int n = arr.Length;
        for(int i = 0; i<n-1; i++)
        {
            int minIndex = i;
            for(int j = i+1; j < n; j++)
            {
                if (arr[j] < arr[minIndex])
                {
                    minIndex = j;
                }
            }
            int temp = arr[minIndex];
            arr[minIndex] = arr[i];
            arr[i] = temp;
        }
    }
}
