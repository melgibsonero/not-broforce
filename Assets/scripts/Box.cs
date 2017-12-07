using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
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

        public Vector3 _followTarget;

        private float gravity;
        private float maxJumpVelocity;
        private float minJumpVelocity;
        private Vector3 velocity;

        private Controller2D controller;

        private List<Vector2> followWaypoints;

        private PathFinding1 pathFinder;
        public float RepathTime = 1f;
        private float _repathTimer;

        protected bool pathNotFound;
        public Sprite box;
        public Sprite boxLit;
        private Vector2 gridCoordinates;
        BoxSelector selector;

        private bool sleeping = true;
        private float teleportWait;

        private bool teleportIn = false;
        private bool teleportOut = false;

        private SpriteRenderer spriteRend;

        private SpriteRenderer childSR;

        public Animator faceAnimator;

        private int moveModifier = 0;

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
        public Controller2D Controller { get { return controller; } }

        void Start() {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            controller = GetComponent<Controller2D>();
            followWaypoints = null;
            boxController = GameObject.FindGameObjectWithTag("BoxController").GetComponent<BoxController>();
            _takingPosition = false;
            _donePositionTaking = false;
            _repathTimer = RepathTime;

            gravity = player.gravity;
            maxJumpVelocity = player.maxJumpVelocity * 1.3f;
            minJumpVelocity = player.minJumpVelocity;
            pathFinder = GameObject.FindGameObjectWithTag("PathFinder").GetComponent<PathFinding1>();
            selector = FindObjectOfType<BoxSelector>();
            spriteRend = GetComponent<SpriteRenderer>();
            childSR = GetComponentInChildren<EmoteChanger>().gameObject.GetComponent<SpriteRenderer>();
            faceAnimator.SetBool("sleeping", true);
        }

        // Update is called once per frame
        void Update() {
            
            if(sleeping)
            {
                if (Vector3.Distance(transform.position,
                    player.transform.position) < _followDistance)
                {
                    sleeping = false;
                    boxController.addBox(this);
                    faceAnimator.SetBool("sleeping", false);
                }
            }
            else if (teleportWait <= 0)
            {
                if(_UnitToFollow != null)
                {
                    _target = new Vector3(_UnitToFollow.position.x, _UnitToFollow.position.y - 0.5f, 0);
                }
                if(_repathTimer > 0)
                {
                    _repathTimer -= Time.deltaTime;
                }
                if(controller.collisions.below && _repathTimer <= 0 && !_donePositionTaking && Vector3.Distance(transform.position,
                    _target) > _followDistance)
                {
                    _repathTimer = RepathTime;
                    if(!_takingPosition)
                    {
                        CheckModifier();
                    }
                    else
                    {
                        moveModifier = 0;
                    }
                    followWaypoints = pathFinder.FindPath(transform.position, new Vector3(_target.x + moveModifier, _target.y, _target.z));
                    if(followWaypoints != null)
                    {
                        faceAnimator.SetBool("confused", false);
                        pathNotFound = false;
                        if(followWaypoints.Count > 0)
                        {
                            _followTarget = followWaypoints[0];
                            CheckFollowDistance();
                        }
                    }
                    else if(followWaypoints == null)
                    {
                        faceAnimator.SetBool("confused", true);
                        if(_takingPosition)
                        {
                            selector.RemoveReservedBoxPlace(_target);
                            BackToLine();
                            boxController.addBox(this);
                        }
                    }
                }
                if(_takingPosition && Vector3.Distance(transform.position,
                    _target) < _followDistance && !_donePositionTaking)
                {
                    ChangeProperties();
                }
                else if (_donePositionTaking)
                {

                }
                else
                {
                    if(!_takingPosition && followWaypoints != null && Vector3.Distance(transform.position,
                    _target) > _followDistance)
                    {
                        if(followWaypoints.Count > 0)
                        {
                            _followTarget = followWaypoints[0];
                            CheckFollowDistance();
                        }
                    }
                    if(Vector3.Distance(transform.position,
                    _target) > _followDistance && followWaypoints == null && !pathNotFound)
                    {
                        if(!_takingPosition)
                        {
                            CheckModifier();
                        } else
                        {
                            moveModifier = 0;
                        }
                        followWaypoints = pathFinder.FindPath(transform.position, new Vector3(_target.x + moveModifier, _target.y, _target.z));
                        if(followWaypoints != null)
                        {
                            faceAnimator.SetBool("confused", false);
                            if(followWaypoints.Count > 0)
                            {
                                _followTarget = followWaypoints[0];
                                CheckFollowDistance();
                            }
                        }
                        else
                        {
                            faceAnimator.SetBool("confused", true);
                            pathNotFound = true;
                            if(_takingPosition)
                            {
                                selector.RemoveReservedBoxPlace(_target);
                                BackToLine();
                                boxController.addBox(this);
                            }
                        }

                    }
                    else if(Vector3.Distance(transform.position,
                    _followTarget) < _followDistance && followWaypoints != null)
                    {
                        if(followWaypoints.Count > 0)
                        {
                            followWaypoints.RemoveAt(0);
                        }
                        if(followWaypoints.Count <= 0)
                        {
                            followWaypoints = null;
                        }
                        else
                        {
                            _followTarget = followWaypoints[0];
                            CheckFollowDistance();
                        }
                    }
                    if(followWaypoints != null)
                    {
                        Move();
                    }
                    else
                    {
                        velocity.x = 0;
                    }

                    velocity.y += gravity * Time.deltaTime;
                    

                    //Onks tää varmasti pakollinen? Näyttää hölmöltä ku laatikot liitää
                    if(velocity.y < -5)
                    {
                        velocity.y = -5;
                    }
                    if(velocity.x > 0)
                    {
                        //animator.SetBool("moving", true);
                        //if(spriteRend.flipX)
                        //{
                        //    spriteRend.flipX = false;
                        //    childSR.flipX = false;
                        //}
                    } else if (velocity.x < 0)
                    {
                        //animator.SetBool("moving", true);
                        //if(!spriteRend.flipX)
                        //{
                        //    spriteRend.flipX = true;
                        //    childSR.flipX = true;
                        //}
                    } else
                    {
                        //animator.SetBool("moving", false);
                    }
                    controller.Move(velocity * Time.deltaTime);
                }
            } else if (teleportWait > 0)
            {
                teleportWait -= Time.deltaTime;
                if(teleportWait < 0.5f && teleportOut)
                {
                    teleportOut = false;
                    teleportIn = true;
                    transform.position = player.transform.position;
                    _followTarget = transform.position;
                } else if (teleportWait <= 0f && teleportIn)
                {
                    teleportIn = false;
                }
            }


        }

        public void AddFollowTarget (Transform target) {
            this._UnitToFollow = target;
        }

        private void CheckModifier()
        {
            moveModifier = player.Controller.collisions.faceDir * -1;
            if(!pathFinder.isGrounded(_target))
            {
                moveModifier = 0;
            } else if (!pathFinder.isGrounded(new Vector3(_target.x + moveModifier, _target.y, _target.z)))
            {
                moveModifier = 0;
            } else if (pathFinder.isGrounded(new Vector3(_target.x + moveModifier, _target.y + 1.5f, _target.z))) {
                moveModifier = 0;
            }
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
            transform.position = _target;
            if(!boxController.IsInStructure(this))
            {
                selector.RemoveReservedBoxPlace(_target);
                BackToLine();
                boxController.addBox(this);
                return;
            }
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1f, gameObject.GetComponent<BoxCollider2D>().size.y);
            gameObject.layer = LayerMask.NameToLayer("PlacedBoxes");
            _donePositionTaking = true;
            boxController.addPlacedBox(this);
            pathFinder.UpdateNode((int)transform.position.x, (int)transform.position.y, false);
            GetComponent<SpriteRenderer>().sprite = boxLit;
            
            selector.RefreshSelectedBox();
        }

        public void RemoveFollowTarget () {
            _followTarget = transform.position;
        }

        public bool TakePosition (Vector3 followTarget) {
            _target = followTarget;
            followWaypoints = pathFinder.FindPath(transform.position, _target);
            if(followWaypoints == null)
            {
                faceAnimator.Play("Confused");
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
            return pathFinder.FindBlockedPath(transform.position, targetPos, boxToRemove);
        }

        public void CheckFollowDistance ()
        {
            if(pathFinder.isGrounded(_followTarget) || pathFinder.isGrounded(new Vector3(_followTarget.x,_followTarget.y - 1, _followTarget.z)))
            {
                _followDistance = 0.5f;
            }
            else
            {
                _followDistance = 1.1f;
            }
        }

        public void TeleportToPlayer ()
        {
            if(_donePositionTaking)
            {
                BackToLine();
            }
            else if(_takingPosition)
            {
                selector.RemoveReservedBoxPlace(_target);
                BackToLine();
            }
            else
            {
                followWaypoints = null;
            }
            teleportWait = 1f;
            velocity.x = 0;
            velocity.y = 0;
            boxController.addBox(this);
            GetComponent<Animator>().Play("TeleportVanish");
            teleportOut = true;
        }
        
        public bool isMovingOnGround()
        {
            bool isMoving = false;

            if (velocity.x != 0 && controller.collisions.below)
            {
                isMoving = true;
            }

            return isMoving;
        }
        
    }
}
