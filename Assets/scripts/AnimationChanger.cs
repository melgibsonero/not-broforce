using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class AnimationChanger : MonoBehaviour
    {
        [SerializeField]
        private float minAnimChangeTime;

        [SerializeField]
        private float maxAnimChangeTime;

        [SerializeField]
        private List<string> animationNames;

        [SerializeField]
        private List<int> animationFrequencies;

        private string currentAnimName;

        private float startTime;
        private float targetTime;

        private System.Random random;

        private void Start()
        {
            currentAnimName = animationNames[0];

            int extraNames = animationNames.Count - animationFrequencies.Count;

            if (extraNames > 0)
            {
                for (int i = animationFrequencies.Count;
                     i < animationFrequencies.Count + extraNames; i++)
                {
                    animationFrequencies.Add(1);
                }
            }

            random = new System.Random();

            StartAnimIntermission();
        }

        private void Update()
        {
            if (Time.time - startTime >= targetTime)
            {
                PlayAnim();
                StartAnimIntermission();
            }
        }

        private void StartAnimIntermission()
        {
            startTime = Time.time;
            targetTime = startTime + minAnimChangeTime;

            int randInt = random.Next(0, 5);

            if (maxAnimChangeTime > minAnimChangeTime)
            {
                targetTime +=
                    ((maxAnimChangeTime - minAnimChangeTime) * (randInt / 5));
            }
        }

        private void PlayAnim()
        {
            //int sum = Utils.Sum(animationFrequencies);
            //int randInt = random.Next(1, sum);
            //for ()
            //{
            //    for ()
            //    {

            //    }
            //}



            //int randInt = random.Next(0, animationNames.Count - 1);

            //Animator.SetBool(currentAnimName, false);

            //currentAnimName = animationNames[randInt];

            //Animator.SetBool(currentAnimName, true);
        }
    }
}
