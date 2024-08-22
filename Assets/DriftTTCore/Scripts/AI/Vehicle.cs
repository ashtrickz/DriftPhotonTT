using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle {
    public int node;
    public string name;
    public bool hasFinished;

    public Vehicle(int node,string name,bool HasFinished){
        this.node = node;
        this.name = name;
        hasFinished =HasFinished;
    }
}
