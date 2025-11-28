using UnityEngine;
using VContainer;

using VContainer.Unity;

using MatchThree.Board;
using MatchThree.Core;
using MatchThree.Element;
using MatchThree.Match;
using MatchThree.Spawning;
using MatchThree.Utils;
using MatchThree.UI;

namespace MatchThree
{
    public class MatchLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GameplayConfig _config;

        [SerializeField]
        private BoardView _boardView;

        [SerializeField]
        private MatchView _matchView;

        [SerializeField]
        private ScoreboardView _scoreboardView;

        [SerializeField]
        private PauseView _pauseView;

        protected override void Configure(IContainerBuilder builder)
        {
            ItemProvider itemProvider = new ItemProvider(_config.Elements);
            FactoryByIdentifier<MatchElement> factory = new FactoryByIdentifier<MatchElement>(itemProvider);
            PoolWithFactory<MatchElement> pool = new PoolWithFactory<MatchElement>(factory);

            builder.RegisterInstance(pool);

            builder.RegisterComponent(_config);
            builder.RegisterComponent(_scoreboardView);
            builder.RegisterComponent(_pauseView);
            builder.RegisterComponent(_boardView);
            builder.RegisterComponent(_matchView);

            builder.Register<EventBus>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<BoardModel>(Lifetime.Scoped);
            builder.Register<MatchModel>(Lifetime.Scoped);
            builder.Register<ScoreboardModel>(Lifetime.Scoped);

            builder.RegisterEntryPoint<ScoreboardController>();
            builder.RegisterEntryPoint<PauseController>();
            builder.RegisterEntryPoint<BoardController>();
            builder.RegisterEntryPoint<MatchController>();
        }
    }
}