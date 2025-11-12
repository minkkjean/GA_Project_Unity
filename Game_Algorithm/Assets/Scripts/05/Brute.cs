using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Brute : MonoBehaviour
{
    [Header("카드 데이터")]
    public CardData quickShotData;
    public CardData heavyShotData;
    public CardData multiShotData;
    public CardData tripleShotData;

    [Header("코스트")]
    public int maxCost = 15;

    void Start()
    {
        StartCoroutine(FindBestComboRoutine());
    }
    IEnumerator FindBestComboRoutine()
    {
        int bestDamage = 0;
        int bestCost = 0;
        string bestCombo = "";

        for (int q = 0; q <= quickShotData.Count; q++)
        {
            for (int h = 0; h <= heavyShotData.Count; h++)
            {
                for (int m = 0; m <= multiShotData.Count; m++)
                {
                    for (int t = 0; t <= tripleShotData.Count; t++)
                    {
                        int currentCost = (q * quickShotData.cost) + (h * heavyShotData.cost) + (m * multiShotData.cost) + (t * tripleShotData.cost);

                        int currentDamage = (q * quickShotData.damage) + (h * heavyShotData.damage) + (m * multiShotData.damage) +(t * tripleShotData.damage);

                        if (currentCost <= maxCost)
                        {
                            if (currentDamage > bestDamage)
                            {
                                bestDamage = currentDamage;
                                bestCost = currentCost;
                                bestCombo = $"퀵샷 {q}장, 헤비샷 {h}장, 멀티샷 {m}장, 트리플샷 {t}장";
                            }
                        }
                    } 
                }
            }
        }

        Debug.Log($"카드 조합: {bestCombo}");
        Debug.Log($"최대 데미지: {bestDamage} (코스트 {bestCost} 사용)");
        yield break;
    }
}