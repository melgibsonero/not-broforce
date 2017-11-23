using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace not_broforce
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        PlayerController player;
        BoxSelector boxSelector;

        // Use this for initialization
        private void Start()
        {
            player = GetComponent<PlayerController>();
            boxSelector = FindObjectOfType<BoxSelector>();
        }

        // Update is called once per frame
        private void Update()
        {
            #region Player controls

            //Left and right movement
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            //Jumping
			if (Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.W))
            {
                player.OnJumpInputDown();
            }
			if (Input.GetKeyUp(KeyCode.Space)||Input.GetKeyUp(KeyCode.W))
            {
                player.OnJumpInputUp();
            }
            #endregion

            #region Box selector controls

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

            // Activating the box selector
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                boxSelector.OnActivationInputDown();
            }
            // Deactivating the box selector
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                boxSelector.OnActivationInputUp();
            }

            // Placing and removing a box
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                boxSelector.OnPlacementInputDown();
            }
            #endregion
        }
    }
}
