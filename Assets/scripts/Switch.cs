using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public abstract class Switch : Activatable, IGridObject
    {
        [SerializeField]
        private GameObject player;

        [SerializeField]
        private Vector2 gridCoordinates;

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set
            {
                gridCoordinates = value;
            }
        }

        public bool SameCoordinates(Vector2 coordinates)
        {
            return (coordinates == gridCoordinates);
        }

        public bool ActivateIfSameCoordinates(Vector2 coordinates)
        {
            if (coordinates == gridCoordinates)
            {
                Activate();
                return true;
            }
            else
            {
                //Deactivate();
                return false;
            }
        }

        public void MoveToGridCoordinates()
        {
            transform.position = LevelController.GetPosFromGridCoord(
                gridCoordinates);
        }

        /// <summary>
        /// Checks if the player character is on the switch.
        /// </summary>
        protected bool CheckPlayerPresence()
        {
            // TODO: Add IGridObject to the player character
            // The player character's grid coordinates
            Vector2 playerGridCoord =
                LevelController.GetGridCoordinates(player.transform.position);

            // If the player character is on the switch, it is activated
            bool activatedByPlayer = 
                ActivateIfSameCoordinates(playerGridCoord);

            return activatedByPlayer;
        }

        /// <summary>
        /// Checks if any box from a list is on the switch.
        /// </summary>
        protected bool CheckBoxPresence(List<Box> boxes)
        {
            bool activatedByBox = false;

            foreach (Box box in boxes)
            {
                activatedByBox = SameCoordinates(box.GridCoordinates);

                // If the box is on the pressure plate, it is activated
                if (activatedByBox)
                {
                    Activate();
                    break;
                }
            }

            return activatedByBox;
        }
    }
}
