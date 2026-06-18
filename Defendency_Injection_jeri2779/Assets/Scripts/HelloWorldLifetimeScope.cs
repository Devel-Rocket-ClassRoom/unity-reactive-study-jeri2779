using VContainer;
using VContainer.Unity;

public class HelloWorldLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<GreetingService>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GreetingEntryPoint>();
    }
}
