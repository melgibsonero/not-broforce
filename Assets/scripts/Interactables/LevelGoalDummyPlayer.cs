using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelGoalDummyPlayer : MonoBehaviour
    {
        private LevelGoal levelGoal;

        private void Start()
        {
            levelGoal = GetComponentInParent<LevelGoal>();
        }

        /// <summary>
        /// Initiates the teleport finish sound.
        /// </summary>
        public void PlayTeleportFinishSound()
        {
            levelGoal.FinishTeleport();
        }
    }
}
