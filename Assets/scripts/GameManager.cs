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
        private int latestCompletedLevel = 0;

        [SerializeField]
        public float musicVolume;

        [SerializeField]
        public float effectVolume;

        public float EffectVolume
        {
            get
            {
                return effectVolume;
            }
            set
            {
                effectVolume = value;
                FindObjectOfType<SFXPlayer>().ChangeVolume(value);
            }
        }

        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                effectVolume = value;
                FindObjectOfType<MusicPlayer>().ChangeVolume(value);
            }
        }

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
                LatestCompletedLevel = 10;
            }

            if (debug_ResetData)
            {
                Reset();
            }
        }

        public void Reset()
        {
            CurrentLevel = 0;
            LatestCompletedLevel = 0;

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

                //if (currentLevel > LatestCompletedLevel)
                //{
                //    LatestCompletedLevel = currentLevel;
                //}
            }
        }

        public int LatestCompletedLevel
        {
            get
            {
                return latestCompletedLevel;
            }
            private set
            {
                if (value < 0)
                {
                    value = 0;
                }

                latestCompletedLevel = value;

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
            if (CurrentLevel > LatestCompletedLevel)
            {
                LatestCompletedLevel = currentLevel;
            }

            // TODO: Get the info about where the player goes
            // from endScreen and give it to GameManager
            CurrentLevel = 0;
        }

        private void SaveGame()
        {
            // Note: Saved data can be found in
            // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

            PlayerPrefs.SetInt("latestCompletedLevel", latestCompletedLevel);
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            PlayerPrefs.SetFloat("effectVolume", effectVolume);
            PlayerPrefs.Save();
            Debug.Log("--[ Game saved ]--");
            Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        }

        private void LoadGame()
        {
            latestCompletedLevel = PlayerPrefs.GetInt("latestCompletedLevel", 0);
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            effectVolume = PlayerPrefs.GetFloat("effectVolume", 0.5f);
            Debug.Log("--[ Game loaded ]--");
            Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        }

        public void SaveSettings()
        {

            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            PlayerPrefs.SetFloat("effectVolume", effectVolume);
            PlayerPrefs.Save();
            Debug.Log("Settings saved");
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
