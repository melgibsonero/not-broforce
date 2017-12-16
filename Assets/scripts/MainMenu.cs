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

        [SerializeField]
        private GameObject credits;

        [SerializeField]
        private FadeToColor fade;

        private Button[] mainButtons;
        private Button level1Button;
        private Button settingsReturn;

        private bool levelMenuOpened = false;
        private bool settingsOpened = false;

        private string SceneName(int levelNum)
        {
            return ("Level" + levelNum);
        }

        public void StartGame()
        {
            GameManager.Instance.StartSceneChange(
                SceneName(targetLevelNum));
        }

        public void StartLevel(int levelNum)
        {
            // Plays a sound
            SFXPlayer.Instance.Play(Sound.Bell);

            GameManager.Instance.SetLevel(levelNum);
            GameManager.Instance.StartSceneChange(
                    SceneName(levelNum));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ActivateLevelMenu()
        {
            ChangeLevelMenuVisibility(true, false);

            // Plays a sound
            SFXPlayer.Instance.Play(Sound.Ascend);
        }

        public void DeactivateLevelMenu(bool init)
        {
            ChangeLevelMenuVisibility(false, init);
        }

        private void ChangeLevelMenuVisibility(bool activate, bool init)
        {
            levelMenuOpened = activate;
            levelMenuButtons.SetActive(activate);

            if (activate)
            {
                // Select and highlight the first level button
                Utils.SelectButton(level1Button);

                submenuBGImage.GetComponent<RectTransform>().sizeDelta
                    = new Vector2(300, 520);

                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Ascend);
            }
            else
            {
                // Select and highlight the start button
                Utils.SelectButton(mainButtons[0]);

                if (!init)
                {
                    // Plays a sound
                    SFXPlayer.Instance.Play(Sound.Descend);
                }
            }

            ChangeSubmenuBGVisibility(activate);

            menuButtons.SetActive(!activate);
        }

        public void ActivateSettings()
        {
            ChangeSettingsVisibility(true, false);

            // Plays a sound
            SFXPlayer.Instance.Play(Sound.Ascend);
        }

        public void DeactivateSettings(bool init)
        {
            GameManager.Instance.SaveSettings();
            ChangeSettingsVisibility(false, init);

            if (!init)
            {
                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Descend);
            }
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

            ChangeSubmenuBGVisibility(activate);

            menuButtons.SetActive(!activate);
        }

        private void ChangeSubmenuBGVisibility(bool activate)
        {
            submenuBGImage.SetActive(activate);
            credits.SetActive(!activate);
        }

        private void Start()
        {
            mainButtons =
                menuButtons.GetComponentsInChildren<Button>(true);
            level1Button =
                levelMenuButtons.GetComponentInChildren<Button>(true);
            settingsReturn =
                settings.GetComponentInChildren<Button>(true);

            DeactivateLevelMenu(init: true);
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
                    DeactivateLevelMenu(false);
                }
                else if (settingsOpened)
                {
                    DeactivateSettings(false);
                }
            }
        }
    }
}
