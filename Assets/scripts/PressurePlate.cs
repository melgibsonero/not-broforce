using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PressurePlate : Switch
    {
        [SerializeField]
        private BoxController boxController;

        [SerializeField]
        private GameObject player;

        private List<Box> placedBoxes;

        public override void Awake()
        {
            base.Awake();

            placedBoxes = boxController.GetPlacedBoxes();
        }

        private void Update()
        {
            CheckIfPressed();
        }

        /// <summary>
        /// Checks if the pressure plate is pressed down
        /// by the player character or any placed box.
        /// </summary>
        private void CheckIfPressed()
        {
            // TODO: Add IGridObject to the player character
            // The player character's grid coordinates
            Vector2 playerGridCoord =
                LevelController.GetGridCoordinates(player.transform.position);

            // The old and the new activation state of the pressure plate
            bool oldState = IsActivated();
            bool newState;

            // If the player character is on the
            // pressure plate, it is activated
            newState = ActivateIfSameCoordinates(playerGridCoord);

            // If the pressure plate is not activated by
            // the player character, it is checked if any
            // of the placed boxes is in the same grid coordinates
            if (!newState)
            {
                foreach (Box box in placedBoxes)
                {
                    newState = SameCoordinates(box.GridCoordinates);

                    // If the box is on the pressure plate, it is activated
                    if (newState)
                    {
                        Activate();
                        break;
                    }
                }

                // If the pressure plate was previously activated but
                // nothing is activating it anymore, it is deactivated
                if (oldState && !newState)
                {
                    Deactivate();
                }
            }
        }
    }
}
