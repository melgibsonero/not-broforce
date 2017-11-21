using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace not_broforce
{
    public class LevelChanger: MonoBehaviour
    {
        [SerializeField]
        private Button restart;

        [SerializeField]
        private Button nextLevel;

        [SerializeField]
        private _sceneName scene;

        [SerializeField]
        private GameObject pauseMenu;

        protected bool paused = false;

        protected bool endScreenActivated = false;

        private string currentScene = "";

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
            ShowPauseMenu(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(!paused)
                {
                    Pause(true);
                    paused = true;
                } else if (paused)
                {
                    Pause(false);
                    paused = false;
                }
            }
            if(endScreenActivated)
            {
                if(Input.GetKey(KeyCode.Space))
                {
                    nextLevel.onClick.Invoke();
                } else if(Input.GetKey(KeyCode.R))
                {
                    restart.onClick.Invoke();
                }
            }
        }

        public void NextLevel()
        {
            SceneManager.LoadScene(SceneName(scene));
        }
       
        public void Restart ()
        {
            if(paused)
            {
                Pause(false);
                paused = false;
            }
            SceneManager.LoadScene(currentScene);
        }
        
        public void ActivateButtons()
        {
            restart.gameObject.SetActive(true);
            nextLevel.gameObject.SetActive(true);
            endScreenActivated = true;
        }

        private void Pause (bool pause)
        {
            if(pause)
            {
                Time.timeScale = 0f;
            } else
            {
                Time.timeScale = 1f;
            }
            ShowPauseMenu(pause);
        }

        public void ResumeGame()
        {
            Pause(false);
            paused = false;
        }

        public void ActivateSettings ()
        {
            //TODO open settings and disable pauseMenu
        }

        public void BackToMainMenu ()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ShowPauseMenu (bool activate)
        {
            pauseMenu.SetActive(activate);
        }

    }
}
