using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SingletonALifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        
        builder.RegisterComponentInHierarchy<UiSingleton>();
    }
}
