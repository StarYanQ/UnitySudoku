using System;
using TMPro;
using UnityEngine;

namespace Sudoku
{
    public delegate void SudokuInputCallback(int row, int col, int num);
    
    public class SudokuNumInput : MonoBehaviour
    {
        public TMP_InputField inputField;
        public int row;
        public int col;
        
        public SudokuInputCallback callback;

        private void Awake()
        {
            inputField.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnEndEdit(string arg0)
        {
            if (int.TryParse(arg0, out var num) && num >= 1 && num <= 9)
            {
                callback?.Invoke(row, col, num);
            }
            else
            {
                inputField.text = "";
            }
        }

        public void SetNumber(int i)
        {
            if (i == 0)
            {
                Clear();
                return;
            }
            inputField.text = i.ToString();
        }

        public void Clear()
        {
            inputField.text = "";
        }
    }
}
