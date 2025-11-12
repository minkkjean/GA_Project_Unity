using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBinary : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    void Start()
    {
        items.Sort((a, b) => a.itemName.CompareTo(b.itemName));

        Item found = Finditem("안승현");
        if (found != null)
            Debug.Log($"[이진 탐색] 찾은 아이템: {found.itemName}, 개수 : {found.quantity}");
        else
            Debug.Log("[이진 탐색] 아이템을 찾을 수 없습니다.");
    }

    public Item Finditem(string targetName)
    {
        int left = 0;
        int right = items.Count - 1;

        while(left <= right)
        {
            int mid = (left + right) / 2;
            int compare = items[mid].itemName.CompareTo(targetName);

            if(compare == 0)
            {
                return items[mid];
            }
            else if (compare < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return null;
    }

    void Update()
    {
        
    }
}
