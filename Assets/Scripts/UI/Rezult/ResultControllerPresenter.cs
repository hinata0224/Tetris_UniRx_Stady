using UnityEngine;
using UniRx;
using Tetris_Block;
using FiledData;

namespace Tetris_UI
{
    public class ResultControllerPresenter : MonoBehaviour
    {
        [SerializeField, Header("参照スクリプト")]
        private BlockManager block;

        [SerializeField]
        private ResultController controller;

        private FieldData data = new();

        void Start()
        {
            block.GetGameOverFlag()
                .First()
                .Subscribe(x => controller.GameOver(data.score.Value));
        }
    }
}