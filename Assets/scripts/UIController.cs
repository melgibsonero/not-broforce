using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace not_broforce
{
    public class UIController: MonoBehaviour
    {

        [SerializeField]
        private GameObject endScreenButtons;

        [SerializeField]
        private _sceneName scene;

        [SerializeField]
        private int levelNum;

        [SerializeField]
        private bool finalLevel;

        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private GameObject settings;

        [SerializeField]
        private GameObject backgroundImage;

        protected bool paused = false;

        protected bool endScreenActivated = false;

        private string currentScene = "";

        private bool settingsOpened = false;

        public bool Paused {

            get { return paused; }
        }

        public bool ChangingScene { get; private set; }

        private enum _sceneName
        {
            HubLevel,
            NextLevel,
            Level,
            MainMenu
        }

        private string SceneName( _sceneName scene )
        {

            string sceneName = "";

            switch(scene)
            {
                case _sceneName.HubLevel:
                    {
                        sceneName = "Hub";
                        break;
                    }
                case _sceneName.NextLevel:
                    {
                        sceneName = "Valtterin Playground";
                        break;
                    }
                case _sceneName.Level:
                    {
                        sceneName = "Level" + levelNum;
                        break;
                    }
                case _sceneName.MainMenu:
                    {
                        sceneName = "MainMenu";
                        break;
                    }
            }

            return sceneName;
        }


        private void Awake()
        {
            Scene scene = SceneManager.GetActiveScene();
            currentScene = scene.name;
            DeactivateSettings();
            ShowPauseMenu(false);
            ShowBackground(false);
            endScreenButtons.SetActive(false);

            if (finalLevel)
            {
                Button nextLevelButton =
                    endScreenButtons.GetComponentInChildren<Button>(true);

                nextLevelButton.interactable = false;
            }
        }


        public void NextLevel()
        {
            if (endScreenActivated && !paused)
            {
                if (scene == _sceneName.Level)
                {
                    // Changes the current level number to the next one
                    if (levelNum > 0)
                    {
                        GameManager.Instance.SetLevel(levelNum);
                    }
                    // If the next level button is clicked and the level number
                    // is invalid, the next scene will be the main menu
                    else
                    {
                        scene = _sceneName.MainMenu;
                    }
                }

                // Starts the scene change
                StartSceneChange(SceneName(scene));
            }
        }
       
        public void Restart ()
        {
            if(endScreenActivated || paused)
            {
                //if(paused)
                //{
                //    ContinueTime();
                //}
                StartSceneChange(currentScene);
            }
        }
        
        public void ActivateButtons()
        {
            endScreenButtons.SetActive(true);
            endScreenActivated = true;

            backgroundImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(460, 500);
            ShowBackground(true);
        }

        private void DeactivateButtons()
        {
            endScreenButtons.SetActive(false);
            ShowBackground(false);
        }

        public bool ToggleMenus ()
        {
            if(!settingsOpened)
            {
                TogglePause();
            }
            else if(settingsOpened)
            {
                DeactivateSettings();
            }
            return paused;
        }

        public void TogglePause ()
        {
            if(!paused)
            {
                paused = true;
                StopTime();
                if(endScreenActivated)
                {
                    DeactivateButtons();
                }

                ShowBackground(true);

            } else
            {
                paused = false;
                ContinueTime();
                if(endScreenActivated)
                {
                    ActivateButtons();
                }

                ShowBackground(false);
            }
            ShowPauseMenu(paused);
        }

        private void ContinueTime()
        {
            Time.timeScale = 1f;
            GameManager.Instance.MenuExited = true;
        }

        private void StopTime()
        {
            Time.timeScale = 0f;
        }

        public void ActivateSettings ()
        {
            settingsOpened = true;
            settings.SetActive(true);
            ShowPauseMenu(false);

            backgroundImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(280, 300);
        }

        public void DeactivateSettings ()
        {
            GameManager.Instance.SaveSettings();
            settingsOpened = false;
            settings.SetActive(false);
            if(paused)
            {
                ShowPauseMenu(true);
            }
        }

        public void OnCancelInputDown()
        {
            if (settingsOpened)
            {
                DeactivateSettings();
            }
            else if (paused)
            {
                TogglePause();
            }
        }

        public void BackToMainMenu ()
        {
            GameManager.Instance.SetLevel(0);
            StartSceneChange("MainMenu");
        }

        public void ShowPauseMenu (bool activate)
        {
            pauseMenu.SetActive(activate);

            backgroundImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(460, 450);
        }

        public void ShowBackground(bool activate)
        {
            backgroundImage.SetActive(activate);
        }

        private void DeactivateAll()
        {
            if (paused)
            {
                paused = false;
                ContinueTime();
            }

            DeactivateButtons();
            DeactivateSettings();
            ShowPauseMenu(false);
            ShowBackground(false);
        }

        private void StartSceneChange(string sceneName)
        {
            DeactivateAll();
            ChangingScene = true;
            GameManager.Instance.StartSceneChange(sceneName);
        }

        public int CurrentLevel()
        {
            return levelNum - 1;
        }

        public bool CurrentLevelIsFinal()
        {
            return finalLevel;
        }
    }
}
