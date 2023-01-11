using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris_UI
{
    public class MenuUIModel
    {
        public void StopTime()
        {
            Time.timeScale = 0f;
        }

        public void StartTime()
        {
            Time.timeScale = 1f;
        }
    }
}