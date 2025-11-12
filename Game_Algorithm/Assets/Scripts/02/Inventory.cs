using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    void Start()
    {
        items.Add(new Item("Sword"));
        items.Add(new Item("Shield"));
        items.Add(new Item("Potion"));

        Item found = Finditem("Potion");

        if (found != null)
            Debug.Log("찾은 아이템: " + found.itemName);
        else
            Debug.Log("아이템을 찾을 수 없습니다.");
    }

    public Item Finditem(string _itemName)
    {
        foreach(var item in items)
        {
            if (item.itemName == _itemName)
                return item;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
