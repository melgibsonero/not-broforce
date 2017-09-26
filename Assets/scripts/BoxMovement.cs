using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce {
    public class BoxMovement : MonoBehaviour {

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

        private int mask = (1 << 8);

        private Vector2 _moveDirection;

        void Start() {
            GameObject.FindGameObjectWithTag("crate").GetComponent<BoxController>().addBox(this);
            _RB = gameObject.GetComponent<Rigidbody2D>();
            
            mask = ~mask;
        }

        // Update is called once per frame
        void Update() {
            Move();
        }

        public void AddFollowTarget (Transform target) {
            this._target = target;
        }

        private void Move() {
            if(_target != null) {
                if (Vector3.Distance(transform.position,
                _target.position) > _followDistance) {
                    Physics2D.queriesStartInColliders = false;
                    

                   
                    Vector3 direction = new Vector3 (_target.position.x - transform.position.x,0,0).normalized;
                    if(direction.x > 0) {
                        _moveDirection = Vector2.right;
                        
                    } else {
                        _moveDirection = Vector2.left;
                    }
                    int mask = (1 << 8);
                    mask = ~mask;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, _moveDirection, distanceX,mask);
                    if (hit.collider != null) {
                        
                        Jump();
                    }
                    transform.Translate(direction * _speed * Time.deltaTime);
                }
            }
        }

        private void Jump () {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceY,mask);
            bool Grounded = false;
            if (hit.collider != null) {
                Debug.Log("jumping");
                Grounded = true;
            }
            if (Grounded) {
                Debug.Log("jumping");
                _RB.AddForce(transform.up * _thrust, ForceMode2D.Impulse);
                
            }
        }

    }
}
