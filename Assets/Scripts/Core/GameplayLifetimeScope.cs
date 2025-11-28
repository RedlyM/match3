using UnityEngine;
using VContainer;

using VContainer.Unity;

using MatchThree.Board;
using MatchThree.Core;
using MatchThree.Element;
using MatchThree.Match;
using MatchThree.Spawning;

namespace MatchThree
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GameplayConfig _config;

        [SerializeField]
        private BoardView _boardView;

        [SerializeField]
        private MatchView _matchView;

        protected override void Configure(IContainerBuilder builder)
        {
            ItemProvider itemProvider = new ItemProvider(_config.Elements);
            FactoryByIdentifier<MatchElement> factory = new FactoryByIdentifier<MatchElement>(itemProvider);
            PoolWithFactory<MatchElement> pool = new PoolWithFactory<MatchElement>(factory);

            builder.RegisterInstance(pool);

            builder.RegisterComponent(_config);

            builder.RegisterComponent(_boardView);
            builder.Register<BoardModel>(Lifetime.Scoped);

            builder.RegisterComponent(_matchView);
            builder.Register<MatchModel>(Lifetime.Scoped);

            builder.RegisterEntryPoint<BoardController>();
            builder.RegisterEntryPoint<MatchController>();
        }
    }
}