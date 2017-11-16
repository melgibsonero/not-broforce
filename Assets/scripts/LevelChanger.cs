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

        protected bool takeInput = false;

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
        }

        private void Update()
        {
            if(takeInput)
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
            SceneManager.LoadScene(currentScene);
        }
        
        public void ActivateButtons()
        {
            restart.gameObject.SetActive(true);
            nextLevel.gameObject.SetActive(true);
            takeInput = true;
        }
    }
}
