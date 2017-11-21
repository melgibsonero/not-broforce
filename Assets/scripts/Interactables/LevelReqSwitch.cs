using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelReqSwitch : Switch
    {
        [SerializeField]
        private int completedLevelReq = 1;

        public void Start()
        {
            if (LevelRequirementFilled())
            {
                Activate();
                //Debug.Log("[LevelReqSwitch]: Level " + completedLevelReq +
                //          " has been completed - switch activated.");
            }
        }

        public bool LevelRequirementFilled()
        {
            return (GameManager.Instance.LatestCompletedLevel >=
                    completedLevelReq);
        }
    }
}
