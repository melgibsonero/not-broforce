using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class SpriteToggle : MonoBehaviour
    {
        // TODO: Fix when optimizing
        //[SerializeField]
        //private bool updateOnce;

        [SerializeField]
        protected Sprite inactiveSprite;

        [SerializeField]
        protected Sprite activeSprite;

        protected Activatable activatable;

        protected SpriteRenderer sr;

        protected bool activated = false;

        protected virtual void Awake()
        {
            activatable = GetComponent<Activatable>();

            if (activatable == null)
            {
                activatable = GetComponentInParent<Activatable>();
            }
            
            sr = GetComponent<SpriteRenderer>();

            if (activatable == null)
            {
                Debug.LogError("Activatable component could not " +
                               "be found in the object.");
            }
            if (sr == null)
            {
                Debug.LogError("SpriteRenderer component could not " +
                               "be found in the object.");
            }
        }

        protected virtual void Update()
        {
            if (activatable != null)
            {
                if (activatable.IsActivated())
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }

            //if (updateOnce)
            //{
            //    enabled = false;
            //}
        }

        public virtual void Activate()
        {
            if (!activated)
            {
                activated = true;

                if (sr != null)
                {
                    sr.sprite = activeSprite;
                }
            }
        }

        public virtual void Deactivate()
        {
            if (activated)
            {
                activated = false;

                if (sr != null)
                {
                    sr.sprite = inactiveSprite;
                }
            }
        }
    }
}
