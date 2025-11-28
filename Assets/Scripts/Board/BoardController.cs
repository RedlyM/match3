using MatchThree.Core;
using UnityEngine;
using UnityEngine.U2D;

using VContainer.Unity;

namespace MatchThree.Board
{
    public class BoardController : IInitializable
    {
        private readonly BoardView _view;
        private readonly BoardModel _model;
        private readonly GameplayConfig _config;
        private readonly Settings _settings;

        public BoardController(BoardView view, BoardModel model, GameplayConfig config, Settings settings)
        {
            _view = view;
            _model = model;
            _config = config;
            _settings = settings;
        }

        public void Initialize()
        {
            _model.Init(_settings.GridSize, _config.ElementSize,_view.transform.position);
            ConfigureBoardSize();
            PlaceMask();
        }

        private void ConfigureBoardSize()
        {
            Bounds bounds = _model.Bounds;
            Vector3 offset = _view.transform.position;

            Spline spline = _view.SpriteShape.spline;
            spline.SetPosition(0, new Vector3(bounds.min.x, bounds.min.y) - offset);
            spline.SetPosition(1, new Vector3(bounds.min.x, bounds.max.y) - offset);
            spline.SetPosition(2, new Vector3(bounds.max.x, bounds.max.y) - offset);
            spline.SetPosition(3, new Vector3(bounds.max.x, bounds.min.y) - offset);
        }

        private void PlaceMask()
        {
            float posY = _model.Bounds.max.y + _view.Mask.localScale.y * 0.5f;
            _view.Mask.position = new Vector2(_view.Mask.position.x, posY);
        }
    }
}