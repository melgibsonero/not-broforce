using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace not_broforce{
    public class ExhaustBox : MonoBehaviour
    {
        ParticleSystem particles;
        Box box;
        bool moving;

        // Use this for initialization
        void Start()
        {
            particles = GetComponent<ParticleSystem>();
            box = GetComponentInParent<Box>();
        }

        // Update is called once per frame
        void Update()
        {
            if (box.isMovingOnGround())
            {
                var emitter = particles.emission;
                emitter.enabled = true;
            }
            else
            {
                var emitter = particles.emission;
                emitter.enabled = false;
            }
            if (box.Controller.collisions.faceDir == -1)
            {
                transform.rotation = new Quaternion(0,-180,0,0);
            }
            else
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            if (box.DonePositionTaking)
            {
                var emitter = particles.emission;
                emitter.enabled = false;
            }
        }
    }
}
