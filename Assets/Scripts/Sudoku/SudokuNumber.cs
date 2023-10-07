using TMPro;
using UnityEngine;

namespace Sudoku
{
    public class SudokuNumber : MonoBehaviour
    {
        public int row;
        public int col;
        public TMP_Text text;
        
        public void SetNumber(int num)
        {
            text.text = num.ToString();
        }
    }
}