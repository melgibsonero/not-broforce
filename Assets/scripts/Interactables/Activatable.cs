using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public abstract class Activatable : MonoBehaviour
    {
        [SerializeField]
        protected int activationCode;

        [SerializeField]
        private bool enableGizmo;

        protected bool activated = false;

        public virtual void Awake() { }

        public int GetCode()
        {
            return activationCode;
        }

        public bool CodeIsUnusable()
        {
            return (activationCode <= 0);
        }

        public bool CodesMatch(int code)
        {
            if (CodeIsUnusable())
            {
                return false;
            }
            else
            {
                return (code == activationCode);
            }
        }

        public bool IsActivated()
        {
            return activated;
        }

        public bool IsActivated(int code)
        {
            if (CodeIsUnusable())
            {
                return activated;
            }
            else
            {
                return (code == activationCode && activated);
            }
        }

        public virtual void Activate()
        {
            if (!activated)
            {
                activated = true;
                //Debug.Log("Activated");
            }
        }

        public virtual void Deactivate()
        {
            if (activated)
            {
                activated = false;
                //Debug.Log("Deactivated");
            }
        }

        private void OnDrawGizmos()
        {
            if (enableGizmo)
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
                // Sets the gizmo's color to correspond with the
                // switch's activation code if it is not activated
                else
                {
                    if (CodeIsUnusable())
                    {
                        Gizmos.color = Color.black;
                    }
                    else
                    {
                        // TODO: At least 5 easily told apart colors

                        Gizmos.color = new Color(1,
                                                 1 - (0.1f * 2 * activationCode),
                                                 1 - 0.1f * 1 * activationCode);
                    }

                    //Gizmos.color = Color.red;
                }

                // Draws a sphere
                Gizmos.DrawSphere(transform.position, radius);
            }
        }
    }
}
