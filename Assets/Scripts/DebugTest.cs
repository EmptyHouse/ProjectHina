using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    private void Awake()
    {
        RunHeapTest();
    }

    public void RunHeapTest()
    {
        EHPriorityQueue<int> MaxHeap = new EHPriorityQueue<int>();

        
        for (int i = 0; i < 1000; ++i)
        {
            MaxHeap.Push(Random.Range(0, 10000));
        }

        int j = 0;
        while (!MaxHeap.IsEmpty())
        {
            print((++j).ToString() + " " + MaxHeap.Pop());
        }
    }
}
