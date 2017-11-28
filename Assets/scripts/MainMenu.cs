using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace not_broforce
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private SceneType scene;

        [SerializeField]
        private int targetLevelNum;

        [SerializeField]
        private FadeToColor fade;

        [SerializeField]
        private GameObject settings;

        [SerializeField]
        private GameObject menuButtons;

        private bool settingsOpened = false;

        //private bool gameStarted;

        //private enum Scene
        //{
        //    TestZoneX,
        //    ValtterinPlayground,
        //    KatsoaSaaMutteiKoskela
        //}

        //private string SceneName(Scene scene)
        //{
        //    // TODO: Better way to change scene

        //    string sceneName = "";

        //    switch (scene)
        //    {
        //        case Scene.TestZoneX:
        //        {
        //            sceneName = "TestZoneX";
        //            break;
        //        }
        //        case Scene.ValtterinPlayground:
        //        {
        //            sceneName = "Valtterin Playground";
        //            break;
        //        }
        //        case Scene.KatsoaSaaMutteiKoskela:
        //        {
        //            sceneName = "Katsoa saa muttei Koskela";
        //            break;
        //        }
        //    }

        //    return sceneName;
        //}


        public void StartGame()
        {
            fade.StartFadeOut();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void DeactivateSettings()
        {
            GameManager.Instance.SaveSettings();
            settingsOpened = false;
            settings.SetActive(false);
            menuButtons.SetActive(true);
        }

        public void ActivateSettings()
        {
            settingsOpened = true;
            settings.SetActive(true);
            menuButtons.SetActive(false);
        }

        private void Awake()
        {
            DeactivateSettings();
        }

        private void Update()
        {
            if (fade.FadedOut)
            {
                GameManager.Instance.LoadScene(
                    LevelChanger_Hub.SceneName(scene, targetLevelNum));
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(settingsOpened)
                {
                    DeactivateSettings();
                }
            }
        }
    }
}
