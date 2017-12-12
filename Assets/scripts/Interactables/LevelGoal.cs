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

        private Animator teleportAnim;

        private AudioSource teleportSound;

        private void Start()
        {
            // Sets the teleport animator
            // (the child object's animator)

            Transform child = transform.GetChild(0);

            if (child != null)
            {
                teleportAnim = child.GetComponent<Animator>();
            }
            else
            {
                Debug.LogError("[LevelGoal]: No child object found");
            }
        }

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

                    // Plays the teleport start sound
                    teleportSound = SFXPlayer.Instance.Play(Sound.TeleportStart);

                    // Activates the level end screen
                    endScreen.ActivateButtons();

                    // Sets the level completed and saves the game
                    GameManager.Instance.LevelCompleted();

                    // Sets the player character inactive
                    PlayerController player = FindObjectOfType<PlayerController>();
                    player.gameObject.SetActive(false);

                    // Moves the dummy player character to the actual pc's position
                    teleportAnim.gameObject.transform.position =
                        player.transform.position;

                    // Plays teleport animation
                    teleportAnim.Play("TeleportAway");
                }
            }
        }

        /// <summary>
        /// Finishes the teleport sound effect.
        /// </summary>
        public void FinishTeleport()
        {
            // Stops the teleport start sound
            teleportSound.Stop();
            teleportSound.enabled = false;

            // Plays the teleport finish sound
            SFXPlayer.Instance.Play(Sound.TeleportFinish);
        }
    }
}
