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

        private ParticleSystem glowparticles;

        public override void Awake()
        {
            base.Awake();

            placedBoxes = boxController.GetPlacedBoxes();
        }
        private void Start()
        {
            glowparticles = GetComponentInChildren<ParticleSystem>();
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

        public override void Activate()
        {
            if (!activated)
            {
                activated = true;

                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Ascend);
                var emission = glowparticles.emission;
                emission.enabled = false;
            }
        }

        public override void Deactivate()
        {
            if (activated)
            {
                activated = false;

                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Descend);
                var emission = glowparticles.emission;
                emission.enabled = true;
            }
        }
    }
}
