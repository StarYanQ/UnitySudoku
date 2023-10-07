using System;
using System.Linq;

namespace Sudoku
{
    public class SudokuSolver
    {
        private int[,] answer = new int[9, 9];
        private int[,] problem = null;
        
        public int[,] Answer => answer;
        public int[,] Problem => problem;
        
        public SudokuSolver(int[,] problem)
        {
            this.problem = problem;
        }
        
        public bool Solve()
        {
            Array.Copy(problem, answer, 81);
            return SolveSudokuInner();
        }

        /// <summary>
        /// 求解数独
        /// </summary>
        /// <returns></returns>
        private bool SolveSudokuInner()
        {
            if (!FindEmptyCell(out int row, out int col))
            {
                return true; // 所有单元格都已经填满
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num))
                {
                    answer[row, col] = num;

                    if (SolveSudokuInner())
                    {
                        return true;
                    }

                    answer[row, col] = 0; // 回溯
                }
            }

            return false; // 无法找到合适的数字填充，需要回溯
        }

        private bool FindEmptyCell(out int row, out int col)
        {
            for (row = 0; row < 9; row++)
            {
                for (col = 0; col < 9; col++)
                {
                    if (answer[row, col] == 0)
                    {
                        return true;
                    }
                }
            }

            row = -1;
            col = -1;
            return false;
        }
        
        private bool IsSafe(int row, int col, int num)
        {
            return !UsedInRow(row, num) && !UsedInCol(col, num) && !UsedInBox(row - row % 3, col - col % 3, num);
        }

        private bool UsedInRow(int row, int num)
        {
            for (int col = 0; col < 9; col++)
            {
                if (answer[row, col] == num)
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
                if (answer[row, col] == num)
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
                    if (answer[row + startRow, col + startCol] == num)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// 检查给定的数独是否是合法的数独
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static bool CheckAnswer(int[,] answer)
        {
            // 检查行、列、小方块是否符合数独规则
            for (int row = 0; row < 9; row++)
            {
                if (!CheckRow(answer, row))
                {
                    return false;
                }
            }

            for (int col = 0; col < 9; col++)
            {
                if (!CheckCol(answer, col))
                {
                    return false;
                }
            }

            for (int row = 0; row < 9; row += 3)
            {
                for (int col = 0; col < 9; col += 3)
                {
                    if (!CheckBox(answer, row, col))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 检查第row行是否符合数独规则
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool CheckRow(int[,] answer, int row)
        {
            Span<bool> used = stackalloc bool[10];
            
            for (int col = 0; col < 9; col++)
            {
                int num = answer[row, col];
                if (num != 0)
                {
                    if (used[num])
                    {
                        return false;
                    }

                    used[num] = true;
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查第col列是否符合数独规则
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool CheckCol(int[,] answer, int col)
        {
            Span<bool> used = stackalloc bool[10];
            
            for (int row = 0; row < 9; row++)
            {
                int num = answer[row, col];
                if (num != 0)
                {
                    if (used[num])
                    {
                        return false;
                    }

                    used[num] = true;
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查第row行第col列开始的小方块是否符合数独规则
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool CheckBox(int[,] answer, int row, int col)
        {
            Span<bool> used = stackalloc bool[10];
            
            for (int r = row; r < row + 3; r++)
            {
                for (int c = col; c < col + 3; c++)
                {
                    int num = answer[r, c];
                    if (num != 0)
                    {
                        if (used[num])
                        {
                            return false;
                        }

                        used[num] = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}