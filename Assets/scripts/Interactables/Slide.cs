
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class Slide : SpriteToggle
    {
        [SerializeField]
        private float slideTime = 1f;

        //[SerializeField]
        //private bool openByDefault;

        private float startTime;

        private float slideDistance;

        // TODO: Also startX for horizontal sliding?
        private float startY;

        private bool slideFinished = true;

        private float ratio;

        protected override void Awake()
        {
            base.Awake();

            if (slideTime < 0)
            {
                slideTime = 1f;
            }

            // The slide direction is determined by whether the
            // previous slide was to open or close the obstacle
            //slideOpen = openByDefault;

            // Sets the slide distance:
            // The height of the obstacle multiplied with some
            // number that leaves a part of the obstacle showing
            // (the number also takes the parent object's scale into account)
            slideDistance = sr.bounds.size.y * 0.55f;

            startY = transform.localPosition.y;
        }

        public override void Activate()
        {
            if (!activated)
            {
                activated = true;
                InitSlide();
            }
        }

        public override void Deactivate()
        {
            if (activated)
            {
                activated = false;
                InitSlide();
            }
        }

        private void InitSlide()
        {
            slideFinished = false;

            if (ratio > 0f)
            {
                startTime = Time.time - (1 - ratio) * slideTime;
            }
            else
            {
                startTime = Time.time;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!slideFinished)
            {
                UpdateSlide();
            }
        }

        private void UpdateSlide()
        {
            // TODO: The texture changes to be smaller

            Vector3 newPosition = transform.localPosition;

            float oldRatio = ratio;
            ratio = (Time.time - startTime) / slideTime;

            if (activated)
            {
                if (oldRatio < 0.5f && ratio >= 0.5)
                {
                    sr.sprite = activeSprite;
                }

                if (ratio >= 1f)
                {
                    newPosition.y = startY + slideDistance;
                    slideFinished = true;
                    ratio = 0f;
                }
                else
                {
                    newPosition.y = EasedSlide();
                    //newPosition.y = startY + ratio * slideDistance;
                }
            }
            else
            {
                if (oldRatio < 0.5f && ratio >= 0.5)
                {
                    sr.sprite = inactiveSprite;
                }

                if (ratio >= 1f)
                {
                    newPosition.y = startY;
                    slideFinished = true;
                    ratio = 0f;
                }
                else
                {
                    newPosition.y = EasedSlide();
                    //newPosition.y = startY + (1 - ratio) * slideDistance;
                }
            }

            transform.localPosition = newPosition;
        }

        private float EasedSlide()
        {
            float altRatio = ratio;

            if (activated)
            {
                altRatio = (Mathf.Sin(ratio * Mathf.PI - Mathf.PI / 2) + 1) / 2;

                //if (ratio < 0.5f)
                //{
                //    altRatio = Mathf.Sin(ratio * Mathf.PI / 2 - Mathf.PI / 2) + 1;
                //}
                //else
                //{
                //    altRatio = Mathf.Sin(ratio * Mathf.PI / 2);
                //}
            }
            else
            {
                altRatio = (Mathf.Sin((1 - ratio) * Mathf.PI - Mathf.PI / 2) + 1) / 2;

                //if (ratio < 0.5f)
                //{
                //    altRatio = Mathf.Sin((1 - ratio) * Mathf.PI / 2 - Mathf.PI / 2) + 1;
                //}
                //else
                //{
                //    altRatio = Mathf.Sin((1 - ratio) * Mathf.PI / 2);
                //}
            }

            return (startY + altRatio * slideDistance);
        }
    }
}
