using UnityEngine;
using R3;

public class HelloR3 : MonoBehaviour
{
    private void Start()
    {
        Observable
        .Range(1,4)
        .Subscribe(x => Debug.Log($"받음: {x}"));
        
    }
}
   
