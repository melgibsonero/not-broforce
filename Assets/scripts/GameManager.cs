using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class GameManager : MonoBehaviour
    {
        #region Statics
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = 
                        Instantiate(Resources.Load<GameManager>("GameManager"));
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField]
        private bool debug_UnlockAll;

        [SerializeField]
        private bool debug_ResetData;

        [SerializeField]
        private int currentLevel = 0;

        [SerializeField]
        private int latestUnlockedLevel = 0;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        private void Init()
        {
            LoadGame();
            DontDestroyOnLoad(gameObject);

            if (debug_UnlockAll)
            {
                // TODO: Decide how many levels are there in total.
                LatestUnlockedLevel = 10;
            }

            if (debug_ResetData)
            {
                Reset();
            }
        }

        public void Reset()
        {
            CurrentLevel = 0;
            LatestUnlockedLevel = 0;

            // Overwrites the existing save!
            SaveGame();
        }

        public int CurrentLevel
        {
            get { return currentLevel; }
            private set
            {
                if (value < 0)
                {
                    value = 0;
                }

                currentLevel = value;

                if (currentLevel > LatestUnlockedLevel)
                {
                    LatestUnlockedLevel = currentLevel;
                }
            }
        }

        public int LatestUnlockedLevel
        {
            get
            {
                return latestUnlockedLevel;
            }
            private set
            {
                if (value < 0)
                {
                    value = 0;
                }

                latestUnlockedLevel = value;

                SaveGame();
            }
        }

        public void EnterLevel(int levelNum)
        {
            CurrentLevel = levelNum;
            Debug.Log("Level entered: " + levelNum);
        }

        public void LevelCompleted()
        {
            if (CurrentLevel == LatestUnlockedLevel)
            {
                LatestUnlockedLevel++;
            }
        }

        private void SaveGame()
        {
            PlayerPrefs.SetInt("latestUnlockedLevel", latestUnlockedLevel);
            PlayerPrefs.Save();

            Debug.Log("--[ Game saved ]--");
            Debug.Log("latestUnlockedLevel: " + latestUnlockedLevel);
        }

        private void LoadGame()
        {
            latestUnlockedLevel = PlayerPrefs.GetInt("latestUnlockedLevel", 0);

            Debug.Log("--[ Game loaded ]--");
            Debug.Log("latestUnlockedLevel: " + latestUnlockedLevel);
        }
    }
}
