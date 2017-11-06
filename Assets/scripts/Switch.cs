using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class Switch : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private LevelController levelController;

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

                Debug.Log("Switch on");
            }
        }

        public void Deactivate()
        {
            if (activated)
            {
                activated = false;
                sr.sprite = offSprite;

                Debug.Log("Switch off");
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
    }
}
