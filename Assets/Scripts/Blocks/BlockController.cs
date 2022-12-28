using UnityEngine;
using UniRx;
using Other_Script;
using FiledData;

namespace Tetris_Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField]
        private float downInterval = 1f;

        [SerializeField]
        private float holedInterval = 0.5f;

        private bool endBlock = false;

        [SerializeField]
        private Transform grid;

        private ReactiveProperty<float> x_value = new(0);
        private ReactiveProperty<float> y_value = new(0);

        private CompositeDisposable disposables = new();

        private TimerModel timer = new();
        private TimerModel holedTimer = new();

        private FieldData model = new();

        void Start()
        {
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

        private void DownBlock()
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValicMovement())
            {
                transform.position += new Vector3(0, 1, 0);
                endBlock = true;
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
            foreach(Transform chiledren in transform)
            {
                int x_value = Mathf.RoundToInt(chiledren.position.x);
                int y_value = Mathf.RoundToInt(chiledren.position.y);

                //画面内かの判定
                if(x_value < 0 || x_value >= model.GetWidht() || y_value < 0)
                {
                    return false;
                }

                if(!model.CheckGrid(x_value, y_value))
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

        private void SetPosition()
        {
            timer.EndTimer();

            foreach(Transform chiledren in transform)
            {
                model.SetGrid(chiledren.transform);
                chiledren.parent = grid;
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}