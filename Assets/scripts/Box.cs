using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce {
    public class Box : MonoBehaviour, IGridObject {

        [SerializeField]
        private Transform _UnitToFollow;


        [SerializeField]
        private Vector3 _target;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _followDistance;

        [SerializeField]
        private float _followDistanceX;

        private bool _takingPosition;

        private bool _donePositionTaking;

        private BoxController boxController;
        private PlayerController player;

        private Vector3 _followTarget;

        private float gravity;
        private float maxJumpVelocity;
        private float minJumpVelocity;
        private Vector3 velocity;

        private Controller2D controller;

        private List<Vector2> followWaypoints;

        private PathFinding1 pathFinder;
        public float RepathTime = 1f;
        private float _repathTimer;

        public Sprite box;
        public Sprite boxLit;
        private Vector2 gridCoordinates;

        public Vector2 GridCoordinates
        {
            get
            {
                return gridCoordinates;
            }

            set
            {
                gridCoordinates = value;
            }
        }

        void Start() {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            controller = GetComponent<Controller2D>();
            followWaypoints = null;
            boxController = GameObject.FindGameObjectWithTag("BoxController").GetComponent<BoxController>();
            boxController.addBox(this);
            _takingPosition = false;
            _donePositionTaking = false;

            _repathTimer = RepathTime;

            gravity = player.gravity;
            maxJumpVelocity = player.maxJumpVelocity * 1.3f;
            minJumpVelocity = player.minJumpVelocity;
            pathFinder = GameObject.FindGameObjectWithTag("PathFinder").GetComponent<PathFinding1>();
        }

        // Update is called once per frame
        void Update() {
            
            if(_UnitToFollow != null)
            {
                _target = new Vector3(_UnitToFollow.position.x, _UnitToFollow.position.y,0);
            }
            if(_repathTimer > 0)
            {
                _repathTimer -= Time.deltaTime;            
            } 

            if(controller.collisions.below && _repathTimer <= 0 && !_donePositionTaking && Vector3.Distance(transform.position,
                _target) > _followDistance)
            {
                Debug.Log("Repath");
                _repathTimer = RepathTime;
                followWaypoints = pathFinder.FindPath(transform.position, _target);
                if(followWaypoints != null)
                {
                    if (followWaypoints.Count > 0)
                    {
                        _followTarget = followWaypoints[0];
                    }
                }
            }

            if(_takingPosition && Vector3.Distance(transform.position,
                _target) < _followDistance && !_donePositionTaking) {
                ChangeProperties();
            } else  if (_donePositionTaking){
                //Do something if structure is broken
            }
            else{
                if(!_takingPosition && followWaypoints != null && Vector3.Distance(transform.position,
                _target) > _followDistance) {
                    if(followWaypoints.Count > 0)
                    {
                        _followTarget = followWaypoints[0];
                    }
                }
                if(Vector3.Distance(transform.position,
                _target) > _followDistance && followWaypoints == null)
                {
                    followWaypoints = pathFinder.FindPath(transform.position, _target);
                    if(followWaypoints != null)
                    {
                        if(followWaypoints.Count > 0)
                        {
                            _followTarget = followWaypoints[0];
                        }
                    }
                    
                } else if (Vector3.Distance(transform.position,
                _followTarget) < _followDistance && followWaypoints != null)
                {
                    if(followWaypoints.Count > 0)
                    {
                        followWaypoints.RemoveAt(0);
                    }
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
                    Move();
                } else
                {
                    velocity.x = 0;
                }
                
                velocity.y += gravity * Time.deltaTime;
                if(velocity.y < -5)
                {
                    velocity.y = -5;
                }
                controller.Move(velocity * Time.deltaTime);
            }
            
        }

        public void AddFollowTarget (Transform target) {
            this._UnitToFollow = target;
        }

        private void Move()
        {
            if(followWaypoints != null)
            {
                if(Mathf.Abs(_followTarget.x - transform.position.x) > _followDistanceX)
                {
                    Physics2D.queriesStartInColliders = false;
                    float direction = Mathf.Sign(_followTarget.x - transform.position.x);
                    if((controller.collisions.above || controller.collisions.below))
                    {
                        velocity.y = 0;
                    }
                    if(direction > 0)
                    {
                        if(controller.collisions.right)
                        {
                            Jump();
                        }
                    }
                    else if(direction > 0)
                    {
                        if(controller.collisions.left)
                        {
                            Jump();
                        }
                    }
                    else if(controller.collisions.left || controller.collisions.right)
                    {

                        Jump();
                    }
                    velocity.x = (direction * _speed);
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
            if (controller.collisions.below ) {
                velocity.y = maxJumpVelocity;           
            }
        }

        private void ChangeProperties () {
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1f, gameObject.GetComponent<BoxCollider2D>().size.y);
            transform.position = _target;
            gameObject.layer = LayerMask.NameToLayer("PlacedBoxes");
            _donePositionTaking = true;
            boxController.addPlacedBox(this);
            pathFinder.UpdateNode((int)transform.position.x, (int)transform.position.y, false);
            GetComponent<SpriteRenderer>().sprite = boxLit;
        }

        public void RemoveFollowTarget () {
            _followTarget = transform.position;
        }

        public bool TakePosition (Vector3 followTarget) {
            _target = followTarget;
            followWaypoints = pathFinder.FindPath(transform.position, _target);
            if(followWaypoints == null)
            {
                return false;
            }

            GetComponentInChildren<EmoteChanger>().changeEmote("Sad");
            _UnitToFollow = null;
            gridCoordinates = LevelController.GetGridCoordinates(followTarget);
            followWaypoints = null;
            _takingPosition = true;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, gameObject.GetComponent<BoxCollider2D>().size.y);
            return true;
        }

        public void BackToLine () {
            GetComponentInChildren<EmoteChanger>().changeEmote("Smile");
            Debug.Log("WORK WROK");
            pathFinder.UpdateNode((int)transform.position.x, (int)transform.position.y, true);
            _takingPosition = false;
            _donePositionTaking = false;
            followWaypoints = null;
            gameObject.layer = LayerMask.NameToLayer("MovingBoxes");
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.7f, gameObject.GetComponent<BoxCollider2D>().size.y);

            GetComponent<SpriteRenderer>().sprite = box;
        }

        public void MoveToGridCoordinates()
        {
            return;
        }

        public bool FindPathInStructure (Vector3 targetPos, Box boxToRemove)
        {
            Debug.Log(pathFinder.FindBlockedPath(transform.position, targetPos, boxToRemove));
            //return pathFinder.FindBlockedPath(transform.position, targetPos, boxToRemove);
            return true;
        }
        
    }
}
