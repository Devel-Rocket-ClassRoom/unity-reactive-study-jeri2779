using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIStudy.WhackAMole
{
    public class MyWhackAMoleLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private MyGameConfig m_Config = new MyGameConfig();

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(m_Config);

            builder.Register<IScoreService, MyScoreService>(Lifetime.Singleton);
            builder.Register<IHighScoreService, MyPlayerPrefsHighScoreService>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<MyAudioManager>().As<IAudioService>();
            builder.RegisterComponentInHierarchy<MyMoleSpawner>();
            builder.RegisterComponentInHierarchy<MyGameHudController>();

            builder.RegisterEntryPoint<MyGameDirector>().AsSelf();
        }
    }
}
