using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    [Header("UI ¿ä¼Ò")]
    public TMP_InputField searchInput;
    public GameObject itemUIPrefab;
    public Transform contentParent;

    private List<Item> allItems = new List<Item>();
    private List<GameObject> itemUIObjects = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            allItems.Add(new Item($"Item_{i:D2}"));
        }

        DisplayItems(allItems);
    }

    private void DisplayItems(List<Item> itemsToShow)
    {
        foreach (GameObject uiObj in itemUIObjects)
        {
            Destroy(uiObj);
        }
        itemUIObjects.Clear();

        foreach (Item item in itemsToShow)
        {
            GameObject newItemUI = Instantiate(itemUIPrefab, contentParent);
            newItemUI.GetComponentInChildren<TMP_Text>().text = item.itemName;
            itemUIObjects.Add(newItemUI);
        }
    }

    public void OnClickLinearSearch()
    {
        string targetName = searchInput.text;
        Item foundItem = FindItemLinear(targetName);

        List<Item> displayList = new List<Item>();
        if (foundItem != null)
        {
            displayList.Add(foundItem);
        }
        DisplayItems(displayList);
    }

    public void OnClickBinarySearch()
    {
        string targetName = searchInput.text;
        Item foundItem = FindItemBinary(targetName);

        List<Item> displayList = new List<Item>();
        if (foundItem != null)
        {
            displayList.Add(foundItem);
        }
        DisplayItems(displayList);
    }

    public void OnClickShowAll()
    {
        searchInput.text = "";
        DisplayItems(allItems);
    }


    private Item FindItemLinear(string targetName)
    {
        foreach (Item item in allItems)
        {
            if (item.itemName == targetName)
            {
                return item;
            }
        }
        return null;
    }

    private Item FindItemBinary(string targetName)
    {
        int left = 0;
        int right = allItems.Count - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            int compare = allItems[mid].itemName.CompareTo(targetName);

            if (compare == 0)
            {
                return allItems[mid];
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
}