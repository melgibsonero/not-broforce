using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace not_broforce
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerController player;
        private BoxController boxController;
        private BoxSelector boxSelector;
        private MouseCursorController cursor;
        private UIController ui;

        Vector2 directionalInput;

        // Game state: paused
        private bool paused;

        // Axis nullifier
        private bool selectionAxisUsed;

        // Settings which affect input
        //private bool playingUsingMouse;
        private bool alwaysShowBoxSelector;
        private bool holdToActivateBoxSelector;

        public Vector2 DirectionalInput { get { return directionalInput; } }

        private void Start()
        {
            player = GetComponent<PlayerController>();
            boxController = FindObjectOfType<BoxController>();
            boxSelector = FindObjectOfType<BoxSelector>();
            cursor = FindObjectOfType<MouseCursorController>();
            ui = FindObjectOfType<UIController>();

            alwaysShowBoxSelector = 
                GameManager.Instance.AlwaysShowBoxSelector;
            holdToActivateBoxSelector = 
                GameManager.Instance.HoldToActivateBoxSelector;

            boxSelector.ShowAlways(alwaysShowBoxSelector);

            //playingUsingMouse = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!paused) // && player.enabled
            {
                CheckPlayerInput();
                CheckBoxSelectorInput();
                CheckBoxRecallInput();
            }

            CheckUIInput();
            //CheckIfPlayingUsingMouse();
        }

        private void CheckPlayerInput()
        {
            //Left and right movement
            directionalInput =
                new Vector2(Input.GetAxisRaw("Horizontal"),
                            Input.GetAxisRaw("Vertical"));

            player.SetDirectionalInput(directionalInput);



            //Jumping
            if (Input.GetButtonDown("Jump"))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetButtonUp("Jump"))
            {
                player.OnJumpInputUp();
            }
        }

        public bool CheckSInput()
        {
            return Input.GetButton("Look down");
        }

        private void CheckBoxSelectorInput()
        {
            // Selector activation
            CheckBoxSelectorActivation();

            // Placing and removing a box
            if (Input.GetButtonDown("Place Box"))
            {
                boxSelector.OnPlacementInputDown();
            }

            // Selector movement
            Utils.Direction selectorInputDir = GetSelectorDirection();

            // Moving the box selector with the
            // arrow keys or directional buttons
            if (!selectionAxisUsed && selectorInputDir != Utils.Direction.None)
            {
                boxSelector.DirectionalMovement(selectorInputDir);
            }

            // Nullifying the axis 
            if (selectorInputDir == Utils.Direction.None)
            {
                if (selectionAxisUsed)
                {
                    selectionAxisUsed = false;
                }
            }
            // A directional button is used
            else if (!selectionAxisUsed)
            {
                selectionAxisUsed = true;
                //HideCursor();
            }
        }

        public static Utils.Direction GetSelectorDirection()
        {
            // Selector movement
            Vector2 selectorMovementInput =
                new Vector2(Input.GetAxisRaw("Horizontal Selection"),
                            Input.GetAxisRaw("Vertical Selection"));

            // Moving the box selector with the
            // arrow keys or directional buttons
            if (selectorMovementInput.y > 0)
            {
                return Utils.Direction.Up;
            }
            else if (selectorMovementInput.y < 0)
            {
                return Utils.Direction.Down;
            }
            else if (selectorMovementInput.x < 0)
            {
                return Utils.Direction.Left;
            }
            else if (selectorMovementInput.x > 0)
            {
                return Utils.Direction.Right;
            }
            else
            {
                return Utils.Direction.None;
            }
        }

        private void CheckBoxSelectorActivation()
        {
            // Activating and deactivating the box selector

            if (!alwaysShowBoxSelector)
            {
                if (holdToActivateBoxSelector)
                {
                    if (Input.GetButtonDown("Activate Box Selector"))
                    {
                        boxSelector.Activate();
                    }
                    else if (Input.GetButtonUp("Activate Box Selector"))
                    {
                        boxSelector.Deactivate(forceHide: false);
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Place Box"))
                    {
                        boxSelector.Activate();
                    }
                    else if (Input.GetButtonDown("Activate Box Selector"))
                    {
                        boxSelector.ToggleActivation();
                    }
                }
            }
        }

        private void CheckBoxRecallInput()
        {
            if (Input.GetButtonDown("Recall Boxes"))
            {
                bool recalled = boxController.RecallAllBoxes();

                if (recalled)
                {
                    // Plays a sound
                    SFXPlayer.Instance.Play(Sound.TeleportFinish);
                }
            }
        }

        private void CheckUIInput()
        {
            // Pausing, resuming and retuning to the previous menu screen
            if (Input.GetButtonDown("Pause") && GameManager.Instance.ClearFade)
            {
                paused = ui.ToggleMenus();

                if (paused && holdToActivateBoxSelector)
                {
                    boxSelector.Deactivate(forceHide: false);
                }
            }

            // Resuming and retuning to the previous menu screen
            else if (Input.GetButtonDown("Cancel"))
            {
                ui.OnCancelInputDown();
            }

            // Sets pause off if the Resume button
            // has been clicked in the pause menu
            if (paused && !ui.Paused)
            {
                paused = false;
            }

            //// Changing level
            //if (Input.GetButtonDown("Accept"))
            //{
            //    ui.NextLevel();
            //}

            //// Restarting level
            //if (Input.GetButtonDown("Restart"))
            //{
            //    ui.Restart();
            //}
        }

        private void HideCursor()
        {
            cursor.PlayingUsingMouse = false;

            //if (playingUsingMouse)
            //{
            //    Debug.Log("input: cursor hidden");
            //    playingUsingMouse = false;
            //    cursor.PlayingUsingMouse = false;

            //    // Prevents the cursor being immediately shown again
            //    cursorPos = cursor.Position;
            //}
        }

        public void LevelCompleted()
        {
            // Hides the box selector
            boxSelector.Deactivate(forceHide: true);
        }

        public void SetAlwaysShowBS(bool alwaysShowBS)
        {
            alwaysShowBoxSelector = alwaysShowBS;
            boxSelector.ShowAlways(alwaysShowBoxSelector);

            //if (alwaysShowBoxSelector)
            //{
            //    SetHoldToActivateBS(false);
            //}
        }

        public void SetHoldToActivateBS(bool holdToActivateBS)
        {
            holdToActivateBoxSelector = holdToActivateBS;

            if (holdToActivateBoxSelector)
            {
                //SetAlwaysShowBS(false);
                boxSelector.Deactivate(forceHide: false);
                //ui.SetAlwaysShowBSOff();
            }
        }
    }
}
