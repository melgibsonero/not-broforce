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

        public Vector2 directionalInput;

        // Game state: paused
        private bool paused;

        // Axis nullifier
        private bool selectionAxisUsed;

        // Settings which affect input
        private bool playingUsingMouse;
        private bool alwaysShowBoxSelector;
        private bool holdToActivateBoxSelector;

        // Mouse cursor positions
        private Vector2 cursorPos;
        private Vector2 oldCursorPos;

        // Use this for initialization
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
            Utils.Direction selectorInputDir = SelectorDirection();

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
                HideCursor();
            }
        }

        public static Utils.Direction SelectorDirection()
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

        //private void CheckIfPlayingUsingMouse()
        //{
        //    if (!playingUsingMouse)
        //    {
        //        // Sets the cursor's old and new
        //        // positions for checking if it moved
        //        oldCursorPos = cursorPos;
        //        cursorPos = cursor.Position;

        //        // Moving the mouse or using its buttons shows the mouse cursor
        //        if (cursorPos != oldCursorPos ||
        //            Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        //        {
        //            Debug.Log("input: cursor shown");
        //            playingUsingMouse = true;
        //            cursor.PlayingUsingMouse = true;
        //        }
        //    }
        //}

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
