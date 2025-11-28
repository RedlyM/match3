using UnityEngine;

namespace MatchThree.Core
{
    public class Settings
    {
        private const string SFX_STATE_KEY = "settings_sfx_state";
        private const string MUSIC_STATE_KEY = "settings_music_state";

        public Vector2Int GridSize { get; set; }

        public bool SfxState
        {
            get => PlayerPrefs.GetInt(SFX_STATE_KEY, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(SFX_STATE_KEY, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public bool MusicState
        {
            get => PlayerPrefs.GetInt(MUSIC_STATE_KEY, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(MUSIC_STATE_KEY, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }
}