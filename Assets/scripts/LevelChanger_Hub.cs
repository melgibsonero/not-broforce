using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace not_broforce
{
    public enum SceneType
    {
        HubLevel,
        Level
    }

    public class LevelChanger_Hub : MonoBehaviour
    {
        // TODO: Merge the LevelChangers or remove this

        [SerializeField]
        private SceneType scene;

        [SerializeField]
        private int targetLevelNum;

        [SerializeField]
        private FadeCatcher fadeCatcher;

        private FadeToColor fade;

        private string currentScene = "";

        private bool levelTransition;

        public static string SceneName(SceneType scene, int levelNum)
        {
            string sceneName = "";

            switch (scene)
            {
                case SceneType.HubLevel:
                {
                    sceneName = "Hub";
                    break;
                }
                case SceneType.Level:
                {
                    if (levelNum == 5)
                    {
                        sceneName = "TestZoneX";
                    }
                    //else if (levelNum == 6)
                    //{
                    //    sceneName = "Valtterin Playground";
                    //}
                    else if (levelNum == 7)
                    {
                        sceneName = "Katsoa saa muttei Koskela";
                    }

                    // TODO: In the final game, only this is used
                    else
                    {
                        sceneName = "Level" + levelNum;
                    }
                    break;
                }
                // TODO:
                //case SceneType.Level:
                //{
                //    sceneName = "Level" + nextLevelNum;
                //    break;
                //}
            }

            return sceneName;
        }


        private void Awake()
        {
            Scene scene = SceneManager.GetActiveScene();
            currentScene = scene.name;

            targetLevelNum = 0;

            if (fadeCatcher != null)
            {
                fade = fadeCatcher.Fade;
            }
        }

        private void Update()
        {
            if (!levelTransition)
            {
                if (targetLevelNum != GameManager.Instance.CurrentLevel)
                {
                    if (GameManager.Instance.CurrentLevel == 0)
                    {
                        StartHubLevelTransition();
                    }
                    else
                    {
                        StartLevelTransition();
                    }
                }
            }
            else
            {
                if (fade == null || fade.FadedOut)
                {
                    GoToLevel();
                }
            }
        }

        private void StartLevelTransition()
        {
            targetLevelNum = GameManager.Instance.CurrentLevel;
            scene = SceneType.Level;
            levelTransition = true;

            if (fade != null)
            {
                fade.StartFadeOut();
            }

            Debug.Log("Transitioning to level " +
                      GameManager.Instance.CurrentLevel);
        }

        private void StartHubLevelTransition()
        {
            targetLevelNum = 0;
            scene = SceneType.HubLevel;
            levelTransition = true;

            if (fade != null)
            {
                fade.StartFadeOut();
            }

            Debug.Log("Transitioning to the hub level");
        }

        public void GoToLevel()
        {
            levelTransition = false;
            GameManager.Instance.StartSceneChange(SceneName(scene, targetLevelNum));

            // TODO: Start fade-in in a level's endScreen
        }

        public void RestartLevel()
        {
            GameManager.Instance.StartSceneChange(currentScene);
        }
    }
}
