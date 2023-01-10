using UnityEngine;
using UniRx;
using Tetris_Block;

namespace Tetris_UI
{
    public class ResultControllerPresenter : MonoBehaviour
    {
        [SerializeField, Header("参照スクリプト")]
        private BlockManager block;

        [SerializeField]
        private ResultController controller;

        void Start()
        {
            block.GetGameOverFlag()
                .First()
                .Subscribe(x => controller.GameOver());
        }
    }
}