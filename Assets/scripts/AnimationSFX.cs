using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class AnimationSFX : MonoBehaviour
    {
        /// <summary>
        /// Plays a step sound.
        /// </summary>
        public void PlayStepSound()
        {
            SFXPlayer.Instance.Play(Sound.Step);
        }

        /// <summary>
        /// Plays a box recall sound.
        /// </summary>
        //public void PlayRecallSound()
        //{
        //    SFXPlayer.Instance.Play(Sound.TeleportFinish);
        //}
    }
}
