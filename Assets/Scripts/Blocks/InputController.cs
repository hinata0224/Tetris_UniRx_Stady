using Tetris_Block;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BlockInput
{
    public class InputController : MonoBehaviour
    {
        private BlockController controller;

        public void GetPlayerInput(InputAction.CallbackContext contex)
        {
            if (contex.started)
            {
                Vector2 vec = new();

                vec.x = contex.ReadValue<Vector2>().x;
                vec.y = contex.ReadValue<Vector2>().y;

                controller.InputValue(vec);
            }
            else if (contex.canceled)
            {
                Vector2 vec = new(0,0);

                controller.InputValue(vec);
            }
        }

        //ブロックの生成時にセットする
        public void SetController(BlockController _controller)
        {
            controller = _controller;
        }
    }
}