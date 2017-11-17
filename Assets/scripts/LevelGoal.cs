using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelGoal : Switch
    {
        [SerializeField]
        private LevelChanger endScreen;

        private void Update()
        {
            if (!IsActivated())
            {
                bool goalReached = CheckPlayerPresence();

                if (goalReached)
                {
                    // TODO: Get the info about where the player goes
                    // from endScreen and give it to GameManager

                    Debug.Log("Level completed: " +
                              GameManager.Instance.CurrentLevel);

                    endScreen.ActivateButtons();
                    GameManager.Instance.LevelCompleted();
                    //SaveGame();
                }
            }
        }

        //private void SaveGame()
        //{
        //    GameManager.Instance.LevelCompleted();
        //    //SaveManager.Instance.EnterLevel(SaveManager.Instance.CurrentLevel + 1);
        //}
    }
}
