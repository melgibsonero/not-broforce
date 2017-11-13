using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PassableObstacle : Activatable
    {
        private BoxCollider2D boxCollider;

        private List<Switch> compatibleSwitches;

        public override void Awake()
        {
            base.Awake();

            InitCollider();

            FindCompatibleSwitches();
        }

        private void InitCollider()
        {
            boxCollider = GetComponent<BoxCollider2D>();

            // If a collider could not be found, an error message is printed
            if (boxCollider == null)
            {
                Debug.LogError("BoxCollider2D component could " +
                               "not be found in the object.");
            }
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
                               "in the scene. This PassableObstacle " +
                               "cannot be activated.");
            }
        }

        private void Update()
        {
            // The old and the new activation state of the obstacle
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
                MakeImpassable();
            }
            else if (!oldState && newState)
            {
                MakePassable();
            }
        }

        private void MakePassable()
        {
            Activate();

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }

        private void MakeImpassable()
        {
            Deactivate();

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }
    }
}
