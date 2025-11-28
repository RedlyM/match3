using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField] private RectTransform _pausePanel;
        [SerializeField] private float _appearanceDuration;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private TMP_Text _sfxText;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private TMP_Text _musicText;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        public Toggle SfxToggle => _sfxToggle;

        public Toggle MusicToggle => _musicToggle;

        public Button PauseButton => _pauseButton;

        public Button CloseButton => _closeButton;

        public Button RestartButton => _restartButton;

        public Button QuitButton => _quitButton;

        public UniTask ShowAsync(CancellationToken token)
        {
            return SetPanelStateAsync(true, token);
        }

        public UniTask HideAsync(CancellationToken token)
        {
            return SetPanelStateAsync(false, token);
        }

        public void ToggleSfx(bool state)
        {
            _sfxText.gameObject.SetActive(!state);
        }

        public void ToggleMusic(bool state)
        {
            _musicText.gameObject.SetActive(!state);
        }

        private async UniTask SetPanelStateAsync(bool state, CancellationToken token)
        {
            Vector3 startScale = state ? Vector3.zero : Vector3.one;
            Vector3 endScale = state ? Vector3.one : Vector3.zero;

            _pausePanel.localScale = startScale;

            if (state)
            {
                _pausePanel.gameObject.SetActive(true);
            }

            await _pausePanel.DOScale(endScale, _appearanceDuration).AwaitForComplete(cancellationToken: token);

            if (!state)
            {
                _pausePanel.gameObject.SetActive(false);
            }
        }
    }
}