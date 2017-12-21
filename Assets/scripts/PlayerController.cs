using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce{
    [RequireComponent(typeof(Controller2D))]
    public class PlayerController : MonoBehaviour
    {

        public GameObject SmalllandingDustCloud;
        public GameObject MediumlandingDustCloud;
        public GameObject LargelandingDustCloud;
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

        Vector3 characterLocation;
        float _jumpTimer;
        bool jumpTimerActive;
        float extraGravity = 0;
        int wallDirX;
        int faceDirOld;
        bool wallSliding;
        bool earlyWalljump;
        bool groundedStateOld;
        float velocityYold;

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

        [HideInInspector]
        public Vector2 _directionalInput;

        Controller2D _controller;
        //SpriteRenderer _spriterer;
        Animator _animation;
        ParticleSystem[] _childParticleSystems;


        public Controller2D Controller { get { return _controller; } }

        private void Awake()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timetoJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timetoJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        }

        private void Start()
        {
            _controller = GetComponent<Controller2D>();
            _animation = GetComponent<Animator>();
            _childParticleSystems = GetComponentsInChildren<ParticleSystem>();

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
            characterLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
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
                SFXPlayer.Instance.Play(Sound.Jump2);
                faceDirOld = wallDirX;
                extraGravity = 0;
                    if (_directionalInput.x == 0) //neutral
                    {

                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                        _animation.Play("Jump");
                    }
                    if (wallDirX == _directionalInput.x) //towards the wall
                    {
                    _animation.Play("Jump");
                    jumpTimerActive = true;
                    velocity.x = -wallDirX * wallJump.x;
                    velocity.y = wallJump.y;
                    _directionalInput = Vector2.zero;
                    }                   
                }
                if (_controller.collisions.below)
                {
                    _animation.Play("Jump");
                    SFXPlayer.Instance.Play(Sound.Jump1);
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
            var _walkParticles = _childParticleSystems[0].emission;
            float directionX;
            if (_directionalInput != Vector2.zero)
            {
                directionX = Mathf.Sign(_directionalInput.x); //check facing direction
            }
            else
            {
                directionX = 0;
            }
            if (!jumpTimerActive)
            {
                if (directionX == -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                if (directionX == 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
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
            
            if(_directionalInput != Vector2.zero && _controller.collisions.below)
            {
                _walkParticles.enabled = true;
            }
            else
            {
                _walkParticles.enabled = false;
            }

            //Till we get an animation for it. I can't figure out that shit
            //Checks if player is grounded
            if (_controller.collisions.below)
            {
                _animation.SetBool("grounded", true);
            }
            else
            {
                _animation.SetBool("grounded", false);
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
            if (_controller.collisions.below)
            {
                if(!GameManager.Instance.MenuExited &&
                   groundedStateOld != _controller.collisions.below)
                {
                    var landingCloud = SmalllandingDustCloud;
                    if (velocityYold < -8.5f)
                    {
                        landingCloud = MediumlandingDustCloud;
                    }
                    if(velocityYold < -16f)
                    {
                        landingCloud = LargelandingDustCloud;
                    }
                    GameObject dustPuff = Instantiate(landingCloud);
                    SFXPlayer.Instance.Play(Sound.Knock);
                    dustPuff.transform.position = new Vector3(characterLocation.x, characterLocation.y, characterLocation.z+0.4f);
                    //Debug.Log(velocityYold);
                }
            }
            groundedStateOld = _controller.collisions.below;
            velocityYold = velocity.y;

        }
        private void HandleWallSliding()
        {
            wallDirX = (_controller.collisions.wjLeft) ? -1 : 1;
            wallSliding = false;
            earlyWalljump = false;
            if ((_controller.collisions.left || _controller.collisions.right) && !_controller.collisions.below && velocity.y <= 0 && CompareWallDir(wallDirX))
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
            if((_controller.collisions.wjLeft ||_controller.collisions.wjRight) && !_controller.collisions.below && CompareWallDir(wallDirX))
            {
                earlyWalljump = true;
            }
            if ((wallSliding || earlyWalljump) && (-wallDirX == _directionalInput.x) && timeToWallUnstick <= 0) //away from the wall
            {
                SFXPlayer.Instance.Play(Sound.Jump2);
                extraGravity = extraGravity/7;
                _animation.Play("Jump");
                faceDirOld = wallDirX;
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        private void CalculateVelocity()
        {
            float targetVelocityX = _directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			if (wallSliding && velocity.y < 0) {
				velocity.y += gravity * 0.25f * Time.deltaTime;
			} else {
				velocity.y += gravity * Time.deltaTime + gravity * extraGravity * Time.deltaTime;
			}
        }
    }
    
}


