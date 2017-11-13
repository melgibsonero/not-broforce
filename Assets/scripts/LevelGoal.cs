using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelGoal : Switch
    {
        private void Update()
        {
            if (!IsActivated())
            {
                bool goalReached = CheckPlayerPresence();

                if (goalReached)
                {
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
