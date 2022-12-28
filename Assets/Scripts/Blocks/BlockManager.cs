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
        [SerializeField, Header("���Ƃ����Ԃ̊��o")]
        private float downInterval = 1f;

        [SerializeField, Header("���������ɗ����鑁��")]
        private float holedInterval = 0.5f;

        [SerializeField, Header("��������ꏊ")]
        private List<Transform> createPos;

        [SerializeField, Header("�u���b�N�̐e�I�u�W�F�N�g")]
        private Transform grid;

        [SerializeField, Header("��������u���b�N�̃��X�g(Prefab���X�g)")]
        private List<GameObject> blocks;

        //�\������u���b�N�̎󂯎M
        private List<GameObject> nextBlock = new(4);

        [SerializeField,Header("�Q�ƃX�N���v�g")]
        private InputController controller;

        private BlockController blockdata;

        void Start()
        {
            Init();
        }

        //Block�̐��������̊֐�
        private void BlockInstance()
        {
            GameObject block = blocks[UnityEngine.Random.Range(0, blocks.Count)];
            GameObject obj = Instantiate(block);

            nextBlock.Add(obj);
        }

        //Block�̕��ёւ�
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