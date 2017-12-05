using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    [Serializable]
    public class NameAndNum
    {
        public string name;
        public int number;
    }

    public class AnimationChanger : MonoBehaviour
    {
        [SerializeField]
        private float minDurationBetweenAnims;

        [SerializeField]
        private float maxDurationBetweenAnims;

        [SerializeField]
        private List<NameAndNum> animations;

        //[SerializeField, Range(0, 5)]
        //int randomNumMod;

        private Animator animator;

        private string currentAnimName;

        private float startTime;
        private float waitTime;

        private bool active;
        private bool justActivated;

        private System.Random random;

        int freqSum;

        // Testing
        [SerializeField]
        private float wait;

        private float RandomFloat(int min, int max)
        {
            //if (randomNumMod >= 0 && randomNumMod <= 1)
            //{
            //    return random.Next(min, min + (max - min) / 2);
            //}
            //else if (randomNumMod > 1 && randomNumMod <= 3)
            //{
            //    return random.Next(min + (max - min) / 2, max);
            //}
            //else if(randomNumMod > 3 && randomNumMod <= 5)
            //{
            //    return random.Next(min, max);
            //}

            return random.Next(min, max);
        }

        private void Start()
        {
            animator = GetComponent<Animator>();

            currentAnimName = animations[0].name;

            random = new System.Random();

            int[] animationFrequencies = new int[animations.Count];

            for (int i = 0; i < animations.Count; i++)
            {
                animationFrequencies[i] = animations[i].number;
            }

            freqSum = Utils.Sum(animationFrequencies);
        }

        private void Update()
        {
            UpdateActive();

            if (active)
            {
                wait = waitTime - (Time.time - startTime);

                if ((Time.time - startTime) >= waitTime)
                {
                    if (!justActivated)
                    {
                        PlayAnim();
                    }

                    StartDelayBetweenAnims();
                }
            }
        }

        private void UpdateActive()
        {
            bool sleeping = animator.GetBool("sleeping");
            bool confused = animator.GetBool("confused");

            if (!sleeping && !confused)
            {
                if (!active)
                {
                    active = true;
                    justActivated = true;
                }
                else if (justActivated)
                {
                    justActivated = false;
                }
            }
            else if (active)
            {
                active = false;
                animator.Play("Idle");
            }
        }

        private void StartDelayBetweenAnims()
        {
            startTime = Time.time;
            waitTime = minDurationBetweenAnims;

            float randFloat = RandomFloat(0, 5);
            //float randFloat = random.Next(0, 5);

            if (maxDurationBetweenAnims > minDurationBetweenAnims)
            {
                waitTime +=
                    ((maxDurationBetweenAnims - minDurationBetweenAnims)
                    * (randFloat / 5f));
            }

            //if (targetTime - startTime > maxDurationBetweenAnims)
            //{
            //    targetTime = startTime + maxDurationBetweenAnims;
            //}
        }

        private void PlayAnim()
        {
            //animator.SetBool(currentAnimName, false);
            //Debug.Log("Animation ends: " + currentAnimName);

            currentAnimName = GetRandomAnimName();

            animator.Play(currentAnimName);
            //animator.SetBool(currentAnimName, true);
            //animator.SetBool(currentAnimName, false);
            Debug.Log("Animation plays: " + currentAnimName);
        }

        private string GetRandomAnimName()
        {
            int randAnim = (int) RandomFloat(0, freqSum) + 1;
            //int randAnim = random.Next(0, freqSum) + 1;
            //Debug.Log("Random number: " + randAnim);

            for (int i = 0; i < animations.Count; i++)
            {
                randAnim -= animations[i].number;

                if (randAnim <= 0)
                {
                    return animations[i].name;
                }
            }

            Debug.LogError("Invalid random animation number");
            return animations[0].name;
        }
    }
}
