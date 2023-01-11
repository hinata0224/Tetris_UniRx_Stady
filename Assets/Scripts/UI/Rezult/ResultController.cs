using UnityEngine;
using TMPro;


namespace Tetris_UI
{
    public class ResultController : MonoBehaviour
    {
        [SerializeField, Header("���U���g�̐e�I�u�W�F�N�g")]
        private GameObject rezult;

        [SerializeField, Header("���U���g�X�R�A")]
        private TextMeshProUGUI scoreText;


        private void Awake()
        {
            rezult.SetActive(false);
        }

        public void GameOver(int _score)
        {
            rezult.SetActive(true);
            scoreText.text = "����̃X�R�A��" + _score + "�ł��B";
        }
    }
}