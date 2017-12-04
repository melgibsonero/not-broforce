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
        private UIController ui;
        private MouseCursorController cursor;

        // Game state: paused
        private bool paused;

        // Axis nullifier
        private bool selectionAxisUsed;

        // Settings which affect input
        private bool playingUsingMouse;
        private bool alwaysShowBoxSelector;
        private bool holdToActivateBoxSelector;

        // Use this for initialization
        private void Start()
        {
            player = GetComponent<PlayerController>();
            boxController = FindObjectOfType<BoxController>();
            boxSelector = FindObjectOfType<BoxSelector>();
            ui = FindObjectOfType<UIController>();
            cursor = FindObjectOfType<MouseCursorController>();

            alwaysShowBoxSelector = 
                GameManager.Instance.AlwaysShowBoxSelector;
            holdToActivateBoxSelector = 
                GameManager.Instance.HoldToActivateBoxSelector;

            boxSelector.ShowAlways(alwaysShowBoxSelector);

            playingUsingMouse = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!paused)
            {
                CheckPlayerInput();
                CheckBoxSelectorInput();
                CheckBoxRecallInput();
            }

            CheckUIInput();
            CheckIfPlayingUsingMouse();
        }

        private void CheckPlayerInput()
        {
            //Left and right movement
            Vector2 directionalInput =
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

        private void CheckBoxSelectorInput()
        {
            CheckBoxSelectorActivation();

            Vector2 directionalInput =
                new Vector2(Input.GetAxisRaw("Horizontal Selection"),
                            Input.GetAxisRaw("Vertical Selection"));

            // Moving the box selector with the
            // arrow keys or directional buttons
            if (!selectionAxisUsed)
            {
                if (directionalInput.y > 0)
                {
                    boxSelector.DirectionalMovement(Utils.Direction.Up);
                }
                else if (directionalInput.y < 0)
                {
                    boxSelector.DirectionalMovement(Utils.Direction.Down);
                }
                else if (directionalInput.x < 0)
                {
                    boxSelector.DirectionalMovement(Utils.Direction.Left);
                }
                else if (directionalInput.x > 0)
                {
                    boxSelector.DirectionalMovement(Utils.Direction.Right);
                }
            }

            if (directionalInput == Vector2.zero)
            {
                if (selectionAxisUsed)
                {
                    selectionAxisUsed = false;
                }
            }
            else if (!selectionAxisUsed)
            {
                selectionAxisUsed = true;
                playingUsingMouse = false;
            }

            // Placing and removing a box
            if (Input.GetButtonDown("Place Box"))
            {
                boxSelector.OnPlacementInputDown();
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
                        boxSelector.Deactivate();
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
                boxController.RecallAllBoxes();
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
                    boxSelector.HideSelector();
                }
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

        private void CheckIfPlayingUsingMouse()
        {
            if (!playingUsingMouse)
            {
                cursor.PlayingUsingMouse = false;

                // Using mouse buttons shows the mouse cursor
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    Debug.Log("mouse on");
                    playingUsingMouse = true;
                    cursor.PlayingUsingMouse = true;
                }
            }
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
                boxSelector.HideSelector();
                //ui.SetAlwaysShowBSOff();
            }
        }
    }
}
