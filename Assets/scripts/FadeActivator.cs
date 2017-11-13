using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class FadeActivator : Activatable
    {
        [SerializeField]
        FadeToColor fade;

        List<Switch> compatibleSwitches;

        public override void Awake()
        {
            FindCompatibleSwitches();
        }

        private void FindCompatibleSwitches()
        {
            compatibleSwitches = new List<Switch>();

            Switch[] allSwitches = FindObjectsOfType<Switch>();

            // Goes through the switch list and adds any switch which
            // has a compatible activation code to compatibleSwitches
            foreach (Switch activator in allSwitches)
            {
                if (GetCode() == activator.GetCode())
                {
                    compatibleSwitches.Add(activator);
                }
            }

            // If there are no compatible switches, an error message is printed
            if (compatibleSwitches.Count == 0)
            {
                Debug.LogError("No compatible switches could be found " +
                               "in the scene. This FadeActivator " +
                               "cannot be activated.");
            }
        }

        private void Update()
        {
            // The old and the new activation state of the fadeActivator
            bool oldState = IsActivated();
            bool newState = false;

            foreach (Switch activator in compatibleSwitches)
            {
                newState = activator.IsActivated();

                if (newState)
                {
                    break;
                }
            }

            if (oldState && !newState)
            {
                Deactivate();
            }
            else if (!oldState && newState)
            {
                StartFadeOut();
            }
        }

        private void StartFadeOut()
        {
            Activate();
            fade.StartFadeOut();
        }

        private void StartFadeIn()
        {
            Activate();
        }
    }
}
