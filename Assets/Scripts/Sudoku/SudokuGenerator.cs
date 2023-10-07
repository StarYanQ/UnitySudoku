using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Sudoku
{
    public class SudokuGenerator
    {
        private int[,] _board = new int[9, 9];
        private int[,] _puzzle = new int[9, 9];
        
        public int[,] Board => _board;
        public int[,] Puzzle => _puzzle;

        public int[,] Generate(int holeCount = 40)
        {
            GenerateRawSudoku();
            Array.Copy(_board, _puzzle, 81);
            DigHoles(holeCount);
            return Puzzle;
        }
        
        /// <summary>
        /// 生成原始数独
        /// </summary>
        /// <returns></returns>
        private bool GenerateRawSudoku()
        {
            // 创建一个随机排列的数字列表
            List<int> numbers = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToList();

            if (!FindEmptyCell(out var row, out var col))
            {
                return true; // 所有单元格都已经填满
            }

            for (int i = 0; i < 9; i++)
            {
                int num = numbers[i];
                if (IsSafe(row, col, num))
                {
                    _board[row, col] = num;

                    if (GenerateRawSudoku())
                    {
                        return true;
                    }

                    _board[row, col] = 0; // 回溯
                }
            }

            return false; // 无法找到合适的数字填充，需要回溯
        }

        private void DigHoles(int emptyCells)
        {
            Random random = new Random();
            var emptyCellsInBox = emptyCells / 9;
            
            // 为多余的空格找到一个地方
            var lastEmptyCellsInBox = emptyCells % 9;
            var lastEmptyCells = new int[9];
            for (int i = 0; i < lastEmptyCellsInBox; i++)
            {
                lastEmptyCells[i] = 1;
            }
            lastEmptyCells.Shuffle(random);

            // 把数独划分成3 x 3的9个小方块，每个小方块分别挖洞
            var index = 0;
            for (int row = 0; row < 9; row += 3)
            {
                for (int col = 0; col < 9; col += 3)
                {
                    var digCount = Mathf.Clamp(emptyCellsInBox + lastEmptyCells[index++], 1, 8);
                    DigHolesInBox(row, col, digCount, random);
                }
            }
        }

        private void DigHolesInBox(int row, int col, int emptyCells, Random random)
        {
            int[] holes = new int[9];
            for (int i = 0; i < 9; i++)
            {
                holes[i] = i;
            }

            // 洗牌算法
            for (int i = 0; i < 9; i++)
            {
                int j = random.Next(9);
                (holes[i], holes[j]) = (holes[j], holes[i]);
            }

            for (int i = 0; i < 9; i++)
            {
                int hole = holes[i];
                int rowInBox = hole / 3;
                int colInBox = hole % 3;
                int rowInBoard = row + rowInBox;
                int colInBoard = col + colInBox;
                if (emptyCells > 0 && _puzzle[rowInBoard, colInBoard] != 0)
                {
                    _puzzle[rowInBoard, colInBoard] = 0;
                    emptyCells--;
                }
            }
        }

        private bool FindEmptyCell(out int row, out int col)
        {
            for (row = 0; row < 9; row++)
            {
                for (col = 0; col < 9; col++)
                {
                    if (_board[row, col] == 0)
                    {
                        return true;
                    }
                }
            }

            row = col = -1;
            return false;
        }

        private bool IsSafe(int row, int col, int num)
        {
            return !UsedInRow(row, num) &&
                   !UsedInCol(col, num) &&
                   !UsedInBox(row - row % 3, col - col % 3, num);
        }

        private bool UsedInRow(int row, int num)
        {
            for (int col = 0; col < 9; col++)
            {
                if (_board[row, col] == num)
                {
                    return true;
                }
            }

            return false;
        }

        private bool UsedInCol(int col, int num)
        {
            for (int row = 0; row < 9; row++)
            {
                if (_board[row, col] == num)
                {
                    return true;
                }
            }

            return false;
        }

        private bool UsedInBox(int startRow, int startCol, int num)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (_board[row + startRow, col + startCol] == num)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}