using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace FiledData
{
    public class FieldData
    {
        private static int widht = 10;
        private static int height = 15;

        private int addScore = 100;

        //スコアのデータを持つ
        public ReactiveProperty<int> score => _score;
        private static readonly IntReactiveProperty _score = new(0);

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

        //1ラインそろってるの確認
        public bool CheckLine(int height)
        {
            for (int i = 0; i < widht; i++)
            {
                if (grid[i, height] == null)
                {
                    return false;
                }
            }

            return true;
        }

        //そろったブロックを消す処理
        public List<GameObject> DeLeteLine(List<int> count)
        {
            List<GameObject> returnlist = new(20);
            for (int i = 0; i < count.Count; i++)
            {
                for (int j = 0; j < widht; j++)
                {
                    returnlist.Add(grid[j, count[i]].gameObject);
                    grid[j, count[i]] = null;
                }
                _score.Value += addScore;
            }
            return returnlist;
        }

        //ブロックを下に落とす処理
        public void RowDown(List<int> count)
        {
            for (int i = 0; i < count.Count; i++)
            {
                for (int h = count[i]; h < height; h++)
                {
                    for (int j = 0; j < widht; j++)
                    {
                        if (grid[j, h] != null)
                        {
                            grid[j, h - 1] = grid[j, h];
                            grid[j, h] = null;
                            grid[j, h - 1].transform.position -= new Vector3(0, 1, 0);
                        }
                    }
                }
            }
        }

        public int GetWidht()
        {
            return widht;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}
