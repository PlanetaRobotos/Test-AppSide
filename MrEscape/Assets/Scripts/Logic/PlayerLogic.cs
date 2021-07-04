using System;
using System.Collections;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic
{
    /// <summary>
    /// Main Logic class
    /// </summary>
    public class PlayerLogic : MonoBehaviour
    {
        public static event Action OnFirstGame;

        public static int CurrentScore { get; set; }
        public static int MaxScore { get; private set; }
        private static int CurrentLevel { get; set; }

        public static int CurrentMaxTime { get; private set; }

        public static bool CanMove { get; set; }

        [Header("Logic Stuff")]
        [SerializeField] private int[] scores;
        [SerializeField] private int[] maxTimes;
        [SerializeField] private Transform[] playerStartPositions;
        [SerializeField] private int countOfLevels;

        private Transform _playerTransform;

        private void Awake()
        {
            CurrentLevel = 0;
            CheckFirstGame();

            LogicSettings();

            CurrentLevel = PlayerPrefs.GetInt(Prefs.CurrentLevel);

            CurrentScore = 0;
            MaxScore = scores[CurrentLevel];
            CurrentMaxTime = maxTimes[CurrentLevel];

            PlayerPrefs.SetInt(Prefs.CompleteLevel, 0);
            PlayerPrefs.SetInt(Prefs.PreviousLevel, CurrentLevel);

            _playerTransform.position = playerStartPositions[CurrentLevel].position;
        }

        /// <summary>
        /// Level, score and ui-level settings
        /// </summary>
        private void LogicSettings()
        {
            _playerTransform = GameObject.FindWithTag(Tags.Player).transform;
            CanMove = true;
            CurrentLevel = PlayerPrefs.GetInt(Prefs.CurrentLevel);

            int uiLevel = PlayerPrefs.GetInt(Prefs.UILevel);
            if (uiLevel == 0)
                PlayerPrefs.SetInt(Prefs.UILevel, 1);

            bool isComplete = PlayerPrefs.GetInt(Prefs.CompleteLevel) == 1;
            if (isComplete)
            {
                int prevLevel = PlayerPrefs.GetInt(Prefs.PreviousLevel);
                int randLevels = PlayerPrefs.GetInt(Prefs.RandomLevels);
                if (CurrentLevel < countOfLevels - 1 && randLevels == 0)
                {
                    PlayerPrefs.SetInt(Prefs.CurrentLevel, CurrentLevel + 1);
                }
                else
                {
                    if (randLevels == 0)
                        PlayerPrefs.SetInt(Prefs.RandomLevels, 1);

                    int curLevel = Random.Range(0, countOfLevels);

                    while (true)
                    {
                        if (curLevel == prevLevel)
                            curLevel = Random.Range(0, countOfLevels);
                        else
                            break;
                    }

                    PlayerPrefs.SetInt(Prefs.CurrentLevel, curLevel);
                }

                PlayerPrefs.SetInt(Prefs.UILevel, uiLevel + 1);
            }
        }

        private static void CheckFirstGame()
        {
            bool isFirstGame = PlayerPrefs.GetInt(Prefs.FirstGame) == 0;
            if (isFirstGame)
            {
                OnFirstGame?.Invoke();
                PlayerPrefs.SetInt(Prefs.UseTimer, 1);
                PlayerPrefs.SetInt(Prefs.FirstGame, 1);
            }
        }
    }
}