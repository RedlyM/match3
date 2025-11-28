using System;
using System.Threading;

using UnityEngine;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using VContainer.Unity;

using MatchThree.Core;
using MatchThree.UI;

namespace MatchThree.Menu
{
    public class MenuController : IInitializable, IDisposable
    {
        private readonly MenuView _view;
        private readonly LoadingScreen _loadingScreen;
        private readonly Settings _settings;

        private CancellationTokenSource _lifetimeCts;

        public MenuController(MenuView view, LoadingScreen loadingScreen, Settings settings)
        {
            _loadingScreen = loadingScreen;
            _settings = settings;
            _view = view;
        }

        public void Initialize()
        {
            _lifetimeCts = new CancellationTokenSource();
            _settings.GridSize = new Vector2Int(8, 6);
            UpdateGridData();

            _loadingScreen.Hide();

            _view.Play.OnClickAsAsyncEnumerable(_lifetimeCts.Token).SubscribeAwait(LoadGameplayAsync);
            _view.IncreaseRows.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => AddSize(1, 0));
            _view.DecreaseRows.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => AddSize(-1, 0));
            _view.IncreaseColumns.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => AddSize(0, 1));
            _view.DecreaseColumns.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => AddSize(0, -1));
        }

        public void Dispose()
        {
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        private async UniTask LoadGameplayAsync(AsyncUnit _, CancellationToken token)
        {
            _loadingScreen.Show();
            await SceneManager.LoadSceneAsync(2);
        }

        private void AddSize(int rows, int columns)
        {
            var newY = Mathf.Clamp(_settings.GridSize.y + rows, 4, 8);
            var newX = Mathf.Clamp(_settings.GridSize.x + columns, 4, 8);
            _settings.GridSize = new Vector2Int(newX, newY);

            UpdateGridData();
        }

        private void UpdateGridData()
        {
            _view.GridSize.text = $"{_settings.GridSize.x}x{_settings.GridSize.y}";
        }
    }
}