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
        private Sprite inactiveSprite;

        [SerializeField]
        private Sprite activeSprite;

        private Activatable activatable;

        private SpriteRenderer sr;

        private bool activated = false;

        private void Awake()
        {
            activatable = GetComponent<Activatable>();
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

        private void Update()
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

        public void Activate()
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

        public void Deactivate()
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
