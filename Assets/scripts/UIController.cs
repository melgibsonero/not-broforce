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
        private Button restart;

        [SerializeField]
        private Button nextLevel;

        [SerializeField]
        private _sceneName scene;

        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private GameObject settings;

        protected bool paused = false;

        protected bool endScreenActivated = false;

        private string currentScene = "";

        private bool settingsOpened = false;

        public bool Paused {

            get { return paused; }
        }


        private enum _sceneName
        {
            HubLevel,
            NextLevel
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
            }

            return sceneName;
        }


        private void Awake()
        {
            Scene scene = SceneManager.GetActiveScene();
            currentScene = scene.name;
            DeactivateSettings();
            ShowPauseMenu(false);
            
        }


        public void NextLevel()
        {
            if(endScreenActivated && !paused)
            {
                GameManager.Instance.LoadScene(SceneName(scene));
            }
        }
       
        public void Restart ()
        {
            if(endScreenActivated || paused)
            {
                if(paused)
                {
                    ContinueTime();
                }
                GameManager.Instance.LoadScene(currentScene);
            }
        }
        
        public void ActivateButtons()
        {
            restart.gameObject.SetActive(true);
            nextLevel.gameObject.SetActive(true);
            endScreenActivated = true;
        }

        private void DeactivateButtons()
        {
            restart.gameObject.SetActive(false);
            nextLevel.gameObject.SetActive(false);
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

            } else
            {
                paused = false;
                ContinueTime();
                if(endScreenActivated)
                {
                    ActivateButtons();
                }
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

        public void BackToMainMenu ()
        {
            ContinueTime();
            GameManager.Instance.LoadScene("MainMenu");
        }

        public void ShowPauseMenu (bool activate)
        {
            pauseMenu.SetActive(activate);
        }

    }
}