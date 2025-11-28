using System;
using Cysharp.Threading.Tasks.Linq;
using Events;
using MatchThree.Utils;
using VContainer.Unity;

namespace MatchThree.UI
{
    public class ScoreboardController : IInitializable, IDisposable
    {
        private readonly ScoreboardView _view;
        private readonly ScoreboardModel _model;
        
        private readonly IEventBus _eventBus;

        private IDisposable _scoreSub;

        public ScoreboardController(ScoreboardView view, ScoreboardModel model, IEventBus eventBus)
        {
            _view = view;
            _model = model;
            _eventBus = eventBus;
        }

        public void Initialize()
        {
            _scoreSub = _model.Score.Subscribe(UpdateScoreText);
            _eventBus.Subscribe<MatchEvent>(AddMatch);
        }

        public void Dispose()
        {
            _scoreSub.Dispose();
            _eventBus.Unsubscribe<MatchEvent>(AddMatch);
        }

        private void AddMatch(MatchEvent match)
        {
            _model.AddMatch(match.Count, match.Length);
        }

        private void UpdateScoreText(int score)
        {
            _view.SetScoreText(score);
        }
    }
}