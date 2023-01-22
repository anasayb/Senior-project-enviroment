using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeClass : IComparable<NodeClass>
{
    
    public int direction;
    public int CarCount;
    
    public NodeClass(int direction,int CarCount)
    {
        this.direction = direction;
        this.CarCount = CarCount;
      

    }

 
    public int CompareTo(NodeClass other)
    {
        if (this.CarCount > other.CarCount) return -1;
        else if (this.CarCount < other.CarCount) return 1;
        else return 0;
    }
    
    
}