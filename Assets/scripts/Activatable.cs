﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public abstract class Activatable : MonoBehaviour
    {
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

                if (sr != null)
                {
                    sr.sprite = onSprite;
                }

                //Debug.Log("Activated");
            }
        }

        public void Deactivate()
        {
            if (activated)
            {
                activated = false;

                if (sr != null)
                {
                    sr.sprite = offSprite;
                }

                //Debug.Log("Deactivated");
            }
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
