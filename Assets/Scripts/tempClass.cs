using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempClass : MonoBehaviour
{
    private PriorityQueue<NodeClass> queue = new PriorityQueue<NodeClass>();

    // Start is called before the first frame update
    void Start()
    {
        NodeClass x = new NodeClass(0, 12);
        NodeClass y = new NodeClass(1, 2);
        NodeClass z = new NodeClass(2, 13);
        NodeClass i = new NodeClass(3, 11);
        queue.Enqueue(x);
        queue.Enqueue(z);
        queue.Enqueue(y);
        queue.Enqueue(i);
        Debug.Log(queue.Peek().CarCount);
        queue.Dequeue();
        Debug.Log(queue.Peek().CarCount);
        queue.Dequeue();
        Debug.Log(queue.Peek().CarCount);
        queue.Dequeue();
        Debug.Log(queue.Peek().CarCount);
        queue.Dequeue();

    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
    
