using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelGoal : SwitchInteractable
    {
        [SerializeField]
        private UIController endScreen;

        private void Update()
        {
            if (!IsActivated())
            {
                bool goalReached = CheckPlayerPresence();

                if (goalReached)
                {
                    // TODO: Get the info about where the player goes
                    // from endScreen and give it to GameManager.
                    // This is to set the next level number correctly.

                    // Prints debug info
                    Debug.Log("Level completed: " +
                              GameManager.Instance.CurrentLevel);

                    // Plays a sound
                    SFXPlayer.Instance.Play(Sound.Score);

                    // Activates the level end screen
                    endScreen.ActivateButtons();

                    // Sets the level completed and saves the game
                    GameManager.Instance.LevelCompleted();
                }
            }
        }
    }
}
