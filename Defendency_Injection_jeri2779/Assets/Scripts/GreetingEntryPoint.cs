using UnityEngine;
using VContainer.Unity;

public class GreetingEntryPoint : IStartable , ITickable
{

    private readonly GreetingService m_Greeting;

    public GreetingEntryPoint(GreetingService greeting)
    {
        m_Greeting = greeting;
    }
    public void Start()
    {
        m_Greeting.Greet();
    }

    public void Tick()
    {
        if(Time.frameCount % 60 == 0)
        {
            Debug.Log("test");
        }
    }
}
