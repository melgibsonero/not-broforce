﻿using System.Collections;
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

        private Rigidbody2D _RB;

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

        void Start() {
            mask = LayerMask.GetMask("Environment", "PlacedBoxes");
            _jumpTimer = new Timer(1);
            boxController = GameObject.FindGameObjectWithTag("BoxController").GetComponent<BoxController>();
            boxController.addBox(this);
            _RB = gameObject.GetComponent<Rigidbody2D>();

            //mask = ~mask;
            _takingPosition = false;
            canMove = true;
            _donePositionTaking = false;
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

        private void Move() {
            if(Input.GetKeyDown(KeyCode.M)) {
                _takingPosition = true;
                _followDistance = 0.4f;
            }
            if(_target != null) {
                if (Vector3.Distance(transform.position,
                _followTarget) > _followDistance) {
                    Physics2D.queriesStartInColliders = false;
                    

                   
                    Vector3 direction = new Vector3 (_followTarget.x - transform.position.x,0,0).normalized;
                    if(direction.x > 0) {
                        _moveDirection = Vector2.right;
                        
                    } else {
                        _moveDirection = Vector2.left;
                    }
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, distanceX,mask);

                    canMove = true;

                    if (hit.collider != null) {
                        
                        Jump();
                        canMove = false;
                    }

                    if (canMove)
                    {
                        transform.Translate(direction * _speed * Time.deltaTime);
                    }
                }
            }
        }

        private void Jump () {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceY,mask);
            bool Grounded = false;
            if (hit.collider != null && _jumpTimer.TimeLeft() <= 0) {
                Grounded = true;
            }
            if (Grounded) {
                Debug.Log("jumping");
                _RB.AddForce(transform.up * _thrust, ForceMode2D.Impulse);
                _jumpTimer.Start();
                
            }
        }

        private void ChangeProperties () {
            transform.position = _followTarget;
            gameObject.layer = LayerMask.NameToLayer("PlacedBoxes");
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _donePositionTaking = true;
            boxController.addPlacedBox(this);
        }

        public void RemoveFollowTarget () {
            _followTarget = transform.position;
        }

        public void TakePosition (Vector3 followTarget) {
            _followTarget = followTarget;
            _takingPosition = true;
            _followDistance = 0.6f;
        }

        public void BackToLine () {
            _takingPosition = false;
            _followDistance = 0.7f;
            _donePositionTaking = false;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.layer = LayerMask.NameToLayer("MovingBoxes");
        }

    }
}