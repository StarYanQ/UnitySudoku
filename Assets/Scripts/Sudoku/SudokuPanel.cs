using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Michsky.MUIP;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Sudoku
{
    public class SudokuPanel : MonoBehaviour
    {
        public SudokuBox box;
        public TMP_Text txtTitle;
        public GameObject btnSudokuTemplate;
        public Transform sudokuBtnContainer;
        public TMP_Text txtTimer;

        public List<SudokuData> sudokuList = new List<SudokuData>();
        public List<SudokuListButton> sudokuBtnList = new List<SudokuListButton>();

        public ButtonManager btnReset;
        public ButtonManager btnGetSolveTip;
        public ButtonManager btnSolveAll;
        public ButtonManager btnBack;

        public bool IsTimerRunning { get; set; }
        public DateTime Timer { get; set; }

        [HideInInspector]
        public int selectedIndex = -1;
        
        private MainScene mainScene;
        
        private void Awake()
        {
            btnSudokuTemplate.gameObject.SetActive(false);
            txtTitle.text = "";
            box.SudokuChanged += OnSudokuChanged;
            txtTimer.text = "00 : 00";
            
            
            btnReset.onClick.RemoveAllListeners();
            btnReset.onClick.AddListener(ResetCurrent);
            
            btnGetSolveTip.onClick.RemoveAllListeners();
            btnGetSolveTip.onClick.AddListener(GetSolveTip);
            
            btnSolveAll.onClick.RemoveAllListeners();
            btnSolveAll.onClick.AddListener(SolveAll);
            
            btnBack.onClick.RemoveAllListeners();
            btnBack.onClick.AddListener(BackToMainPanel);
        }

        private void Start()
        {
            mainScene = FindObjectOfType<MainScene>();
        }

        private void BackToMainPanel()
        {
            mainScene.SwitchToStartGamePanel();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            CheckTimer(dt);
        }
        
        private void RefreshAll()
        {
            foreach (var button in sudokuBtnList)
            {
                button.Refresh();
            }
            box.Refresh();
        }

        private void CheckTimer(float dt)
        {
            if (IsTimerRunning)
            {
                Timer = Timer.AddSeconds(dt);
                txtTimer.text = Timer.ToString("mm : ss");
            }
        }

        private void ResetCurrent()
        {
            if(selectedIndex == -1)
                return;
            
            var sudoku = sudokuList[selectedIndex];
            sudoku.Reset();
            RefreshAll();
        }

        private void GetSolveTip()
        {
            if(selectedIndex == -1)
                return;
            
            // 先生成一个完整答案
            var sudoku = sudokuList[selectedIndex];
            var answer = sudoku.Clone();
            answer.Solve();
            
            // 从原始数独中找到一个空位
            if(sudoku.GetRandomEmptyIndex(out var row, out var col))
            {
                // 将答案中的值复制过去
                sudoku.Solution[row, col] = answer.Solution[row, col];
                RefreshAll();
            }
        }
        
        private void SolveAll()
        {
            SolveAllAsync().Forget();
        }

        private async UniTask SolveAllAsync()
        {
            await UniTask.SwitchToThreadPool();

            var sw = Stopwatch.StartNew();
            var tasks = new List<UniTask>();
            foreach (var sudokuData in sudokuList)
            {
                tasks.Add(UniTask.RunOnThreadPool(() =>
                {
                    sudokuData.Solve();
                    sudokuData.CheckAnswer();
                }));
            }

            await UniTask.WhenAll();
            
            sw.Stop();
            Debug.Log($"求解全部数独耗时：{sw.ElapsedMilliseconds} ms");
            
            await UniTask.SwitchToMainThread();
            RefreshAll();
        }

        private void SelectSudoku(int index)
        {
            var sudoku = sudokuList[index];
            selectedIndex = index;
            box.BindSudoku(sudoku);
            txtTitle.text = $"数独 {index + 1}  完成度：【{sudoku.GetStatueDesc()}】";
        }
        
        private void OnSudokuChanged()
        {
            if (selectedIndex != -1)
            {
                var sudoku = sudokuList[selectedIndex];
                sudoku.CheckAnswer();
                txtTitle.text = $"数独 {selectedIndex + 1}  完成度：【{sudoku.GetStatueDesc()}】";
                var btn = sudokuBtnList[selectedIndex];
                btn.Refresh();
            }
            
            // 检查是否全部完成
            var allDone = true;
            foreach (var sudoku in sudokuList)
            {
                if (sudoku.IsSolvedCached == false)
                {
                    allDone = false;
                    break;
                }
            }
            
            if (allDone)
            {
                IsTimerRunning = false;
                var time = Timer.ToString("mm:ss");
                var title = "提示";
                var msg = $"恭喜你，你完成了所有的数独！\n用时：{time}\n是否返回主菜单？";
                var modalWindow = mainScene.modalWindow;
                modalWindow.onConfirm.RemoveAllListeners();
                modalWindow.onConfirm.AddListener(() =>
                {
                    modalWindow.CloseWindow();
                    BackToMainPanel();
                });
                modalWindow.onCancel.RemoveAllListeners();
                modalWindow.onCancel.AddListener(() =>
                {
                    modalWindow.CloseWindow();
                });
                modalWindow.titleText = title;
                modalWindow.descriptionText = msg;
                modalWindow.OpenWindow();
                modalWindow.UpdateUI();
            }
        }

        [Button]
        public async void GenerateSudokuParallax(int count, int digHoleCount = 30)
        {
            await UniTask.SwitchToThreadPool();
            
            IsTimerRunning = false;
            var sw = Stopwatch.StartNew();
            var dataArray = new SudokuData[count];
            var tasks = new List<UniTask>();
            for (int i = 0; i < count; i++)
            {
                // 在线程池中生成数独
                var index = i;
                tasks.Add(UniTask.RunOnThreadPool(() =>
                {
                    var sudoku = SudokuData.GeneratePuzzle(digHoleCount);
                    dataArray[index] = sudoku;
                }));
            }
            await UniTask.WhenAll(tasks);
            
            sw.Stop();
            Debug.Log($"生成{count}个数独耗时：{sw.ElapsedMilliseconds}ms");
            
            await UniTask.SwitchToMainThread();
            // 将生成的数独加入游戏
            foreach (var sudoku in dataArray)
            {
                sudokuList.Add(sudoku);
                var btn = Instantiate(btnSudokuTemplate, sudokuBtnContainer).GetComponent<SudokuListButton>();
                sudokuBtnList.Add(btn);
                btn.gameObject.SetActive(true);
                btn.bindSudoku = sudoku;
                btn.index = sudokuList.Count - 1;
                btn.Refresh();
                btn.OnSelectSudoku += SelectSudoku;
                    
                if(selectedIndex == -1)
                    SelectSudoku(0);
            }
            
            IsTimerRunning = true;
        }

        [Button]
        private void TestSolveAll()
        {
            foreach (var sudokuData in sudokuList)
            {
                sudokuData.Solve();
            }
            RefreshAll();
        }

        public void StartGame(int difficulty)
        {
            var digHoleCount = 30;
            switch (difficulty)
            {
                case 0:
                    digHoleCount = 20;
                    break;
                case 1:
                    digHoleCount = 30;
                    break;
                case 2:
                    digHoleCount = 40;
                    break;
            }
            
            ClearAll();
            GenerateSudokuParallax(9, digHoleCount);
        }

        private void ClearAll()
        {
            foreach (var sudokuBtn in sudokuBtnList)
            {
                Destroy(sudokuBtn.gameObject);
            }
            sudokuBtnList.Clear();
            sudokuList.Clear();
            selectedIndex = -1;
            txtTitle.text = "数独";
            Timer = DateTime.MinValue;
            IsTimerRunning = false;
        }
    }
}