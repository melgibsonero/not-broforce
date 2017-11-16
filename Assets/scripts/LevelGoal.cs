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
                    endScreen.ActivateButtons();
                    Debug.Log("LEVEL COMPLETED!");
                    SaveGame();
                }
            }
        }

        private void SaveGame()
        {
            int levelsCompleted = PlayerPrefs.GetInt("levelsCompleted", 0);
            levelsCompleted++;
            PlayerPrefs.SetInt("levelsCompleted", levelsCompleted);

            Debug.Log("levelsCompleted: " + levelsCompleted);

            PlayerPrefs.Save();
        }
    }
}
