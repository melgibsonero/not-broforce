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

        public FadeToColor Fade { get; private set; }

        private void Awake()
        {
            FindFade();

            // Initializes the fade
            Fade.InitAfterSceneChange(gameCamera);
        }

        private void FindFade()
        {
            // Finds all fade objects in the scene
            FadeToColor[] fades = FindObjectsOfType<FadeToColor>();

            // A temporary fade to which a possible debug fade is set
            FadeToColor tempFade = null;

            // Goes through all fade objects
            foreach (FadeToColor f in fades)
            {
                // If the fade is being used, it is
                // immediately set to fade and the
                // loop is ended
                if (f.UsedInAllScenes)
                {
                    Fade = f;
                    Debug.Log("Fade successfully caught from previous scene");
                    break;
                }
                // Otherwise the unused fade is set to tempFade
                else
                {
                    tempFade = f;
                }
            }

            // No used fade found
            if (Fade == null)
            {
                // An unused fade found
                if (tempFade != null)
                {
                    Fade = tempFade;
                    Debug.Log("Only a debug fade found");
                }
                // No fade found
                else
                {
                    Debug.LogError("A GameObject with a FadeToColor component " +
                                   "could not be found in the scene.");

                    return;
                }
            }
        }

        private void FadeIn()
        {
            if (Fade != null)
            {
                Fade.StartFadeIn();
            }
        }
    }
}
