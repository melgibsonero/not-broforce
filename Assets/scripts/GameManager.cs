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

        private PlayerInput input;

        private MouseCursorController cursor;

        [SerializeField]
        private int currentLevel = 0;

        [SerializeField]
        private int latestCompletedLevel = 0;

        [SerializeField]
        public float musicVolume;

        [SerializeField]
        public float effectVolume;

        private FadeToColor fade;

        private string nextScene = "";

        private bool changingScene;

        private bool sceneLoaded;

        private bool alwaysShowBoxSelector;

        private bool holdToActivateBoxSelector;

        private bool playingUsingMouse;

        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicVolume = value;
                MusicPlayer.Instance.SetVolume(value);
            }
        }

        public float EffectVolume
        {
            get
            {
                return effectVolume;
            }
            set
            {
                effectVolume = value;
                SFXPlayer.Instance.SetVolume(value);
            }
        }

        public bool AlwaysShowBoxSelector
        {
            get
            {
                return alwaysShowBoxSelector;
            }
            set
            {
                alwaysShowBoxSelector = value;
                if(input != null)
                {
                    input.SetAlwaysShowBS(value);
                }
            }
        }

        public bool HoldToActivateBoxSelector
        {
            get
            {
                return holdToActivateBoxSelector;
            }
            set
            {
                holdToActivateBoxSelector = value;
                if(input != null)
                {
                    input.SetHoldToActivateBS(value);
                }
            }
        }

        public bool PlayingUsingMouse
        {
            get
            {
                return playingUsingMouse;
            }
            set
            {
                playingUsingMouse = value;
                cursor.PlayingUsingMouse = value;
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
            sceneLoaded = true;
            InitScene();

            LoadGame();

            DontDestroyOnLoad(gameObject);

            // Creates a new MusicPlayer instance
            // if one does not already exist
            MusicPlayer.Instance.Create();

            if (debug_UnlockAll)
            {
                LatestCompletedLevel = 8;
            }

            if (debug_ResetData)
            {
                Reset();
            }
        }

        private void InitInput()
        {
            if (input == null)
            {
                input = FindObjectOfType<PlayerInput>();
            }
        }

        private void ResetInput()
        {
            if (input != null)
            {
                input = null;
            }
        }

        public void Reset()
        {
            CurrentLevel = 0;

            // Overwrites the existing save!
            LatestCompletedLevel = 0;
        }

        public bool ClearFade
        {
            get
            {
                return (fade == null || fade.FadedIn);
            }
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
            //CurrentLevel = 0;
        }

        private void SaveGame()
        {
            // Note: Saved data can be found in
            // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

            PlayerPrefs.SetInt("latestCompletedLevel", latestCompletedLevel);
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            PlayerPrefs.SetFloat("effectVolume", effectVolume);

            // TODO: Put these to settings menu
            Utils.PlayerPrefsSetBool(
                "alwaysShowBoxSelector", alwaysShowBoxSelector);
            Utils.PlayerPrefsSetBool(
                "holdToActivateBoxSelector", holdToActivateBoxSelector);

            PlayerPrefs.Save();
            Debug.Log("--[ Game saved ]--");
            Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        }

        private void LoadGame()
        {
            latestCompletedLevel = PlayerPrefs.GetInt("latestCompletedLevel", 0);
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            effectVolume = PlayerPrefs.GetFloat("effectVolume", 0.5f);

            // TODO: Put these to settings menu
            alwaysShowBoxSelector =
                Utils.PlayerPrefsGetBool("alwaysShowBoxSelector", false);
            holdToActivateBoxSelector =
                Utils.PlayerPrefsGetBool("holdToActivateBoxSelector", false);

            Debug.Log("--[ Game loaded ]--");
            Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            PlayerPrefs.SetFloat("effectVolume", effectVolume);

            // TODO: Put these to settings menu
            Utils.PlayerPrefsSetBool(
                "alwaysShowBoxSelector", alwaysShowBoxSelector);
            Utils.PlayerPrefsSetBool(
                "holdToActivateBoxSelector", holdToActivateBoxSelector);

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
                StartSceneChange("Hub");
            }

            InitScene();

            if (changingScene &&
                (fade == null || fade.FadedOut))
            {
                LoadScene();
            }
        }

        public void StartSceneChange(string sceneName)
        {
            if (!changingScene)
            {
                nextScene = sceneName;
                changingScene = true;

                if (fade != null)
                {
                    fade.StartFadeOut();
                }

                Debug.Log("Next scene: " + sceneName);
            }
        }

        public void LoadScene()
        {
            LoadScene(nextScene);
        }

        public void LoadScene(string sceneName)
        {
            // Stops all SFX
            SFXPlayer.Instance.StopAllSFXPlayback();

            changingScene = false;
            sceneLoaded = true;
            SceneManager.LoadScene(sceneName);
        }

        private void InitScene()
        {
            if (sceneLoaded)
            {
                sceneLoaded = false;

                ResetInput();
                InitInput();

                ResetFade();
                InitFade();
            }
        }

        private void InitFade()
        {
            if (fade == null)
            {
                fade = FindObjectOfType<FadeToColor>();

                if (fade == null)
                {
                    Debug.LogError("Could not find a FadeToColor " +
                                   "object in the scene.");
                }
            }
        }

        private void ResetFade()
        {
            if (fade != null)
            {
                fade = null;
            }
        }
    }
}
