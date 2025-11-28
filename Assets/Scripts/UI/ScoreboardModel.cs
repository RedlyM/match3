using Cysharp.Threading.Tasks;

namespace MatchThree.UI
{
    public class ScoreboardModel
    {
        private const int SCORE_PER_MATCH = 10;

        public IReadOnlyAsyncReactiveProperty<int> Score => _score;

        private AsyncReactiveProperty<int> _score;

        public ScoreboardModel()
        {
            _score =  new AsyncReactiveProperty<int>(0);
        }

        public void AddMatch(int matchCount, int matchLength)
        {
            _score.Value += matchCount * matchLength *  SCORE_PER_MATCH;
        }
    }
}