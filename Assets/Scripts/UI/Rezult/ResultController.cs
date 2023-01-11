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


        private void Awake()
        {
            rezult.SetActive(false);
        }

        public void GameOver(int _score)
        {
            rezult.SetActive(true);
            scoreText.text = "今回のスコアは" + _score + "です。";
        }
    }
}