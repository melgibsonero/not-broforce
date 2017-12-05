using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace not_broforce
{
    public class LevelMenuButton : MonoBehaviour
    {
        [SerializeField]
        private MainMenu mainMenu;

        [SerializeField]
        private int level;

        private int latestCompletedLevel;

        private Button button;
        private Text label;

        private bool locked;

        private bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                button.interactable = !locked;

                // Sets the font color
                Color newColor = label.color;
                if (locked)
                {
                    newColor.a = 100 / 255f;
                }
                else
                {
                    newColor.a = 1f;
                }
                label.color = newColor;
            }
        }

        private void Start()
        {
            latestCompletedLevel = GameManager.Instance.LatestCompletedLevel;

            button = GetComponent<Button>();
            label = GetComponentInChildren<Text>();

            if (level > latestCompletedLevel + 1)
            {
                Locked = true;
            }
        }

        public void OnClick()
        {
            if (!Locked)
            {
                mainMenu.StartLevel(level);
            }
        }
    }
}
