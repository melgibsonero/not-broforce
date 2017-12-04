using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace not_broforce{
    public class ExhaustBox : MonoBehaviour
    {
        ParticleSystem particles;
        public bool moving;

        // Use this for initialization
        void Start()
        {
            particles = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (moving)
            {
                //particles.emission.enabled;

            }
        }
    }
}
