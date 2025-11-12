using System.Collections.Generic;
using UnityEngine;
public class ActionTicket
{
    public string UnitName;
    public float ActionTime;
}

public class Test : MonoBehaviour
{
    private SimplePriorityQueue<ActionTicket> actionQueue = new SimplePriorityQueue<ActionTicket>();
    private Dictionary<string, int> unitSpeeds = new Dictionary<string, int>();
    private int turnCount = 1;

    void Start()
    {
        unitSpeeds.Add("전사", 5);
        unitSpeeds.Add("마법사", 7);
        unitSpeeds.Add("궁수", 11);
        unitSpeeds.Add("도적", 12);

        foreach (var unit in unitSpeeds)
        {
            float initialActionTime = 100f / unit.Value;

            ActionTicket newTicket = new ActionTicket();
            newTicket.UnitName = unit.Key;
            newTicket.ActionTime = initialActionTime;

            actionQueue.Enqueue(newTicket, newTicket.ActionTime);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleTurn();
        }
    }

    void HandleTurn()
    {
        ActionTicket firstTicket = actionQueue.Dequeue();
        float currentTime = firstTicket.ActionTime;

        List<ActionTicket> readyUnits = new List<ActionTicket>();
        List<ActionTicket> futureUnits = new List<ActionTicket>();

        readyUnits.Add(firstTicket);

        while (actionQueue.Count > 0)
        {
            ActionTicket nextTicket = actionQueue.Dequeue();
            if (nextTicket.ActionTime == currentTime)
            {
                readyUnits.Add(nextTicket);
            }
            else
            {
                futureUnits.Add(nextTicket);
            }
        }

        if (readyUnits.Count > 1)
        {
            readyUnits.Sort((ticketA, ticketB) => unitSpeeds[ticketB.UnitName].CompareTo(unitSpeeds[ticketA.UnitName]));
        }

        foreach (ActionTicket ticket in readyUnits)
        {
            Debug.Log($"{turnCount}턴 / {ticket.UnitName}의 턴입니다.");
            turnCount++;

            int speed = unitSpeeds[ticket.UnitName];
            float nextActionTime = currentTime + (100f / speed);

            ActionTicket nextTurnTicket = new ActionTicket();
            nextTurnTicket.UnitName = ticket.UnitName;
            nextTurnTicket.ActionTime = nextActionTime;

            futureUnits.Add(nextTurnTicket);
        }

        foreach (ActionTicket ticket in futureUnits)
        {
            actionQueue.Enqueue(ticket, ticket.ActionTime);
        }
    }
}