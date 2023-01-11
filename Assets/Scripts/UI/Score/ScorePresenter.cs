using UnityEngine;
using FiledData;
using UniRx;

namespace Tetris_UI
{
    public class ScorePresenter : MonoBehaviour
    {
        private FieldData data = new();

        [SerializeField]
        private ScoreVew score;

        void Start()
        {
            data.score
                .Subscribe(x => score.SetScore(x))
                .AddTo(this);
        }
    }
}