using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQueueTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var queue = new SimplePriorityQueue<string>();
        queue.Enqueue("Player A", 5);
        queue.Enqueue("Player B", 15);
        queue.Enqueue("Player C", 0);

        while(queue.Count > 0)
        {
            Debug.Log(queue.Dequeue());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
