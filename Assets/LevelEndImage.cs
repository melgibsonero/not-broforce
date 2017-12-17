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

        [SerializeField]
        private Text levelCompletedText;

        [SerializeField]
        private Text gameCompletedText;

        private int levelNum;

        private bool finalLevel;

        private void Start()
        {
            Image image = GetComponent<Image>();
            UIController ui = GetComponentInParent<UIController>();

            if (ui != null)
            {
                levelNum = ui.CurrentLevel();
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
                levelCompletedText.text = "Level " + levelNum + " Completed!";
            }

            levelCompletedText.enabled = !finalLevel;
            gameCompletedText.enabled = finalLevel;
        }
    }
}
