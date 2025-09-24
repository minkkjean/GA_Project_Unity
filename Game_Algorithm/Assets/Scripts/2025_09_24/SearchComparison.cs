using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics; // Stopwatch ����� ����
using TMPro; // TextMeshPro ��� ��

public class SearchComparison : MonoBehaviour
{
    // UI�� ����� ǥ���� �ؽ�Ʈ (Inspector���� ����)
    public TMP_Text resultText;

    private List<Item> items = new List<Item>();

    // �� �˰����� �� �� Ƚ���� ������ ����
    private long linearSearchTotalComparisons = 0;
    private long quickSortTotalComparisons = 0;
    private long binarySearchTotalComparisons = 0;

    void Start()
    {
        RunComparison(10000, 100); // 10,000���� ������ �� 100���� Ž��
    }

    void RunComparison(int itemCount, int searchCount)
    {
        // 1. ������ ����
        items.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            // "Item_00000", "Item_00001" �������� �̸� ����
            string itemName = "Item_" + i.ToString("D5");
            items.Add(new Item(itemName, Random.Range(1, 100)));
        }

        // 2. �˻��� Ÿ�� ����
        List<string> targets = new List<string>();
        for (int i = 0; i < searchCount; i++)
        {
            targets.Add("Item_" + Random.Range(0, itemCount).ToString("D5"));
        }

        // 3. ���� Ž�� ���� �� �� Ƚ�� ����
        linearSearchTotalComparisons = 0;
        foreach (string target in targets)
        {
            linearSearchTotalComparisons += FindItemLinearSteps(target);
        }

        // 4. �� ���� ���� �� �� Ƚ�� ����
        quickSortTotalComparisons = 0;
        QuickSort(items, 0, items.Count - 1);

        // 5. ���� Ž�� ���� �� �� Ƚ�� ����
        binarySearchTotalComparisons = 0;
        foreach (string target in targets)
        {
            binarySearchTotalComparisons += FindItemBinarySteps(target);
        }

        // 6. ��� ���
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

    // ���� Ž�� (�� Ƚ�� ��ȯ)
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

    // ���� Ž�� (�� Ƚ�� ��ȯ)
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

    // �� ����
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
            quickSortTotalComparisons++; // �� Ƚ�� ����
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