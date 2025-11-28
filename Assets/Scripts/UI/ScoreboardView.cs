using UnityEngine;

using TMPro;

namespace MatchThree.UI
{
    public class ScoreboardView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _scoreText;

        public void SetScoreText(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}