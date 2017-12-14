using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace not_broforce
{
    public class LevelEndImage : MonoBehaviour
    {
        [SerializeField]
        private Sprite levelCompletedSprite;

        [SerializeField]
        private Sprite gameCompletedSprite;

        private bool finalLevel;

        private void Start()
        {
            Image image = GetComponent<Image>();
            UIController ui = GetComponentInParent<UIController>();

            if (ui != null)
            {
                finalLevel = ui.CurrentLevelIsFinal();
            }
            else
            {
                Debug.LogError("UIController component not found in parent.");
            }


            if (finalLevel)
            {
                image.sprite = gameCompletedSprite;
            }
            else
            {
                image.sprite = levelCompletedSprite;
            }
        }
    }
}
