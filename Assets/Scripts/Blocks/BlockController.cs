using UnityEngine;
using UniRx;
using Other_Script;

namespace Tetris_Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField]
        private float downInterval = 1f;

        [SerializeField]
        private float holedInterval = 0.5f;

        private ReactiveProperty<float> x_value = new(0);
        private ReactiveProperty<float> y_value = new(0);

        private CompositeDisposable disposables = new();

        private TimerModel timer = new();
        private TimerModel holedTimer = new();

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

            timer.SetTimer(downInterval);
        }

        private void BlockMove(float x)
        {
            timer.RestertTimer();
            if (Mathf.Sign(x) == 1)
            {
                transform.position += new Vector3(1, 0, 0);
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
            x_value.Value = 0;
        }

        private void RotateBlock(float y)
        {
            timer.RestertTimer();
            if (Mathf.Sign(y) == 1)
            {
                transform.RotateAround(transform.position, new Vector3(0, 0, 90), 90);
                y_value.Value = 0;
            }
            else
            {
                transform.position += new Vector3(0, -1, 0);
                holedTimer = new TimerModel();

                holedTimer.GetEndTimer()
                    .Subscribe(_ =>
                    {
                        DownBlock();
                        holedTimer.RestertTimer();
                    }).AddTo(this);

                holedTimer.SetTimer(holedInterval);
            }
        }

        private void DownBlock()
        {
            transform.position += new Vector3(0, -1, 0);
            timer.RestertTimer();
        }

        public void InputValue(Vector2 vec)
        {
            x_value.Value = vec.x;
            y_value.Value = vec.y;
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}