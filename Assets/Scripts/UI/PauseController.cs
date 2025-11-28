using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using VContainer.Unity;
using MatchThree.Core;
using UnityEngine.SceneManagement;

namespace MatchThree.UI
{
    public class PauseController : IInitializable, IDisposable
    {
        private readonly PauseView _view;
        private readonly Settings _settings;
        private readonly LoadingScreen _loadingScreen;
        private CancellationTokenSource _lifetimeCts;

        public PauseController(PauseView view, Settings settings, LoadingScreen loadingScreen)
        {
            _view = view;
            _settings = settings;
            _loadingScreen = loadingScreen;
        }

        public void Initialize()
        {
            _lifetimeCts = new CancellationTokenSource();

            _view.PauseButton.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => SetPanelState(true));
            _view.CloseButton.OnClickAsAsyncEnumerable(_lifetimeCts.Token).Subscribe(_ => SetPanelState(false));
            _view.RestartButton.OnClickAsAsyncEnumerable(_lifetimeCts.Token).SubscribeAwait(RestartAsync);
            _view.QuitButton.OnClickAsAsyncEnumerable(_lifetimeCts.Token).SubscribeAwait(LoadMenuAsync);

            _view.SfxToggle.isOn = _settings.SfxState;
            _view.MusicToggle.isOn = _settings.MusicState;

            _view.SfxToggle.OnValueChangedAsAsyncEnumerable(_lifetimeCts.Token)
                .Subscribe(state => _view.ToggleSfx(state));
            _view.MusicToggle.OnValueChangedAsAsyncEnumerable(_lifetimeCts.Token)
                .Subscribe(state => _view.ToggleMusic(state));
        }

        public void Dispose()
        {
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        private void SetPanelState(bool state)
        {
            if (state)
            {
                _view.ShowAsync(_lifetimeCts.Token).Forget();
            }
            else
            {
                _view.HideAsync(_lifetimeCts.Token).Forget();
            }
        }

        private async UniTask RestartAsync(AsyncUnit _, CancellationToken token)
        {
            _loadingScreen.Show();

            await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        private async UniTask LoadMenuAsync(AsyncUnit _, CancellationToken token)
        {
            _loadingScreen.Show();

            await SceneManager.LoadSceneAsync(1);
        }
    }
}