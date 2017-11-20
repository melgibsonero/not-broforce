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

        public override void Awake()
        {
            DisableStateSprites();
        }

        public void Start()
        {
            if (LevelRequirementFilled())
            {
                Activate();
                //Debug.Log("[LevelReqSwitch]: Level " + completedLevelReq +
                //          " has been completed - switch activated.");
            }
        }

        //private void Update()
        //{
        //    if (!IsActivated())
        //    {
        //        if (LevelRequirementFilled())
        //        {
        //            Activate();
        //        }
        //    }
        //}

        public bool LevelRequirementFilled()
        {
            return (GameManager.Instance.LatestCompletedLevel >=
                    completedLevelReq);
        }
    }
}
