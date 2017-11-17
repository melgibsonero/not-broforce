using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    // Note:
                    // There must be a Resources folder under Assets and
                    // GameManager there for this to work. Not necessary if
                    // a GameManager object is present in a scene from the
                    // get-go.

                    instance = 
                        Instantiate(Resources.Load<GameManager>("GameManager"));

                    //GameObject gmObj = Instantiate(Resources.Load("GameManager") as GameObject);
                    //instance = gmObj.GetComponent<GameManager>();
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
        private bool debug_ReturnToHub;

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

        public void SetLevel(int levelNum)
        {
            CurrentLevel = levelNum;
            //Debug.Log("Level set: " + levelNum);
        }

        public void LevelCompleted()
        {
            // TODO: Get the info about where the player goes
            // from endScreen and give it to GameManager
            CurrentLevel = 0;

            if (CurrentLevel == LatestUnlockedLevel)
            {
                LatestUnlockedLevel++;
            }
        }

        private void SaveGame()
        {
            // Note: Saved data can be found in
            // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

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

        private void Update()
        {
            // Testing purposes only
            if (debug_ReturnToHub)
            {
                debug_ReturnToHub = false;
                CurrentLevel = 0;
                SceneManager.LoadScene("Hub");
            }
        }
    }
}
