using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockInput;
using System;
using UniRx;

namespace Tetris_Block
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField, Header("落とす時間の感覚")]
        private float downInterval = 1f;

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
            GameObject block = blocks[UnityEngine.Random.Range(0, blocks.Count)];
            GameObject obj = Instantiate(block);

            nextBlock.Add(obj);
        }

        //Blockの並び替え
        private void SetBlockPosition()
        {
            for (int i = 0; i < nextBlock.Count; i++)
            {
                if (i == 0)
                {
                    blockdata = nextBlock[i].GetComponent<BlockController>();
                    blockdata.Init(downInterval, holedInterval, grid);
                    controller.SetController(blockdata);

                    blockdata.GetNextCreate()
                        .Subscribe(_ => DeleteBlock());
                }
                nextBlock[i].transform.position = createPos[i].position;
            }
        }

        private void DeleteBlock()
        {
            nextBlock.RemoveAt(0);
            BlockInstance();
            SetBlockPosition();
        }

        private void Init()
        {
            for(int i = 0; i < createPos.Count; i++)
            {
                BlockInstance();
            }

            SetBlockPosition();
        }
    }
}