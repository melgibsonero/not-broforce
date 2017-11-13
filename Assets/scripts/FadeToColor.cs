using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class FadeToColor : MonoBehaviour
    {
        [SerializeField]
        private Color color;

        [SerializeField]
        private float fadeOutTime = 1;

        [SerializeField]
        private float fadeInTime = 1;

        [SerializeField]
        private Camera followedCamera;

        private SpriteRenderer sr;

        private bool fadeOut;
        private float fadeProgress;
        private float elapsedTime;
        private bool active;

        public bool FadedOut
        {
            get
            {
                return (fadeOut && fadeProgress == 1);
            }
        }

        public bool FadedIn
        {
            get
            {
                return (!fadeOut && fadeProgress == 1);
            }
        }

        public void SetFollowedCamera(Camera camera)
        {
            followedCamera = camera;
        }

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();

            CheckForErrors();

            DontDestroyOnLoad(gameObject);
        }

        private void CheckForErrors()
        {
            if (followedCamera == null)
            {
                Debug.LogError("Camera component could " +
                               "not be found in the object.");
            }

            if (sr == null)
            {
                Debug.LogError("SpriteRenderer component could " +
                               "not be found in the object.");
            }
        }

        public void ResetCompatibleSwitches()
        {
            FadeActivator activator = GetComponent<FadeActivator>();

            if (activator != null)
            {
                activator.FindCompatibleSwitches();
            }
            else
            {
                Debug.LogError("FadeActivator component could " +
                               "not be found in the object.");
            }
        }

        public void StartNextFade()
        {
            fadeOut = !fadeOut;

            if (fadeOut)
            {
                StartFadeOut();
            }
            else
            {
                StartFadeIn();
            }
        }

        public void StartFadeOut()
        {
            fadeOut = true;
            StartFade();
        }

        public void StartFadeIn()
        {
            fadeOut = false;
            StartFade();
        }

        private void StartFade()
        {
            fadeProgress = 0;
            elapsedTime = 0;
            active = true;
        }

        private void FinishFade()
        {
            fadeProgress = 1;
            active = false;
        }

        private void FollowCamera()
        {
            Vector3 newPosition = followedCamera.transform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }

        private void Update()
        {
            // Moves the fade object to the camera's position
            if (followedCamera != null)
            {
                FollowCamera();
            }

            if (active)
            {
                // Increases the elapsed time
                elapsedTime += Time.deltaTime;

                // Updates the fade's progress
                UpdateFadeProgress();

                // Updates the fade object's transparency
                UpdateTransparency();
            }
        }

        private void UpdateFadeProgress()
        {
            if (fadeOut)
            {
                if (fadeOutTime <= 0)
                {
                    fadeProgress = 1;
                }
                else
                {
                    fadeProgress = elapsedTime / fadeOutTime;
                }
            }
            else
            {
                if (fadeInTime <= 0)
                {
                    fadeProgress = 1;
                }
                else
                {
                    fadeProgress = elapsedTime / fadeInTime;
                }
            }

            // If the fade's progress is complete,
            // the fading process is finished 
            if (fadeProgress >= 1.0f)
            {
                FinishFade();
            }
        }

        private void UpdateTransparency()
        {
            if (sr != null)
            {
                Color newColor = color;

                if (fadeOut)
                {
                    newColor.a = fadeProgress;
                }
                else
                {
                    newColor.a = 1.0f - fadeProgress;
                }

                sr.color = newColor;
            }
        }
    }
}
