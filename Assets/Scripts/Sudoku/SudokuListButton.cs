using System;
using Michsky.MUIP;
using UnityEngine;

namespace Sudoku
{
    public class SudokuListButton : MonoBehaviour
    {
        public ButtonManager buttonManager;
        public int index;
        public SudokuData bindSudoku;
        public event Action<int> OnSelectSudoku;

        private void Awake()
        {
            buttonManager.onClick.RemoveAllListeners();
            buttonManager.onClick.AddListener(OnClickInner);
        }

        private void OnClickInner()
        {
            OnSelectSudoku?.Invoke(index);
        }

        public void Refresh()
        {
            buttonManager.SetText($"数独 {index + 1}  完成度：【{bindSudoku.GetStatueDesc()}】");
        }
    }
}