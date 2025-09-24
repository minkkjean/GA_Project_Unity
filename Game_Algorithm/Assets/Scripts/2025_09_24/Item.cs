using System;

public class Item : IComparable<Item>
{
    public string itemName;
    public int quantity;

    public Item(string newName, int newQuantity)
    {
        itemName = newName;
        quantity = newQuantity;
    }

    // 이진 탐색 시 아이템 이름을 기준으로 정렬하고 비교하기 위해 필요한 함수
    public int CompareTo(Item other)
    {
        if (other == null) return 1;
        // string 클래스의 CompareTo를 사용해 이름을 비교
        return this.itemName.CompareTo(other.itemName);
    }
}