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
            int random = Random.Range(0, 3);

            switch (random)
            {
                case 0:
                {
                    SFXPlayer.Instance.Play(Sound.Step1);
                    break;
                }
                case 1:
                {
                    SFXPlayer.Instance.Play(Sound.Step2);
                    break;
                }
                case 2:
                {
                    SFXPlayer.Instance.Play(Sound.Step3);
                    break;
                }
                case 3:
                {
                    SFXPlayer.Instance.Play(Sound.Step4);
                    break;
                }
            }
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
