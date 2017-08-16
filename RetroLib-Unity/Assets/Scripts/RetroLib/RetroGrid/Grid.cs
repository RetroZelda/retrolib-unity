using UnityEngine;
using System.Collections;
using System;

namespace Retro.Grid
{
    public abstract class Grid : MonoBehaviour {

        private Plane _plane;

        private Block[,] Blocks;

        public float BlockSize = 1.0f;
        
        public int NumberOfRows = 10;
        public int NumberOfColums = 10;

    #if UNITY_EDITOR
        [SerializeField]
        public Color _DebugDrawColor = Color.yellow;

        [SerializeField]
        public bool _DebugLockGridView = false;
    #endif

        protected abstract Block InstantiateBlock();
        

        public Plane GetPlane()
        {
            // TODO: Cache this
            _plane = new Plane(transform.up, transform.position);
            return _plane;
        }

        public Block GetBlock(int row, int col)
        {
            if(row < NumberOfRows && row > -1 && col < NumberOfColums && col > -1)
                return Blocks[row,col];
            else
                return null;
        }

        public void BuildGrid(int rowtotal, int coltotal)
        {
            NumberOfRows = rowtotal;
            NumberOfColums = coltotal;
            Blocks = new Block[rowtotal, coltotal];

            int nCurColorIndex = 0;
            int nStartColorIndex = 0;
            for (int row = 0; row < rowtotal; ++row)
            {
                nCurColorIndex = nStartColorIndex++;
                for (int col = 0; col < coltotal; ++col)
                {
                    Block block = CreateBlock(row, col);
                }
            }
            Refresh();
        }
        
        private Block CreateBlock(int row, int col)
        {
            Vector3 TempPosition = new Vector3(-BlockSize * NumberOfRows, 0, BlockSize * NumberOfColums);
            Vector3 TempScale = new Vector3(0,0,0);

            GameObject OpenBlock = InstantiateBlock().gameObject;
            OpenBlock.SetActive(true);

            TempPosition *= -0.5f;
            TempPosition.x -= BlockSize * row + BlockSize / 2;
            TempPosition.z += BlockSize * col + BlockSize / 2;

            TempScale.x = TempScale.y = TempScale.z = BlockSize;

            OpenBlock.transform.SetParent(transform);
            OpenBlock.transform.position = TempPosition;
            OpenBlock.transform.localScale = TempScale;


            Blocks[row,col] = OpenBlock.GetComponent<Block>();
            Blocks[row,col].SetUp(this, row, col);
            return Blocks[row, col];
        }
        
        public Block FindNearestBlock(Vector3 _Position)
        {
            float closest = Vector3.Distance(_Position, Blocks[0,0].transform.position);
            int tRow = 0;
            int tCol = 0;
            float temp;

            for(int row = 0; row < NumberOfRows; ++row)
                for(int col = 0; col < NumberOfColums; ++col)
                {
                    temp = Vector3.Distance(_Position, Blocks[row, col].transform.position);
                    if(temp < closest)
                    {
                        closest = temp;
                        tRow = row;
                        tCol = col;
                    }
                }

            return Blocks[tRow,tCol];
        }

        // TODO: This function
        public Block FindNearestBlock_MAKEFASTER(Vector3 _Position)
        {
            Vector3 v3RelativePos = _Position - transform.position;
            int nRow = Mathf.Clamp((int)v3RelativePos.x, 0, NumberOfRows);
            int nCol = Mathf.Clamp((int)v3RelativePos.y, 0, NumberOfColums);

            return Blocks[nRow, nCol];
        }

        public void Refresh()
        {
            for (int row = 0; row < NumberOfRows; ++row)
            {
                for (int col = 0; col < NumberOfColums; ++col)
                {
                    Blocks[row, col].Refresh();
                }
            }
        }

    #if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (!_DebugLockGridView)
                return;

            Grid grid = this; // this is here in case we need to move this functionality back into the editor script

            float fBlock = grid.BlockSize;
            float fRowOffset = (fBlock * grid.NumberOfRows) * 0.5f;
            float fColOffset = (fBlock * grid.NumberOfColums) * 0.5f;
            Vector3 v3Center = grid.transform.position;
            Vector3 v3Start = v3Center;
            Vector3 v3End = v3Center;

            Gizmos.color = grid._DebugDrawColor;

            // draw each row
            for (int nRow = 0; nRow < grid.NumberOfRows + 1; ++nRow)
            {
                v3Start = v3Center;
                v3End = v3Center;

                v3Start.x -= fRowOffset - (fBlock * nRow);
                v3Start.z -= fColOffset;

                v3End.x -= fRowOffset - (fBlock * nRow);
                v3End.z += fColOffset;

                Gizmos.DrawLine(v3Start, v3End);
            }

            // draw each col
            for (int nCol = 0; nCol < grid.NumberOfColums + 1; ++nCol)
            {
                v3Start = v3Center;
                v3End = v3Center;

                v3Start.x -= fRowOffset;
                v3Start.z -= fColOffset - (fBlock * nCol);

                v3End.x += fRowOffset;
                v3End.z -= fColOffset - (fBlock * nCol);

                Gizmos.DrawLine(v3Start, v3End);
            }
        }
    #endif
    }
}