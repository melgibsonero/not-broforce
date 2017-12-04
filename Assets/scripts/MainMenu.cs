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
        private GameObject levelMenu;

        [SerializeField]
        private GameObject settings;

        [SerializeField]
        private GameObject submenuBGImage;

        [SerializeField]
        private GameObject menuButtons;

        private bool levelMenuOpened = false;
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
            GameManager.Instance.StartSceneChange(
                    LevelChanger_Hub.SceneName(scene, targetLevelNum));
        }

        public void StartLevel(int levelNum)
        {
            GameManager.Instance.StartSceneChange(
                    LevelChanger_Hub.SceneName(SceneType.Level,
                                               levelNum));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ActivateLevelMenu()
        {
            ChangeLevelMenuVisibility(true);
        }

        public void DeactivateLevelMenu()
        {
            ChangeLevelMenuVisibility(false);
        }

        private void ChangeLevelMenuVisibility(bool activate)
        {
            levelMenuOpened = activate;
            levelMenu.SetActive(activate);

            if (activate)
            {
                submenuBGImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(300, 520);
            }
            submenuBGImage.SetActive(activate);

            menuButtons.SetActive(!activate);
        }

        public void ActivateSettings()
        {
            ChangeSettingsVisibility(true);
        }

        public void DeactivateSettings()
        {
            GameManager.Instance.SaveSettings();
            ChangeSettingsVisibility(false);
        }

        private void ChangeSettingsVisibility(bool activate)
        {
            settingsOpened = activate;
            settings.SetActive(activate);

            if (activate)
            {
                submenuBGImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(280, 300);
            }
            submenuBGImage.SetActive(activate);

            menuButtons.SetActive(!activate);
        }

        private void Awake()
        {
            DeactivateLevelMenu();
            DeactivateSettings();
        }

        private void Update()
        {
            // TODO: PlayerInput

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (levelMenuOpened)
                {
                    DeactivateLevelMenu();
                }
                else if (settingsOpened)
                {
                    DeactivateSettings();
                }
            }
        }
    }
}
