using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 꼭 필요합니다.

/**
 * 1. 카드 정보를 담을 '틀' (데이터 상자)
 */
public class Card
{
    public string Name;
    public int Damage;
    public int Cost;

    // 생성자: 카드를 쉽게 만들기 위한 함수
    public Card(string name, int damage, int cost)
    {
        Name = name;
        Damage = damage;
        Cost = cost;
    }

    // 디버깅 시 콘솔에 예쁘게 출력하기 위한 함수
    public override string ToString()
    {
        return $"[{Name}] (Dmg: {Damage}, Cost: {Cost})";
    }
}

/**
 * 2. 메인 로직이 들어갈 클래스
 */
public class ComboOptimizer : MonoBehaviour
{
    // AI의 최대 코스트
    private int maxCost = 15;

    // AI가 현재 손에 들고 있는 카드 목록 (총 6장)
    private List<Card> hand = new List<Card>
    {
        new Card("퀵 샷", 6, 2),
        new Card("퀵 샷", 6, 2),
        new Card("헤비 샷", 8, 3),
        new Card("헤비 샷", 8, 3),
        new Card("멀티 샷", 16, 5),
        new Card("트리플 샷", 24, 7)
    };

    // --- 탐색 결과를 저장할 변수들 ---
    private int bestDamageFound = 0;
    private int costForBestDamage = 0;
    private List<Card> bestComboFound = new List<Card>();

    /**
     * 핵심 로직: 모든 조합을 탐색하는 재귀 함수
     */
    private void FindBestComboRecursive(int cardIndex, List<Card> currentCombo, int currentCost, int currentDamage)
    {
        // [종료 조건]
        if (cardIndex == hand.Count)
        {
            if (currentDamage > bestDamageFound)
            {
                bestDamageFound = currentDamage;
                costForBestDamage = currentCost;
                bestComboFound = new List<Card>(currentCombo);
            }
            return;
        }

        // --- [재귀 호출] ---
        Card currentCard = hand[cardIndex];

        // 선택 1: "이 카드를 쓰지 않는다"
        FindBestComboRecursive(cardIndex + 1, currentCombo, currentCost, currentDamage);

        // 선택 2: "이 카드를 쓴다" (코스트가 15를 넘지 않는 경우)
        if (currentCost + currentCard.Cost <= maxCost)
        {
            currentCombo.Add(currentCard);
            FindBestComboRecursive(
                cardIndex + 1,
                currentCombo,
                currentCost + currentCard.Cost,
                currentDamage + currentCard.Damage
            );
            currentCombo.RemoveAt(currentCombo.Count - 1);
        }
    }

    // 게임이 시작될 때 (Play 버튼 눌렀을 때) 실행됩니다.
    void Start()
    {
        UnityEngine.Debug.Log("--- AI 콤보 최적화 시작 ---");

        // 1. 탐색 시작
        FindBestComboRecursive(0, new List<Card>(), 0, 0);

        // 2. 탐색 완료 후 결과 출력
        UnityEngine.Debug.Log($"--- 탐색 완료 (총 64개 조합 확인) ---");
        UnityEngine.Debug.Log($"<color=cyan>최대 데미지: {bestDamageFound}</color>");
        UnityEngine.Debug.Log($"<color=yellow>사용한 코스트: {costForBestDamage} / {maxCost}</color>");

        UnityEngine.Debug.Log("--- 최적의 카드 조합 ---");
        if (bestComboFound.Count > 0)
        {
            foreach (Card card in bestComboFound)
            {
                UnityEngine.Debug.Log(card.ToString());
            }
        }
        else
        {
            UnityEngine.Debug.Log("사용할 수 있는 조합이 없습니다.");
        }
    }
}