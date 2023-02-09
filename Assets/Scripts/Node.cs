using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// this class is dedicated for the Priority Queue to carry the direction with its car load
public class NodeClass : IComparable<NodeClass>
{
    
    public int direction;
    public int CarCount;
    
    public NodeClass(int direction,int CarCount)
    {
        this.direction = direction;
        this.CarCount = CarCount;
      

    }

    //this function prioritize based on car number
    public int CompareTo(NodeClass other)
    {
        if (this.CarCount > other.CarCount) return -1;
        else if (this.CarCount < other.CarCount) return 1;
        else return 0;
    }
    
    
}