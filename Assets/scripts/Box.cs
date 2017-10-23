using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce {
    public class Box : MonoBehaviour {

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _followDistance;

        [SerializeField]
        private float _followDistanceX;

        //private Rigidbody2D _RB;

        [SerializeField]
        private float distanceX;

        [SerializeField]
        private float distanceY;

        [SerializeField]
        private float _thrust;

        private int mask;

        private Vector2 _moveDirection;

        private bool canMove;

        private Timer _jumpTimer;

        private bool _takingPosition;

        private bool _donePositionTaking;

        private BoxController boxController;

        private Vector3 _followTarget;

        private float gravity;
        private float maxJumpVelocity;
        private float minJumpVelocity;
        private Vector3 velocity;

        private Controller2D controller;



        void Start() {
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            controller = GetComponent<Controller2D>();
            
            //mask = LayerMask.GetMask("Environment", "PlacedBoxes");
            _jumpTimer = new Timer(1);
            boxController = GameObject.FindGameObjectWithTag("BoxController").GetComponent<BoxController>();
            boxController.addBox(this);
            //_RB = gameObject.GetComponent<Rigidbody2D>();

            //mask = ~mask;
            _takingPosition = false;
            canMove = true;
            _donePositionTaking = false;
            _followDistanceX = 0.7f;
            _followDistance = 0.7f;
          
            gravity = player.gravity;
            maxJumpVelocity = player.maxJumpVelocity;
            minJumpVelocity = player.minJumpVelocity;
        }

        // Update is called once per frame
        void Update() {
            _jumpTimer.Update();
            if(_takingPosition && Vector3.Distance(transform.position,
                _followTarget) < _followDistance && !_donePositionTaking) {
                ChangeProperties();
            } else  if (_donePositionTaking){
                //Do something if structure is broken
            } else  {
                if (!_takingPosition) {
                    _followTarget = _target.position;
                }
                Move();
            }
        }

        public void AddFollowTarget (Transform target) {
            this._target = target;
        }

        private void Move()
        {
            if(_target != null)
            {
                if(Mathf.Abs(_followTarget.x - transform.position.x) > _followDistanceX)
                {
                    Physics2D.queriesStartInColliders = false;
                    float direction = Mathf.Sign(_followTarget.x - transform.position.x);
                    if(direction > 0)
                    {
                        _moveDirection = Vector2.right;

                    }
                    else
                    {
                        _moveDirection = Vector2.left;
                    }
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, distanceX, mask);

                    canMove = true;

                    if(hit.collider != null)
                    {
                        Jump();
                        canMove = false;
                    }

                    if(canMove)
                    {
                        velocity.x = (direction * _speed);
                    }
                }
                else
                {
                    velocity.x = 0;
                }

                if (controller.collisions.above || controller.collisions.below)
                {
                    velocity.y = 0;
                }

                if ((_followTarget.y - transform.position.y) > _followDistanceX)
                {
                    Jump();
                }                
                velocity.y += gravity * Time.deltaTime;               
                controller.Move(velocity * Time.deltaTime);
            }
         }

        private void Jump () {
            Physics2D.queriesStartInColliders = false;
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceY,mask);
            bool Grounded = false;
            if (controller.collisions.below ) {
                Grounded = true;
            }
            if (Grounded) {
                Debug.Log("jumping");
                velocity.y = maxJumpVelocity;
                //_jumpTimer.Start();                
            }
        }

        private void ChangeProperties () {
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1f, gameObject.GetComponent<BoxCollider2D>().size.y);
            transform.position = _followTarget;
            gameObject.layer = LayerMask.NameToLayer("PlacedBoxes");
            //gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _donePositionTaking = true;
            boxController.addPlacedBox(this);
        }

        public void RemoveFollowTarget () {
            _followTarget = transform.position;
        }

        public void TakePosition (Vector3 followTarget) {
            _followTarget = followTarget;
            _takingPosition = true;
            _followDistance = 0.3f;
            _followDistanceX = 0.03f;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.8f, gameObject.GetComponent<BoxCollider2D>().size.y);
        }

        public void BackToLine () {
            _takingPosition = false;
            _followDistanceX = 0.7f;
            _donePositionTaking = false;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.layer = LayerMask.NameToLayer("MovingBoxes");
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1f, gameObject.GetComponent<BoxCollider2D>().size.y);
        }

    }
}
