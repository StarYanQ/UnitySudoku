using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sudoku
{
    public class SudokuBox : MonoBehaviour
    {
        public Transform container;
        public GameObject inputTemplate;
        public GameObject numberTemplate;
        
        public float cellEdgeOffsetH = 315;
        public float cellEdgeOffsetV = 315;
        
        public event Action SudokuChanged;
        
        private List<SudokuNumInput> inputs = new List<SudokuNumInput>();
        private List<SudokuNumber> numbers = new List<SudokuNumber>();

        public SudokuData Data { get; set; }

        private void Awake()
        {
            inputTemplate.gameObject.SetActive(false);
            numberTemplate.gameObject.SetActive(false);
        }

        public void BindSudoku(int[,] problem)
        {
            BindSudoku(new SudokuData(problem));
        }
        
        public void BindSudoku(SudokuData sudoku)
        {
            Data = sudoku;
            foreach (var input in inputs)
            {
                Destroy(input.gameObject);
            }
            inputs.Clear();
            foreach (var number in numbers)
            {
                Destroy(number.gameObject);
            }
            numbers.Clear();
            
            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    var num = Data.Puzzle[row, col];
                    var offsetH = cellEdgeOffsetH / 4 * (col - 4);
                    var offsetV = cellEdgeOffsetV / 4 * (row - 4);
                    if (num == 0)
                    {
                        var input = Instantiate(inputTemplate, container).GetComponent<SudokuNumInput>();
                        input.gameObject.SetActive(true);
                        input.row = row;
                        input.col = col;
                        input.callback = OnInputCallback;
                        input.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetH, offsetV);
                        input.Clear();
                        inputs.Add(input);
                    }
                    else
                    {
                        var number = Instantiate(numberTemplate, container).GetComponent<SudokuNumber>();
                        number.row = row;
                        number.col = col;
                        number.gameObject.SetActive(true);
                        number.SetNumber(num);
                        number.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetH, offsetV);
                        numbers.Add(number);
                    }
                }
            }
            Refresh(false);
        }

        private void OnInputCallback(int row, int col, int num)
        {
            if(Data == null)
                return;
            
            Data.Solution[row, col] = num;
            
            OnOnSudokuChanged();
        }
        
        private void Refresh(int[,] solve)
        {
            Data.Solution = solve;
            Refresh();
        }

        public void Refresh(bool callback = true)
        {
            foreach (var input in inputs)
            {
                input.SetNumber(Data.Solution[input.row, input.col]);
            }
            
            foreach (var number in numbers)
            {
                number.SetNumber(Data.Solution[number.row, number.col]);
            }
            
            if(callback) 
                OnOnSudokuChanged();
        }

        [Button]
        private void TestGenerate()
        {
            var sudokuGenerator = new SudokuGenerator();
            var problem = sudokuGenerator.Generate();
            BindSudoku(problem);
        }

        [Button]
        private void TestSolve()
        {
            var sudokuSolver = new SudokuSolver(Data.Puzzle);
            if (sudokuSolver.Solve())
            {
                Refresh(sudokuSolver.Answer);
            }
            else
            {
                Debug.LogError("无解");
            }
        }
        
        [Button]
        private void TestReset()
        {
            if(Data == null)
                return;
            
            Data.Reset();
            Refresh();
        }

        protected virtual void OnOnSudokuChanged()
        {
            SudokuChanged?.Invoke();
        }
    }
}