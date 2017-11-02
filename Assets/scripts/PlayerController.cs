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
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;

        int wallDirX;
        int faceDirOld;
        bool wallSliding;

        private float wallSlideSpeedMax;
        public float wallStickTime = .4f;
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

        Vector2 _directionalInput;

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

            _timer = new Timer(0.35f);            
        }

        private void Update()
        {
            _timer.Check(false);
         
            CalculateVelocity();
            HandleWallSliding();

            controller.Move(velocity * Time.deltaTime);

            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
                _timer.Stop();
                faceDirOld = 0;
            }
            Animate();
        }

        public void SetDirectionalInput(Vector2 input)
        {
            if (_timer.Active)
            {
                _directionalInput = input*Time.deltaTime;
            }
            else
            {
                _directionalInput = input;
            }         
        }
        private bool CompareWallDir(int WallDir)
        {
            if(faceDirOld == 0)
            {
                return true;
            }
            if(WallDir == faceDirOld)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public void OnJumpInputDown()
        {
                if (wallSliding)
                {
                faceDirOld = wallDirX;                  
                    if (_directionalInput.x == 0) //
                    {
                        _timer.Start();
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    print("no input");
                    }
                    if (wallDirX == _directionalInput.x) //towards the wall
                    {
                        _timer.Start();
                        velocity.x = -wallDirX * wallJump.x;
                        velocity.y = wallJump.y;
                        _directionalInput = Vector2.zero;
                    print("towards");
                    }
                    if (-wallDirX == _directionalInput.x) //away from the wall
                    {
                    faceDirOld = wallDirX;
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                    print("leap");
                    }
            }
                if (controller.collisions.below)
                {
                    velocity.y = maxJumpVelocity;
                }
        }
        public void OnJumpInputUp()
        {
            if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
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
        private void HandleWallSliding()
        {
            wallDirX = (controller.collisions.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && CompareWallDir(wallDirX))
            {
                wallSliding = true;
                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (_directionalInput.x != wallDirX && _directionalInput.x != 0)
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
        }

        private void CalculateVelocity()
        {
            float targetVelocityX = _directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }
    }
    
}


