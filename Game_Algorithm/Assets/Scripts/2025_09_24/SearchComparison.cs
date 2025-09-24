using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics; // Stopwatch 사용을 위해
using TMPro; // TextMeshPro 사용 시

public class SearchComparison : MonoBehaviour
{
    // UI에 결과를 표시할 텍스트 (Inspector에서 연결)
    public TMP_Text resultText;

    private List<Item> items = new List<Item>();

    // 각 알고리즘의 총 비교 횟수를 저장할 변수
    private long linearSearchTotalComparisons = 0;
    private long quickSortTotalComparisons = 0;
    private long binarySearchTotalComparisons = 0;

    void Start()
    {
        RunComparison(10000, 100); // 10,000개의 아이템 중 100개를 탐색
    }

    void RunComparison(int itemCount, int searchCount)
    {
        // 1. 데이터 생성
        items.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            // "Item_00000", "Item_00001" 형식으로 이름 생성
            string itemName = "Item_" + i.ToString("D5");
            items.Add(new Item(itemName, Random.Range(1, 100)));
        }

        // 2. 검색할 타겟 생성
        List<string> targets = new List<string>();
        for (int i = 0; i < searchCount; i++)
        {
            targets.Add("Item_" + Random.Range(0, itemCount).ToString("D5"));
        }

        // 3. 선형 탐색 실행 및 비교 횟수 측정
        linearSearchTotalComparisons = 0;
        foreach (string target in targets)
        {
            linearSearchTotalComparisons += FindItemLinearSteps(target);
        }

        // 4. 퀵 정렬 실행 및 비교 횟수 측정
        quickSortTotalComparisons = 0;
        QuickSort(items, 0, items.Count - 1);

        // 5. 이진 탐색 실행 및 비교 횟수 측정
        binarySearchTotalComparisons = 0;
        foreach (string target in targets)
        {
            binarySearchTotalComparisons += FindItemBinarySteps(target);
        }

        // 6. 결과 출력
        DisplayResults(itemCount, searchCount);
    }

    void DisplayResults(int itemCount, int searchCount)
    {
        string results = $"Item Count: {itemCount}\n" +
                         $"Search Count: {searchCount}\n\n" +
                         $"Linear Search Total Comparisons: {linearSearchTotalComparisons}\n" +
                         $"Quick Sort Comparisons: {quickSortTotalComparisons}\n" +
                         $"Binary Search Total Comparisons: {binarySearchTotalComparisons}\n\n" +
                         $"Total (Sort + Binary): {quickSortTotalComparisons + binarySearchTotalComparisons}";

        UnityEngine.Debug.Log(results);
        if (resultText != null)
        {
            resultText.text = results;
        }
    }

    #region Search & Sort Algorithms

    // 선형 탐색 (비교 횟수 반환)
    private int FindItemLinearSteps(string target)
    {
        int steps = 0;
        foreach (Item item in items)
        {
            steps++;
            if (item.itemName == target)
            {
                return steps;
            }
        }
        return steps;
    }

    // 이진 탐색 (비교 횟수 반환)
    private int FindItemBinarySteps(string target)
    {
        int steps = 0;
        int left = 0;
        int right = items.Count - 1;
        while (left <= right)
        {
            steps++;
            int mid = left + (right - left) / 2;
            int cmp = items[mid].itemName.CompareTo(target);

            if (cmp == 0) return steps;
            else if (cmp < 0) left = mid + 1;
            else right = mid - 1;
        }
        return steps;
    }

    // 퀵 정렬
    private void QuickSort(List<Item> list, int left, int right)
    {
        if (left >= right) return;

        int pivotIndex = Partition(list, left, right);
        QuickSort(list, left, pivotIndex - 1);
        QuickSort(list, pivotIndex + 1, right);
    }

    private int Partition(List<Item> list, int left, int right)
    {
        Item pivot = list[right];
        int i = left - 1;
        for (int j = left; j < right; j++)
        {
            quickSortTotalComparisons++; // 비교 횟수 증가
            if (list[j].CompareTo(pivot) < 0)
            {
                i++;
                Swap(list, i, j);
            }
        }
        Swap(list, i + 1, right);
        return i + 1;
    }

    private void Swap(List<Item> list, int a, int b)
    {
        Item temp = list[a];
        list[a] = list[b];
        list[b] = temp;
    }

    #endregion
}