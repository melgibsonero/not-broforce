﻿using System.Collections;
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
            Level2,
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
                case _sceneName.Level2:
                    {
                        sceneName = "Level2";
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
        }


        public void NextLevel()
        {
            if(endScreenActivated && !paused)
            {
                GameManager.Instance.SetLevel(
                    GameManager.Instance.CurrentLevel + 1);

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
        }

        private void DeactivateButtons()
        {
            endScreenButtons.SetActive(false);
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
    }
}
