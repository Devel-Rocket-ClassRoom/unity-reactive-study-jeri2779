using UnityEngine;
using R3;
using System;

public class MyLifetimeProbe : MonoBehaviour
{
    
    void Start()
    {
        Observable
            .Interval(TimeSpan.FromSeconds(5))
            .Subscribe(_ => Debug.Log("MyLifeTime Probe"))
            .AddTo(this);
    }

  
}
