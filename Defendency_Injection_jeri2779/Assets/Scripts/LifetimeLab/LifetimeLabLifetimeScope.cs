using VContainer;
using VContainer.Unity;

namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// 수명 실험실의 루트 스코프. 같은 모양의 서비스 셋을 수명만 다르게 등록한다.
    /// </summary>
    public class LifetimeLabLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SingletonService>(Lifetime.Singleton);
            builder.Register<ScopedService>(Lifetime.Scoped);
            builder.Register<TransientService>(Lifetime.Transient);
        }
    }
}
