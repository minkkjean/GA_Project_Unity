using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyCoinSample : MonoBehaviour
{
    //코인 종류가 내림차순으로 정렬되어 있어야 함.
    int[] coinType = { 50, 40, 10 };

    void Start()
    {
        Debug.Log(CountCoins(80));
    }

    int CountCoins(int amount)
    {
        int count = 0;

        foreach (int c in coinType)
        {
            int use = amount / c; //해당 코인으로 줄 수 있는 최대 개수
            count += use;
            amount -= use * c;
        }

        return count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
