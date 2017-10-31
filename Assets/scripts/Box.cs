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

        private bool _takingPosition;

        private bool _donePositionTaking;

        private BoxController boxController;

        private Vector3 _followTarget;

        private float gravity;
        private float maxJumpVelocity;
        private float minJumpVelocity;
        private Vector3 velocity;

        private Controller2D controller;

        private List<Vector2> followWaypoints;

        private PathFinding1 pathFinder;



        void Start() {
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            controller = GetComponent<Controller2D>();
            followWaypoints = null;
            //mask = LayerMask.GetMask("Environment", "PlacedBoxes");
            boxController = GameObject.FindGameObjectWithTag("BoxController").GetComponent<BoxController>();
            boxController.addBox(this);
            //_RB = gameObject.GetComponent<Rigidbody2D>();

            //mask = ~mask;
            _takingPosition = false;
            canMove = true;
            _donePositionTaking = false;
          
            gravity = player.gravity;
            maxJumpVelocity = player.maxJumpVelocity * 1.3f;
            minJumpVelocity = player.minJumpVelocity;
            pathFinder = GameObject.FindGameObjectWithTag("PathFinder").GetComponent<PathFinding1>();
        }

        // Update is called once per frame
        void Update() {
            if(Input.GetKeyDown(KeyCode.J)) {
                Jump();
            }
            if(_takingPosition && Vector3.Distance(transform.position,
                _followTarget) < _followDistance && !_donePositionTaking) {
                ChangeProperties();
            } else  if (_donePositionTaking){
                //Do something if structure is broken
            } else  {
                if(!_takingPosition && followWaypoints != null) {
                    _followTarget = followWaypoints[0];
                }
                if(Vector3.Distance(transform.position,
                _target.position) > _followDistance && followWaypoints == null)
                {
                    followWaypoints = pathFinder.FindPath(transform.position, _target.position);
                    Debug.Log("finding way");
                    if(followWaypoints != null)
                    {
                        _followTarget = followWaypoints[0];
                    }
                    
                } else if (Vector3.Distance(transform.position,
                _followTarget) < _followDistance && followWaypoints != null)
                {
                    followWaypoints.RemoveAt(0);
                    if(followWaypoints.Count <= 0)
                    {
                        followWaypoints = null;
                    } else
                    {
                        _followTarget = followWaypoints[0];
                    }
                }
                if(followWaypoints != null)
                {
                    Debug.Log("moving");
                    Move();
                } else
                {
                    velocity.x = 0;
                }
                velocity.y += gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }
        }

        public void AddFollowTarget (Transform target) {
            this._target = target;
        }

        private void Move()
        {
            if(followWaypoints != null)
            {
                if(Mathf.Abs(_followTarget.x - transform.position.x) > _followDistanceX)
                {
                    Physics2D.queriesStartInColliders = false;
                    float direction = Mathf.Sign(_followTarget.x - transform.position.x);
                    //if(direction > 0)
                    //{
                    //    _moveDirection = Vector2.right;

                    //}
                    //else
                    //{
                    //    _moveDirection = Vector2.left;
                    //}
                    //RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, distanceX, mask);

                    canMove = true;
                    if((controller.collisions.above || controller.collisions.below))
                    {
                        velocity.y = 0;
                    }
                    if(controller.collisions.left || controller.collisions.right)
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

               
                if(Mathf.Abs(_followTarget.x - transform.position.x) < _followDistanceX && (_followTarget.y - transform.position.y) > _followDistance)
                {
                    Jump();
                }
            }
            
            
           
            
         }

        private void Jump () {
            Physics2D.queriesStartInColliders = false;
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceY,mask);
            if (controller.collisions.below ) {
                velocity.y = maxJumpVelocity;           
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
            gameObject.layer = LayerMask.NameToLayer("MovingBoxes");
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1f, gameObject.GetComponent<BoxCollider2D>().size.y);
        }

    }
}
