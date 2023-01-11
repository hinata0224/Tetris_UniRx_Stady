using UnityEngine;
using UniRx;
using Other_Script;
using FiledData;
using System;
using System.Collections.Generic;

namespace Tetris_Block
{
    public class BlockController : MonoBehaviour
    {
        private float downInterval;

        private float holedInterval;

        private bool endBlock = false;

        private Transform grid;

        private ReactiveProperty<float> x_value = new(0);
        private ReactiveProperty<float> y_value = new(0);

        private Subject<Unit> nextCreate = new();
        private Subject<Unit> gameOverFlag = new();

        private CompositeDisposable disposables = new();

        private TimerModel timer = new();
        private TimerModel holedTimer = new();

        private FieldData model = new();

        //初期化
        public void Init(float down, float holed, Transform _grid)
        {
            downInterval= down;
            holedInterval= holed;
            grid = _grid;

            x_value.Where(x => x != 0)
                .Subscribe(x => BlockMove(x))
                .AddTo(disposables);

            y_value.Where(x => x != 0)
                .Subscribe(x => RotateBlock(x))
                .AddTo(disposables);

            y_value.Where(x => x == 0 && holedTimer != null)
                .Subscribe(_ => holedTimer.EndTimer())
                .AddTo(disposables);

            timer.GetEndTimer()
                .Subscribe(_ => DownBlock());

            timer.GetEndTimer()
                .Where(_ => endBlock)
                .Subscribe(_ =>
                {
                    SetPosition();
                });

            timer.SetTimer(downInterval);

            //ブロックが落ち始めるときに他のブロックと重なっていたり、フィールド外ならゲームオーバー
            if (!ValicMovement())
            {
                gameOverFlag.OnNext(Unit.Default);
            }

        }

        //左右の移動
        private void BlockMove(float x)
        {
            timer.RestertTimer();
            if (Mathf.Sign(x) == 1)
            {
                transform.position += new Vector3(1, 0, 0);
                if (!ValicMovement())
                {
                    transform.position += new Vector3(-1, 0, 0);
                }
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
                if (!ValicMovement())
                {
                    transform.position += new Vector3(1, 0, 0);
                }
            }
            x_value.Value = 0;
        }

        //上ボタンの場合回転下ボタンの場合下に落とす
        private void RotateBlock(float y)
        {
            timer.RestertTimer();
            if (Mathf.Sign(y) == 1)
            {
                transform.RotateAround(transform.position, new Vector3(0, 0, 90), 90);
                if (!ValicMovement())
                {
                    transform.RotateAround(transform.position, new Vector3(0, 0, 90), -90);
                }
                y_value.Value = 0;
            }
            else
            {
                if (!endBlock)
                {
                    transform.position += new Vector3(0, -1, 0);
                    if (!ValicMovement())
                    {
                        transform.position += new Vector3(0, 1, 0);
                    }
                    holedTimer = new TimerModel();
                    //長押ししていたら落ち続ける
                    holedTimer.GetEndTimer()
                        .Where(_ => !endBlock)
                        .Subscribe(_ =>
                        {
                            DownBlock();
                            holedTimer.RestertTimer();
                        }).AddTo(this);

                    holedTimer.SetTimer(holedInterval);
                }
            }
        }

        //ブロックを下に落とす
        private void DownBlock()
        {
            endBlock= true;
            transform.position += new Vector3(0, -1, 0);
            if (!ValicMovement())
            {
                transform.position += new Vector3(0, 1, 0);
            }
            else
            {
                endBlock = false;
            }
            timer.RestertTimer();
        }

        //ブロックがフィールドにあるかどうかフィールド外かブロックがあればfalseを返す。
        private bool ValicMovement()
        {
            foreach (Transform children in transform)
            {
                int roundX = Mathf.RoundToInt(children.transform.position.x);
                int roundY = Mathf.RoundToInt(children.transform.position.y);

                if (roundX < 0 || roundX >= model.GetWidht() || roundY < 0)
                {
                    return false;
                }

                if (!model.CheckGrid(roundX, roundY))
                {
                    return false;
                }
            }

            return true;
        }

        public void InputValue(Vector2 vec)
        {
            x_value.Value = vec.x;
            y_value.Value = vec.y;
        }

        //子オブジェクトをGridに保存してこの親オブジェクトを削除
        private void SetPosition()
        {
            timer.EndTimer();
            int count = transform.childCount;
            for (int i = count -1; i >= 0; i--)
            {
                model.SetGrid(transform.GetChild(i));
                transform.GetChild(i).parent = grid;
            }

            //一列そろっているかの確認とそろっているラインの高さを保管
            List<int> lineCount = new(5);
            for (int i = model.GetHeight() - 1; i >= 0; i--)        //上から確認する
            {
                if (model.CheckLine(i))
                {
                    lineCount.Add(i);
                }
            }
            //ブロックの削除と削除した列を一段下げる
            if(lineCount.Count > 0)
            {
                List<GameObject> tempObj = model.DeLeteLine(lineCount);
                foreach(GameObject obj in tempObj)
                {
                    Destroy(obj);
                }
                model.RowDown(lineCount);
            }

            nextCreate.OnNext(Unit.Default);

        }

        public IObservable<Unit> GetNextCreate()
        {
            return nextCreate.AddTo(disposables);
        }

        public IObservable<Unit> GetGameOverFlag()
        {
            return gameOverFlag.AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}