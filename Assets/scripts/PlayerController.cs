﻿using System.Collections;
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
        public float jumpTimer = 0.2f;

        public Vector2 wallJump;
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;

        float _jumpTimer;
        bool jumpTimerActive;
        float extraGravity = 0;
        int wallDirX;
        int faceDirOld;
        bool wallSliding;
        bool earlyWalljump;

        private float wallSlideSpeedMax;
        public float wallStickTime = .4f;
        private float timeToWallUnstick;

        [HideInInspector]
        public float gravity;
        [HideInInspector]
        public float maxJumpVelocity;
        [HideInInspector]
        public float minJumpVelocity;
        Vector3 velocity;
        float velocityXSmoothing;

        Vector2 _directionalInput;

        Controller2D _controller;
        //SpriteRenderer _spriterer;
        Animator _animation;

        private void Awake()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timetoJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timetoJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
            wallSlideSpeedMax = -gravity / 5;
        }

        private void Start()
        {
            _controller = GetComponent<Controller2D>();
            _animation = GetComponent<Animator>();

            _jumpTimer = jumpTimer;
        }

        private void Update()
        {
            if (jumpTimerActive)
            {
                _jumpTimer -= Time.deltaTime;
                if (_jumpTimer < 0)
                {
                    jumpTimerActive = false;
                    _jumpTimer = jumpTimer;
                }
            }

            CalculateVelocity();
            HandleWallSliding();

            _controller.Move(velocity * Time.deltaTime);
            if (!_controller.collisions.below)
            {
                extraGravity += Time.deltaTime*1.7f;
            }
            if (_controller.collisions.above || _controller.collisions.below)
            {
                velocity.y = 0;
                faceDirOld = 0;
                extraGravity = 0;
            }
            Animate();
        }

        public void SetDirectionalInput(Vector2 input)
        {
            if (!jumpTimerActive)
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
                if (wallSliding || earlyWalljump)
                {
                faceDirOld = wallDirX;
                extraGravity = 0;
                    if (_directionalInput.x == 0) //neutral
                    {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    if (wallDirX == _directionalInput.x) //towards the wall
                    {
                        jumpTimerActive = true;
                    velocity.x = -wallDirX * wallJump.x;
                    velocity.y = wallJump.y;
                    _directionalInput = Vector2.zero;
                    }
                    if (-wallDirX == _directionalInput.x) //away from the wall
                    {
                    faceDirOld = wallDirX;
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                    }
            }
                if (_controller.collisions.below)
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
            return _controller.collisions.below;
        }

        void Animate() //Every reference to Animator should be here
        {
            int directionX = _controller.collisions.faceDir; //check facing direction

            if (directionX == -1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (directionX == 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            //If horizontal movement is not zero.
            if (_directionalInput != Vector2.zero)
            {
                _animation.SetBool("moving", true);
            }
            if(_directionalInput == Vector2.zero)
            {
                _animation.SetBool("moving", false);
            }
            
            //Till we get an animation for it. I can't figure out that shit
            //If Vertical movement is not zero.
            if (!_animation.GetBool("jumping") && velocity.y > 0)
            {
                _animation.SetBool("jumping", true);
            }
            if (_controller.collisions.below)
            {
                _animation.SetBool("jumping", false);
            }

            if (!_animation.GetBool("wallsliding") && wallSliding)
            {
                _animation.SetBool("wallsliding", true);
            }
            if(!wallSliding)
            {
                _animation.SetBool("wallsliding", false);
            }
            /*if (wallJumping)
            {
                _animation.SetBool("walljumping", true);
            }
            else
            {
                _animation.SetBool("walljumping", false);
            }
            */
            
        }
        private void HandleWallSliding()
        {
            wallDirX = (_controller.collisions.wjLeft) ? -1 : 1;
            wallSliding = false;
            earlyWalljump = false;
            if ((_controller.collisions.left || _controller.collisions.right) && !_controller.collisions.below/* && velocity.y < 0 */&& CompareWallDir(wallDirX))
            {
                wallSliding = true;
                /*if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }
                */

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
            if((_controller.collisions.wjLeft ||_controller.collisions.wjRight) && !_controller.collisions.below && velocity.y < 0 && CompareWallDir(wallDirX))
            {
                earlyWalljump = true;
            }
        }

        private void CalculateVelocity()
        {
            float targetVelocityX = _directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			if (wallSliding && velocity.y < 0) {
				velocity.y += gravity / 3 * Time.deltaTime;
			} else {
				velocity.y += gravity * Time.deltaTime + gravity * extraGravity * Time.deltaTime;
			}
        }
    }
    
}


