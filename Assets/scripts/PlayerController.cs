using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    [RequireComponent(typeof(Controller2D))]
    public class PlayerController : MonoBehaviour
    {

        public float maxJumpHeight = 4;
        public float minJumpHeight = 1;
        public float timetoJumpApex = .4f;
        public float accelerationTimeAirborne = .2f;
        public float accelerationTimeGrounded = .1f;
        public float moveSpeed = 6;

        public Vector2 wallJump;
        //public Vector2 wallJumpOff;
        //public Vector2 wallLeap;

        private float wallSlideSpeedMax;
        public float wallStickTime = .15f;
        private float timeToWallUnstick;

        private Timer _timer;

        [HideInInspector]
        public float gravity;
        [HideInInspector]
        public float maxJumpVelocity;
        [HideInInspector]
        public float minJumpVelocity;
        Vector3 velocity;
        float velocityXSmoothing;

        Controller2D controller;
        SpriteRenderer spriterer;

        private void Awake()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timetoJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timetoJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
            wallSlideSpeedMax = -gravity / 5;
        }

        private void Start()
        {
            controller = GetComponent<Controller2D>();
            spriterer = GetComponent<SpriteRenderer>();

            _timer = new Timer(0.2f);

            print("Gravity: " + gravity + " VelocityJump: " + maxJumpVelocity + " wallSlideSpeed = " + wallSlideSpeedMax);
        }

        private void Update()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            int wallDirX = (controller.collisions.left) ? -1 : 1;

            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x,
                targetVelocityX,
                ref velocityXSmoothing,
                (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


            bool wallSliding = false;
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                wallSliding = true;
                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                    print("slowing down");
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (input.x != wallDirX && input.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }

            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (wallSliding)
                {

                    velocity.x = -wallDirX * wallJump.x;
                    velocity.y = wallJump.y;
                    /*
                    if (wallDirX == input.x) //towards the wall
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                        print("jump towards" + Time.time);
                    }
                    else if(input.x == 0) //no input
                    {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                        print("jump no input" + Time.time);
                    }
                    else //away from wall
                    {
                        velocity.x = -wallDirX * wallLeap.x;
                        velocity.y = wallLeap.y;
                        print("jump away" + Time.time);
                    }
                    */
                }
                if (controller.collisions.below)
                {
                    velocity.y = maxJumpVelocity;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            Animate();
        }

        public bool GetGrounded()
        {
            return controller.collisions.below;
        }

        void Animate()
        {
            int directionX = controller.collisions.faceDir; //check facing direction


            if (directionX == -1)
            {
                spriterer.flipX = true;
            }
            if (directionX == 1)
            {
                spriterer.flipX = false;
            }

        }

    }
}


