using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public abstract class Switch : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private GameObject player;

        [SerializeField]
        private Vector2 gridCoordinates;

        [SerializeField]
        private int activationCode;

        //[SerializeField]
        //private LayerMask possibleActivators;

        [SerializeField]
        private Sprite onSprite;

        [SerializeField]
        private Sprite offSprite;

        private SpriteRenderer sr;

        private bool activated = false;

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set
            {
                gridCoordinates = value;
            }
        }

        public virtual void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            if (sr == null)
            {
                Debug.LogError("SpriteRenderer component could not " +
                               "be found in the object.");
            }
        }

        public int GetCode()
        {
            return activationCode;
        }

        public bool IsActivated()
        {
            return activated;
        }

        public bool IsActivated(int code)
        {
            return (code == activationCode && activated);
        }

        public void Activate()
        {
            if (!activated)
            {
                activated = true;
                sr.sprite = onSprite;

                //Debug.Log("Switch on");
            }
        }

        public void Deactivate()
        {
            if (activated)
            {
                activated = false;
                sr.sprite = offSprite;

                //Debug.Log("Switch off");
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

        private void OnDrawGizmos()
        {
            // Radius of a sphere
            float radius = 0.2f;

            // A point above the switch
            //Vector3 pointAbove = transform.position + Vector3.up;

            // Sets the gizmo's color green if the switch is activated
            if (activated)
            {
                Gizmos.color = Color.green;
            }
            // Sets the gizmo's color red if the switch is not activated
            else
            {
                Gizmos.color = Color.red;
            }

            // Draws a sphere
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
