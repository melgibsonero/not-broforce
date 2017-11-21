using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PressurePlate : SwitchInteractable
    {
        [SerializeField]
        private BoxController boxController;

        private List<Box> placedBoxes;

        public override void Awake()
        {
            base.Awake();

            placedBoxes = boxController.GetPlacedBoxes();
        }

        private void Update()
        {
            // The old and the new activation state of the switch
            bool oldState = IsActivated();
            bool newState;

            newState = CheckPlayerPresence();

            // If the pressure plate is not activated by
            // the player character, it is checked if any
            // of the placed boxes is in the same grid coordinates
            if (!newState)
            {
                newState = CheckBoxPresence(placedBoxes);
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
