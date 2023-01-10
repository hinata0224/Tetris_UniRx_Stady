using UnityEngine;
using TMPro;

namespace Tetris_UI
{
    public class ResultController : MonoBehaviour
    {
        [SerializeField, Header("リザルトの親オブジェクト")]
        private GameObject rezult;

        [SerializeField, Header("リザルトスコア")]
        private TextMeshProUGUI scoreText;

        private ScoreModel model = new();

        private void Awake()
        {
            rezult.SetActive(false);
        }

        public void GameOver()
        {
            rezult.SetActive(true);
            scoreText.text = "今回のスコアは" + model.GetScorePoint() + "です。";
        }
    }
}