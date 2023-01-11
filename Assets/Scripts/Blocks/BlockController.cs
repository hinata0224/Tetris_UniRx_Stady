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

        //������
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

            //�u���b�N�������n�߂�Ƃ��ɑ��̃u���b�N�Əd�Ȃ��Ă�����A�t�B�[���h�O�Ȃ�Q�[���I�[�o�[
            if (!ValicMovement())
            {
                gameOverFlag.OnNext(Unit.Default);
            }

        }

        //���E�̈ړ�
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

        //��{�^���̏ꍇ��]���{�^���̏ꍇ���ɗ��Ƃ�
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
                    //���������Ă����痎��������
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

        //�u���b�N�����ɗ��Ƃ�
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

        //�u���b�N���t�B�[���h�ɂ��邩�ǂ����t�B�[���h�O���u���b�N�������false��Ԃ��B
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

        //�q�I�u�W�F�N�g��Grid�ɕۑ����Ă��̐e�I�u�W�F�N�g���폜
        private void SetPosition()
        {
            timer.EndTimer();
            int count = transform.childCount;
            for (int i = count -1; i >= 0; i--)
            {
                model.SetGrid(transform.GetChild(i));
                transform.GetChild(i).parent = grid;
            }

            //��񂻂���Ă��邩�̊m�F�Ƃ�����Ă��郉�C���̍�����ۊ�
            List<int> lineCount = new(5);
            for (int i = model.GetHeight() - 1; i >= 0; i--)        //�ォ��m�F����
            {
                if (model.CheckLine(i))
                {
                    lineCount.Add(i);
                }
            }
            //�u���b�N�̍폜�ƍ폜���������i������
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