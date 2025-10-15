using System.Collections.Generic; 
using System.Linq;               
using UnityEngine;

public class BattleManager : MonoBehaviour
{
   
    private List<Unit> allUnits;

    
    private Queue<Unit> turnOrderQueue;

    
    private int turnCount = 1;

    
    private bool isFirstRound = true;

    void Start()
    {
        
        allUnits = new List<Unit>();
        allUnits.Add(new Unit("전사", 5));
        allUnits.Add(new Unit("마법사", 7));
        allUnits.Add(new Unit("궁수", 10));
        allUnits.Add(new Unit("도적", 12));

        
        turnOrderQueue = new Queue<Unit>();

        
        SetNextRoundOrder();

        Debug.Log("전투 시작! 스페이스바를 눌러 턴을 진행하세요.");
    }

    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            if (turnOrderQueue.Count == 0)
            {
                
                SetNextRoundOrder();
            }

            
            Unit currentUnit = turnOrderQueue.Dequeue();

            
            Debug.Log($"{turnCount}턴 / {currentUnit.name} 의 턴입니다.");
            turnCount++;
        }
    }

    
    void SetNextRoundOrder()
    {
        List<Unit> orderedUnits;

        
        if (isFirstRound)
        {
            
            orderedUnits = allUnits.OrderBy(unit => Random.value).ToList();
            isFirstRound = false; 
        }
        
        else
        {
            
            orderedUnits = allUnits.OrderBy(unit => unit.speed).ToList();
        }

        
        foreach (Unit unit in orderedUnits)
        {
            turnOrderQueue.Enqueue(unit);
        }
    }
}