using System;
using Michsky.MUIP;
using UnityEngine;

namespace Sudoku
{
    public class StartGamePanel : MonoBehaviour
    {
        public ButtonManager btnEasy;
        public ButtonManager btnNormal;
        public ButtonManager btnHard;
        public ButtonManager btnExit;

        private MainScene mainScene;
        
        private void Awake()
        {
            btnEasy.onClick.RemoveAllListeners();
            btnEasy.onClick.AddListener(OnEasyClick);
            
            btnNormal.onClick.RemoveAllListeners();
            btnNormal.onClick.AddListener(OnNormalClick);
            
            btnHard.onClick.RemoveAllListeners();
            btnHard.onClick.AddListener(OnHardClick);
            
            btnExit.onClick.RemoveAllListeners();
            btnExit.onClick.AddListener(OnExitClick);
        }

        private void Start()
        {
            mainScene = FindObjectOfType<MainScene>();
        }

        private void OnEasyClick()
        {
            mainScene.SwitchToSudokuPanel();
            mainScene.sudokuPanel.StartGame(0);
        }

        private void OnNormalClick()
        {
            mainScene.SwitchToSudokuPanel();
            mainScene.sudokuPanel.StartGame(1);
        }

        private void OnHardClick()
        {
            mainScene.SwitchToSudokuPanel();
            mainScene.sudokuPanel.StartGame(2);
        }

        private void OnExitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}