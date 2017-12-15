using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce{
    public class CameraFollow : MonoBehaviour
    {
        public GameObject target;
        private Controller2D controller;
        private PlayerInput playerInput;
        
        public float verticalOffset;
        public float lookAheadDistanceX;
        public float lookAheadDistanceY;
        public float lookSmoothTimeX;
        public float verticalSmoothTime;
        public Vector2 focusAreaSize;
        
        FocusArea focusArea;

        float currentLookAheadX;
        float targetLookAheadX;
        float lookAheadDirectionX;
        float smoothLookVelocityX;
        float smoothVelocityY;

        bool lookAheadStopped;

        private void Start()
        {
            controller = target.GetComponent<Controller2D>();
            playerInput = target.GetComponent<PlayerInput>();

            focusArea = new FocusArea(controller.bc.bounds, focusAreaSize);
        }
        private void LateUpdate()
        {
            focusArea.Update(controller.bc.bounds);

            Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

            if(focusArea.velocity.x != 0)
            {
                lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
                if(Mathf.Sign(playerInput.DirectionalInput.x) == Mathf.Sign(focusArea.velocity.x) && playerInput.DirectionalInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4;
                    }
                }
            }
            


            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
            if (playerInput.CheckSInput())
            {
                focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y - lookAheadDistanceY, ref smoothVelocityY, verticalSmoothTime);
            }
            else
            {
                focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            }
            
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(focusArea.centre, focusAreaSize);
        }

        struct FocusArea
        {
            public Vector2 centre;
            public Vector2 velocity;
            float left, right;
            float top, bottom;

            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;

                velocity = Vector2.zero;
                centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if(targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }else if(targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }
                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom){
                    shiftY = targetBounds.min.y - bottom;
                }else if (targetBounds.max.y > top){
                    shiftY = targetBounds.max.y - top;
                }
                //if (playerInput.CheckSInput()&&!(targetBounds.max.y > top))
                //{
                //    shiftY -= 0.025f;
                //}

                top += shiftY;
                bottom += shiftY;
                centre = new Vector2((left + right) / 2, (top + bottom) / 2);

                velocity = new Vector2(shiftX, shiftY);
            }
        }
    }
}
