using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockInput;
using System;
using UniRx;
using System.Threading;
using Other_Script;

namespace Tetris_Block
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField, Header("落とす時間の感覚")]
        private float downInterval = 1f;

        [SerializeField,Header("次のミノの生成インターバル")]
        private float nextInterval = 1f;

        [SerializeField, Header("長押し時に落ちる早さ")]
        private float holedInterval = 0.5f;

        [SerializeField, Header("生成する場所")]
        private List<Transform> createPos;

        [SerializeField, Header("ブロックの親オブジェクト")]
        private Transform grid;

        [SerializeField, Header("生成するブロックのリスト(Prefabリスト)")]
        private List<GameObject> blocks;

        //表示するブロックの受け皿
        private List<GameObject> nextBlock = new(4);

        private bool instanceCheck = true;

        private Subject<Unit> gameOverFlag = new();

        [SerializeField,Header("参照スクリプト")]
        private InputController controller;

        private BlockController blockdata;

        void Start()
        {
            Init();
        }

        //Blockの生成だけの関数
        private void BlockInstance()
        {
            if (instanceCheck)
            {
                GameObject block = blocks[UnityEngine.Random.Range(0, blocks.Count)];
                GameObject obj = Instantiate(block);

                nextBlock.Add(obj);
            }
        }

        //Blockの並び替え
        private void SetBlockPosition()
        {
            for (int i = 0; i < nextBlock.Count; i++)
            {
                if (i == 0)
                {
                    blockdata = nextBlock[i].GetComponent<BlockController>();
                    controller.SetController(blockdata);

                    blockdata.GetGameOverFlag()
                        .Subscribe(_ =>
                        {
                            Debug.Log("zikkou");
                            instanceCheck = false;
                            gameOverFlag.OnNext(Unit.Default);
                        });
                    blockdata.GetNextCreate()
                        .Subscribe(_ =>
                        {
                            DeleteBlock();
                            TimerModel timer = new();
                            timer.GetEndTimer()
                            .Where(_ => instanceCheck)
                            .Subscribe(_ =>
                            {
                                BlockInstance();
                                SetBlockPosition();
                                timer.EndTimer();
                            });
                            timer.SetTimer(nextInterval);
                        });
                    blockdata.Init(downInterval, holedInterval, grid);

                }
                nextBlock[i].transform.position = createPos[i].position;
            }
        }

        private void DeleteBlock()
        {
            Destroy(blockdata.gameObject);
            nextBlock.RemoveAt(0);
        }

        private void Init()
        {
            for(int i = 0; i < createPos.Count; i++)
            {
                BlockInstance();
            }

            SetBlockPosition();
        }

        public IObservable<Unit> GetGameOverFlag()
        {
            return gameOverFlag;
        }
    }
}