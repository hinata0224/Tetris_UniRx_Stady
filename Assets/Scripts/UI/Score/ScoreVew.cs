using UnityEngine;
using TMPro;

namespace Tetris_UI
{
    public class ScoreVew : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;
        public void SetScore(int _score)
        {
            scoreText.text = "Score : " + _score;
        }
    }
}