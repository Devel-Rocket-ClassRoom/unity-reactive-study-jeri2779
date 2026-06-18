using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppRootLifetimeScope : LifetimeScope
{
    private static AppRootLifetimeScope s_Instance;

    // 도메인 리로드를 끈 상태에서도 Play 재시작 시 정적 필드를 비운다.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() => s_Instance = null;

    protected override void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_Instance = this;
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected override void Configure(IContainerBuilder builder)
    {
         
        builder.Register<GlobalCounterService>(Lifetime.Singleton);
    }
}
