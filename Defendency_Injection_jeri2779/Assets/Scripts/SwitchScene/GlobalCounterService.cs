using UnityEngine;
using DIStudy.LifetimeLab;


public class GlobalCounterService 
{
    public string Id {get;} = InstanceIdFactory.Next("D1");

    public int Count {get; private set;}

    public void Add(int amount) => Count += amount;

    
 
}
