using UnityEngine;

using VContainer;
using VContainer.Unity;

using MatchThree.UI;

namespace MatchThree.Core
{
    public class ServicesLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private LoadingScreen _loadingScreen;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(_loadingScreen);
            DontDestroyOnLoad(this);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_loadingScreen);

            builder.Register<Settings>(Lifetime.Singleton);
            builder.RegisterEntryPoint<Launcher>();
        }
    }
}