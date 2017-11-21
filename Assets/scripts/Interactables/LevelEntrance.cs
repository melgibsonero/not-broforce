using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelEntrance : SwitchInteractable
    {
        [SerializeField]
        private int levelNumber;

        private void Update()
        {
            if (!IsActivated())
            {
                if (levelNumber > 0 &&
                    GameManager.Instance.CurrentLevel != levelNumber)
                {
                    bool reached = CheckPlayerPresence();

                    if (reached)
                    {
                        if (GameManager.Instance.CurrentLevel != 0)
                        {
                            Debug.LogError("A LevelEntrance was reached " +
                                           "but the current level number " +
                                           "is not 0 (hub level).");
                        }

                        GameManager.Instance.SetLevel(levelNumber);

                        Deactivate();
                    }
                }
            }
        }
    }
}
