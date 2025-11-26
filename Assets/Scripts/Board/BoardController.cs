using UnityEngine;
using VContainer.Unity;

namespace MatchThree.Board
{
    public class BoardController : IInitializable
    {
        private readonly BoardView _view;
        private readonly BoardModel _model;

        public BoardController(BoardView view, BoardModel model)
        {
            _view = view;
            _model = model;
        }

        public void Initialize()
        {
            _model.Init();
            ConfigureBoardSize();
        }

        private void ConfigureBoardSize()
        {
            var bounds = _model.Bounds;

            var spline = _view.SpriteShape.spline;
            spline.SetPosition(0, new Vector3(bounds.min.x, bounds.min.y));
            spline.SetPosition(1, new Vector3(bounds.min.x, bounds.max.y));
            spline.SetPosition(2, new Vector3(bounds.max.x, bounds.max.y));
            spline.SetPosition(3, new Vector3(bounds.max.x, bounds.min.y));
        }

        
    }
}