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
        private float fadeTime;

        [SerializeField]
        private Camera followedCamera;

        private SpriteRenderer sr;

        private Timer fadeTimer;

        private bool active;
        private bool fadeOut;

        private float fadeProgress;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            fadeTimer = new Timer(fadeTime);

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

            DontDestroyOnLoad(gameObject);
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
            fadeTimer.Start();
            active = true;
        }

        private void FinishFade()
        {
            fadeProgress = 1;
            fadeTimer.Stop();
            active = false;
        }

        private void Update()
        {
            if (followedCamera != null)
            {
                FollowCamera();
            }

            if (active)
            {
                fadeTimer.Update();

                fadeProgress = (fadeTime - fadeTimer.TimeLeft()) / fadeTime;

                UpdateTransparency();

                if (fadeProgress >= 1.0f)
                {
                    FinishFade();
                }
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

        private void FollowCamera()
        {
            Vector3 newPosition = followedCamera.transform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}
