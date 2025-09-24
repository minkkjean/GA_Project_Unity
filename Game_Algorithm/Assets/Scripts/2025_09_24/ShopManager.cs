using UnityEngine;
using UnityEngine.UI; // Text, InputField 등 레거시 UI를 사용하기 위해 필요
using System.Collections.Generic;
using System.Linq; // OrderBy 정렬 기능을 사용하기 위해 필요

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField searchInput;      // 검색어를 입력할 InputField (레거시)
    public GameObject itemUIPrefab;     // 목록에 표시될 아이템 UI 프리팹
    public Transform contentParent;     // ScrollView 안의 Content 오브젝트

    private List<Item> shopItems = new List<Item>();
    private List<GameObject> itemUIObjects = new List<GameObject>();
    private bool isSorted = false; // 이진 탐색을 위해 리스트가 정렬되었는지 확인하는 변수

    void Start()
    {
        // 1. 게임이 시작될 때 100개의 아이템을 생성합니다.
        for (int i = 0; i < 100; i++)
        {
            shopItems.Add(new Item($"Item_{i:D2}", Random.Range(1, 10)));
        }

        // 2. 생성된 전체 아이템 목록을 화면에 표시합니다.
        DisplayItems(shopItems);
    }

    // 주어진 아이템 리스트를 UI에 표시하는 함수
    void DisplayItems(List<Item> itemsToShow)
    {
        // 이전에 있던 UI 오브젝트들을 모두 삭제해서 목록을 초기화합니다.
        foreach (GameObject uiObj in itemUIObjects)
        {
            Destroy(uiObj);
        }
        itemUIObjects.Clear();

        // 새로 받은 리스트의 아이템들을 하나씩 UI로 생성합니다.
        foreach (Item item in itemsToShow)
        {
            GameObject newItemUI = Instantiate(itemUIPrefab, contentParent);
            // 프리팹의 자식에 있는 Text 컴포넌트를 찾아 아이템 이름을 설정합니다.
            newItemUI.GetComponentInChildren<Text>().text = item.itemName;
            itemUIObjects.Add(newItemUI);
        }
    }

    // '선형 탐색' 버튼에 연결할 public 함수
    public void OnLinearSearchButtonClicked()
    {
        string targetName = searchInput.text;
        Item foundItem = FindItemLinear(targetName);
        UpdateDisplayAfterSearch(foundItem);
    }

    // '이진 탐색' 버튼에 연결할 public 함수
    public void OnBinarySearchButtonClicked()
    {
        // 이진 탐색은 정렬된 상태에서만 가능하므로, 아직 정렬되지 않았다면 정렬을 먼저 수행합니다.
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

    // 검색 결과에 따라 화면을 업데이트하는 함수
    private void UpdateDisplayAfterSearch(Item foundItem)
    {
        if (foundItem != null)
        {
            // 아이템을 찾았다면, 해당 아이템 하나만 목록에 표시합니다.
            DisplayItems(new List<Item> { foundItem });
        }
        else
        {
            // 아이템을 찾지 못했다면, 목록을 비웁니다.
            Debug.Log("Item not found.");
            DisplayItems(new List<Item>());
        }
    }

    #region Search Algorithms
    // 선형 탐색 알고리즘
    private Item FindItemLinear(string targetName)
    {
        foreach (Item item in shopItems)
        {
            if (item.itemName == targetName)
            {
                return item; // 찾으면 바로 반환
            }
        }
        return null; // 못 찾으면 null 반환
    }

    // 이진 탐색 알고리즘
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
                return shopItems[mid]; // 찾음
            }
            else if (comparison < 0)
            {
                left = mid + 1; // 중간값보다 크면 오른쪽 부분을 탐색
            }
            else
            {
                right = mid - 1; // 중간값보다 작으면 왼쪽 부분을 탐색
            }
        }
        return null; // 못 찾음
    }
    #endregion
}