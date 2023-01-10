using UnityEngine;
using TMPro;

namespace Tetris_UI
{
    public class ScoreModelController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;
        public void SetText(int score)
        {
            scoreText.text = "Score : " + score;
        }
    }
}