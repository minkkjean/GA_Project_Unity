using UnityEngine;

[CreateAssetMenu(fileName = "Cardname", menuName = "Card/Card Data")]
public class CardData : ScriptableObject
{
    public CardType type; 
    public int damage;
    public int cost;
    public int Count;
}