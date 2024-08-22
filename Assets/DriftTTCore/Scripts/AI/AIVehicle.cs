using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle {
    
    public int Node;
    public string Name;
    public bool HasFinished;

    public AIVehicle(int node,string name,bool hasFinished){
        Node = node;
        Name = name;
        HasFinished =hasFinished;
    }
}
