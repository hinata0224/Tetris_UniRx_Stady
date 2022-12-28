using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiledData
{
    public class FieldData
    {
        private static int widht = 10;
        private static int height = 15;

        private static Transform[,] grid = new Transform[widht, height];

        //フィールドにブロックを置いたときにブロックの情報を保存
        public void SetGrid(Transform block)
        {
            Vector3 pos = block.position;
            grid[(int)pos.x, (int)pos.y] = block;
        }

        //その場所にブロックがあるかの確認
        public bool CheckGrid(int x_value,int y_value)
        {
            if (grid[x_value,y_value] != null)
            {
                return false;
            }
            return true;
        }

        public int GetWidht()
        {
            return widht;
        }
    }
}
