using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace not_broforce
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Scene scene;

        [SerializeField]
        private FadeToColor fade;

        private bool gameStarted;

        private enum Scene
        {
            TestZoneX,
            ValtterinPlayground,
            KatsoaSaaMutteiKoskela
        }

        private string SceneName(Scene scene)
        {
            // TODO: Better way to change scene

            string sceneName = "";

            switch (scene)
            {
                case Scene.TestZoneX:
                {
                    sceneName = "TestZoneX";
                    break;
                }
                case Scene.ValtterinPlayground:
                {
                    sceneName = "Valtterin Playground";
                    break;
                }
                case Scene.KatsoaSaaMutteiKoskela:
                {
                    sceneName = "Katsoa saa muttei Koskela";
                    break;
                }
            }

            return sceneName;
        }

        public void StartGame()
        {
            fade.StartFadeOut();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Update()
        {
            if (fade.FadedOut)
            {
                SceneManager.LoadScene(SceneName(scene));
                fade.StartFadeIn();
            }
        }
    }
}
