using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class FadeCatcher : MonoBehaviour
    {
        [SerializeField]
        private Camera gameCamera;

        private FadeToColor fade;

        private void Awake()
        {
            FindFade();
            fade.ResetActivator();
            FadeIn();
        }

        private void FindFade()
        {
            fade = FindObjectOfType<FadeToColor>();

            if (fade == null)
            {
                Debug.LogError("A GameObject with aFadeToColor component " +
                               "could not be found in the scene.");

                return;
            }

            fade.SetFollowedCamera(gameCamera);
        }

        public FadeToColor GetFade()
        {
            return fade;
        }

        private void FadeIn()
        {
            if (fade != null)
            {
                fade.StartFadeIn();
            }
        }
    }
}
