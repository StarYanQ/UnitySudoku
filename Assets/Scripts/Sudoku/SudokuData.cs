using System;
using System.Linq;

namespace Sudoku
{
    public class SudokuData
    {
        public int[,] Puzzle { get; set; }
        public int[,] Solution { get; set; }
        /// <summary>
        /// 是否已经解出
        /// 缓存结果，避免重复计算
        /// 在调用CheckAnswer()之后更新
        /// </summary>
        public bool IsSolvedCached { get; private set; }
        
        public SudokuData()
        {
            Puzzle = new int[9, 9];
            Solution = new int[9, 9];
        }
        
        public SudokuData(int[,] puzzle)
        {
            Puzzle = puzzle;
            Solution = new int[9, 9];
            Array.Copy(puzzle, Solution, 81);
        }
        
        /// <summary>
        /// 解数独
        /// </summary>
        public void Solve()
        {
            var solver = new SudokuSolver(Puzzle);
            solver.Solve();
            Solution = solver.Answer;
        }

        /// <summary>
        /// 检查答案是否正确
        /// </summary>
        /// <returns></returns>
        public bool CheckAnswer()
        {
            var isCorrect = SudokuSolver.CheckAnswer(Solution);
            IsSolvedCached = isCorrect;
            return isCorrect;
        }
        
        /// <summary>
        /// 重置数独
        /// </summary>
        public void Reset()
        {
            Array.Copy(Puzzle, Solution, 81);
        }
        
        public static SudokuData GeneratePuzzle(int digHoleCount)
        {
            var generator = new SudokuGenerator();
            var puzzle = generator.Generate(digHoleCount);
            var data = new SudokuData(puzzle);
            return data;
        }

        /// <summary>
        /// 获取数独状态描述
        /// </summary>
        /// <returns></returns>
        public string GetStatueDesc()
        {
            var allEmptyCells = Puzzle.SelectAll().Count(x => x == 0);
            var emptyCells = Solution.SelectAll().Count(x => x == 0);
            if (emptyCells == 0)
            {
                var isCorrect = IsSolvedCached;
                var desc = isCorrect ? "正确" : "错误";
                return $"{allEmptyCells} / {allEmptyCells} {desc}";
            }
            
            return $"{allEmptyCells - emptyCells} / {allEmptyCells}";
        }

        public SudokuData Clone()
        {
            var data = new SudokuData();
            Array.Copy(Puzzle, data.Puzzle, 81);
            Array.Copy(Solution, data.Solution, 81);
            return data;
        }

        public bool GetRandomEmptyIndex(out int row, out int col)
        {
            var emptyCells = Solution.SelectAll()
                .Select((value,index) => (value,index))
                .Where(item => item.value == 0)
                .ToArray();
            if(emptyCells.Length == 0)
            {
                row = -1;
                col = -1;
                return false;
            }
            
            var index = UnityEngine.Random.Range(0, emptyCells.Length);
            var emptyCell = emptyCells[index].index;
            row = emptyCell / 9;
            col = emptyCell % 9;
            return true;
        }
    }
}