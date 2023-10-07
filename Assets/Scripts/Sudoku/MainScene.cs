using System;
using Michsky.MUIP;
using UnityEngine;

namespace Sudoku
{
    public class MainScene : MonoBehaviour
    {
        public SudokuPanel sudokuPanel;
        public StartGamePanel startGamePanel;
        public ModalWindowManager modalWindow;

        private void Awake()
        {
            modalWindow.CloseWindow();
        }

        public void SwitchToStartGamePanel()
        {
            sudokuPanel.gameObject.SetActive(false);
            startGamePanel.gameObject.SetActive(true);
        }
        
        public void SwitchToSudokuPanel()
        {
            sudokuPanel.gameObject.SetActive(true);
            startGamePanel.gameObject.SetActive(false);
        }
    }
}