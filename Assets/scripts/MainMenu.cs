using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace not_broforce
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private SceneType scene;

        [SerializeField]
        private int targetLevelNum;

        [SerializeField]
        private GameObject menuButtons;

        [SerializeField]
        private GameObject levelMenuButtons;

        [SerializeField]
        private GameObject settings;

        [SerializeField]
        private GameObject submenuBGImage;

        //[SerializeField]
        //private EventSystem eventSystem;

        [SerializeField]
        private FadeToColor fade;

        private Button[] mainButtons;
        private Button level1Button;
        private Button settingsReturn;

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
            GameManager.Instance.SetLevel(levelNum);
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
            levelMenuButtons.SetActive(activate);

            if (activate)
            {
                // Select and highlight the first level button
                Utils.SelectButton(level1Button);

                submenuBGImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(300, 520);
            }
            else
            {
                // Select and highlight the start button
                Utils.SelectButton(mainButtons[0]);
            }

            submenuBGImage.SetActive(activate);

            menuButtons.SetActive(!activate);
        }

        public void ActivateSettings()
        {
            ChangeSettingsVisibility(true, false);
        }

        public void DeactivateSettings(bool init)
        {
            GameManager.Instance.SaveSettings();
            ChangeSettingsVisibility(false, init);
        }

        private void ChangeSettingsVisibility(bool activate, bool init)
        {
            settingsOpened = activate;
            settings.SetActive(activate);

            if (activate)
            {
                // Select and highlight the return button
                Utils.SelectButton(settingsReturn);

                submenuBGImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(280, 300);
            }
            else if (!init)
            {
                // Select and highlight the settings button
                Utils.SelectButton(mainButtons[1]);
            }

            submenuBGImage.SetActive(activate);

            menuButtons.SetActive(!activate);
        }

        private void Start()
        {
            mainButtons =
                menuButtons.GetComponentsInChildren<Button>(true);
            level1Button =
                levelMenuButtons.GetComponentInChildren<Button>(true);
            settingsReturn =
                settings.GetComponentInChildren<Button>(true);

            DeactivateLevelMenu();
            DeactivateSettings(init: true);

            // Select and highlight the start button
            Utils.SelectButton(mainButtons[0]);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Pause"))
            {
                if (levelMenuOpened)
                {
                    DeactivateLevelMenu();
                }
                else if (settingsOpened)
                {
                    DeactivateSettings(false);
                }
            }
        }
    }
}
