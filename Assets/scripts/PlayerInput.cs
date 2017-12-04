using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace not_broforce
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerController player;
        private BoxSelector boxSelector;
        private UIController ui;

        public Vector2 directionalInput;

        // Game state: paused
        private bool paused;

        // Settings which affect input
        private bool alwaysShowBoxSelector;
        private bool holdToActivateBoxSelector;

        // Use this for initialization
        private void Start()
        {
            player = GetComponent<PlayerController>();
            boxSelector = FindObjectOfType<BoxSelector>();
            ui = FindObjectOfType<UIController>();

            alwaysShowBoxSelector = 
                GameManager.Instance.AlwaysShowBoxSelector;
            holdToActivateBoxSelector = 
                GameManager.Instance.HoldToActivateBoxSelector;

            boxSelector.ShowAlways(alwaysShowBoxSelector);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!paused)
            {
                CheckPlayerInput();
                CheckBoxSelectorInput();
            }

            CheckUIInput();
        }

        private void CheckPlayerInput()
        {
            //Left and right movement
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            //Jumping
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W))
            {
                player.OnJumpInputUp();
            }
        }

        private void CheckBoxSelectorInput()
        {
            CheckBoxSelectorActivation();

            // Moving the box selector with the arrow keys
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                boxSelector.DirectionalMovement(Utils.Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                boxSelector.DirectionalMovement(Utils.Direction.Down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                boxSelector.DirectionalMovement(Utils.Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                boxSelector.DirectionalMovement(Utils.Direction.Right);
            }

            // Placing and removing a box
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                boxSelector.OnPlacementInputDown();
            }
        }

        private void CheckBoxSelectorActivation()
        {
            // TODO: Use the settings to determine controls
            // Activating and deactivating the box selector

            if (!alwaysShowBoxSelector)
            {
                if (holdToActivateBoxSelector)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        boxSelector.Activate();
                    }
                    else if (Input.GetMouseButtonUp(1))
                    {
                        boxSelector.Deactivate();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        boxSelector.Activate();
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        boxSelector.ToggleActivation();
                    }
                }
            }
        }

        private void CheckUIInput()
        {
            // Pausing, resuming and retuning to the previous menu screen
            if (Input.GetKeyDown(KeyCode.Escape))
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

            // Changing level
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ui.NextLevel();
            }

            // Restarting level
            if (Input.GetKeyDown(KeyCode.R))
            {
                ui.Restart();
            }
        }

        public void SetAlwaysShowBS(bool alwaysShowBS)
        {
            alwaysShowBoxSelector = alwaysShowBS;
            boxSelector.ShowAlways(alwaysShowBoxSelector);
        }

        public void SetHoldToActivateBS(bool holdToActivateBS)
        {
            holdToActivateBoxSelector = holdToActivateBS;

            if (holdToActivateBoxSelector)
            {
                boxSelector.HideSelector();
            }
        }
    }
}
