using UnityEngine;
using UnityEngine.UI; // Text, InputField �� ���Ž� UI�� ����ϱ� ���� �ʿ�
using System.Collections.Generic;
using System.Linq; // OrderBy ���� ����� ����ϱ� ���� �ʿ�

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField searchInput;      // �˻�� �Է��� InputField (���Ž�)
    public GameObject itemUIPrefab;     // ��Ͽ� ǥ�õ� ������ UI ������
    public Transform contentParent;     // ScrollView ���� Content ������Ʈ

    private List<Item> shopItems = new List<Item>();
    private List<GameObject> itemUIObjects = new List<GameObject>();
    private bool isSorted = false; // ���� Ž���� ���� ����Ʈ�� ���ĵǾ����� Ȯ���ϴ� ����

    void Start()
    {
        // 1. ������ ���۵� �� 100���� �������� �����մϴ�.
        for (int i = 0; i < 100; i++)
        {
            shopItems.Add(new Item($"Item_{i:D2}", Random.Range(1, 10)));
        }

        // 2. ������ ��ü ������ ����� ȭ�鿡 ǥ���մϴ�.
        DisplayItems(shopItems);
    }

    // �־��� ������ ����Ʈ�� UI�� ǥ���ϴ� �Լ�
    void DisplayItems(List<Item> itemsToShow)
    {
        // ������ �ִ� UI ������Ʈ���� ��� �����ؼ� ����� �ʱ�ȭ�մϴ�.
        foreach (GameObject uiObj in itemUIObjects)
        {
            Destroy(uiObj);
        }
        itemUIObjects.Clear();

        // ���� ���� ����Ʈ�� �����۵��� �ϳ��� UI�� �����մϴ�.
        foreach (Item item in itemsToShow)
        {
            GameObject newItemUI = Instantiate(itemUIPrefab, contentParent);
            // �������� �ڽĿ� �ִ� Text ������Ʈ�� ã�� ������ �̸��� �����մϴ�.
            newItemUI.GetComponentInChildren<Text>().text = item.itemName;
            itemUIObjects.Add(newItemUI);
        }
    }

    // '���� Ž��' ��ư�� ������ public �Լ�
    public void OnLinearSearchButtonClicked()
    {
        string targetName = searchInput.text;
        Item foundItem = FindItemLinear(targetName);
        UpdateDisplayAfterSearch(foundItem);
    }

    // '���� Ž��' ��ư�� ������ public �Լ�
    public void OnBinarySearchButtonClicked()
    {
        // ���� Ž���� ���ĵ� ���¿����� �����ϹǷ�, ���� ���ĵ��� �ʾҴٸ� ������ ���� �����մϴ�.
        if (!isSorted)
        {
            shopItems = shopItems.OrderBy(item => item.itemName).ToList();
            isSorted = true;
            Debug.Log("Binary search requires a sorted list. List has been sorted.");
        }

        string targetName = searchInput.text;
        Item foundItem = FindItemBinary(targetName);
        UpdateDisplayAfterSearch(foundItem);
    }

    // �˻� ����� ���� ȭ���� ������Ʈ�ϴ� �Լ�
    private void UpdateDisplayAfterSearch(Item foundItem)
    {
        if (foundItem != null)
        {
            // �������� ã�Ҵٸ�, �ش� ������ �ϳ��� ��Ͽ� ǥ���մϴ�.
            DisplayItems(new List<Item> { foundItem });
        }
        else
        {
            // �������� ã�� ���ߴٸ�, ����� ���ϴ�.
            Debug.Log("Item not found.");
            DisplayItems(new List<Item>());
        }
    }

    #region Search Algorithms
    // ���� Ž�� �˰���
    private Item FindItemLinear(string targetName)
    {
        foreach (Item item in shopItems)
        {
            if (item.itemName == targetName)
            {
                return item; // ã���� �ٷ� ��ȯ
            }
        }
        return null; // �� ã���� null ��ȯ
    }

    // ���� Ž�� �˰���
    private Item FindItemBinary(string targetName)
    {
        int left = 0;
        int right = shopItems.Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int comparison = shopItems[mid].itemName.CompareTo(targetName);

            if (comparison == 0)
            {
                return shopItems[mid]; // ã��
            }
            else if (comparison < 0)
            {
                left = mid + 1; // �߰������� ũ�� ������ �κ��� Ž��
            }
            else
            {
                right = mid - 1; // �߰������� ������ ���� �κ��� Ž��
            }
        }
        return null; // �� ã��
    }
    #endregion
}