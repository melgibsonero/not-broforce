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

        private void CheckIfPressed()
        {
            Vector2 playerGridCoord =
                LevelController.GetGridCoordinates(player.transform.position);

            bool oldState = IsActivated();
            bool newState;

            newState = ActivateIfSameCoordinates(playerGridCoord);

            if (!newState)
            {
                foreach (Box box in placedBoxes)
                {
                    newState = SameCoordinates(box.GridCoordinates);
                    if (newState)
                    {
                        Activate();
                        break;
                    }
                }

                if (oldState && !newState)
                {
                    Deactivate();
                }
            }
        }
    }
}
