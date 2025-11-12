using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InventoryBigTest : MonoBehaviour
{
    List<Item> items = new List<Item>();

    private System.Random rand = new System.Random();

    void Start()
    {
        for (int i = 0; i < 100000; i++)
        {
            string name = $"item_{i:D5}";
            int qty = rand.Next(1, 100);
            items.Add(new Item(name, qty));
        }

        string target = "item_45672";
        Stopwatch sw = Stopwatch.StartNew();
        Item foundLinear = FindItemLinear(target);
        sw.Stop();
        UnityEngine.Debug.Log($"[선형 탐색] {target} 개수: {foundLinear?.quantity}, 시간: {sw.ElapsedMilliseconds}ms");

        items.Sort((a, b) => a.itemName.CompareTo(b.itemName));

        sw.Restart();
        Item foundBinary = FindItemBinary(target);
        sw.Stop();
        UnityEngine.Debug.Log($"[이진 탐색] {target} 개수: {foundBinary?.quantity}, 시간: {sw.ElapsedMilliseconds}ms");
    }

    public Item FindItemLinear(string targetName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == targetName)
                return item;
        }
        return null;
    }

    public Item FindItemBinary(string targetName)
    {
        int left = 0;
        int right = items.Count;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            int cmp = items[mid].itemName.CompareTo(targetName);

            if (cmp == 0) return items[mid];
            else if (cmp < 0) left = mid + 1;
            else right = mid - 1;
        }
        return null;
    }
}
