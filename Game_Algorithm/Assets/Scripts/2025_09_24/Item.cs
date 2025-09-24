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

    // ���� Ž�� �� ������ �̸��� �������� �����ϰ� ���ϱ� ���� �ʿ��� �Լ�
    public int CompareTo(Item other)
    {
        if (other == null) return 1;
        // string Ŭ������ CompareTo�� ����� �̸��� ��
        return this.itemName.CompareTo(other.itemName);
    }
}