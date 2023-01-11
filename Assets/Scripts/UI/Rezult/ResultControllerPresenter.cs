using UnityEngine;
using UniRx;
using Tetris_Block;
using FiledData;

namespace Tetris_UI
{
    public class ResultControllerPresenter : MonoBehaviour
    {
        [SerializeField, Header("�Q�ƃX�N���v�g")]
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