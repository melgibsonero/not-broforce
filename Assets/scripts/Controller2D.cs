using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController
{
    float maxSlopeAngle = 60;

    public CollisionInfo collisions;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    public void Move(Vector2 _moveAmount, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions._moveAmountOld = _moveAmount;

        if(_moveAmount.y < 0)
        {
            DescendSlope(ref _moveAmount);
        }

        if(_moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(_moveAmount.x);
        }

        HorizontalCollisions(ref _moveAmount);

        if (_moveAmount.y != 0)
        {
            VerticalCollisions(ref _moveAmount);
        }

        transform.Translate(_moveAmount);

        if(standingOnPlatform == true)
        {
            collisions.below = true;
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 _moveAmountOld;
        public int faceDir;
        public float _lastWallNormal;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownSlope = false;

            slopeAngleOld = slopeAngle;
            _lastWallNormal = 0;
            slopeAngle = 0;
        }
    }

    void HorizontalCollisions(ref Vector2 _moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(_moveAmount.x) + skinWidth;

        if (Mathf.Abs(_moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if(hit.distance == 0)
                {
                    continue;
                }
                collisions._lastWallNormal = hit.normal.x;
                
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        _moveAmount = collisions._moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        _moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref _moveAmount, slopeAngle);
                    _moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                _moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        _moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(_moveAmount.x); 
                    }

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 _moveAmount)
    {
        float directionY = Mathf.Sign(_moveAmount.y);
        float rayLength = Mathf.Abs(_moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + _moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                _moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    _moveAmount.x = _moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(_moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(_moveAmount.x);
            rayLength = Mathf.Abs(_moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * _moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                    {
                    _moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    }
                }
            }
    }

    void ClimbSlope(ref Vector2 _moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(_moveAmount.x);
        float climb_moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (_moveAmount.y <= climb_moveAmountY)
        {
            _moveAmount.y = climb_moveAmountY;
            _moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(_moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }
    void DescendSlope(ref Vector2 _moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(_moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(_moveAmount.y) + skinWidth, collisionMask);
        SlideDownMaxSlope(maxSlopeHitLeft, ref _moveAmount);
        SlideDownMaxSlope(maxSlopeHitRight, ref _moveAmount);

        if (!collisions.slidingDownSlope)
        {

            float directionX = Mathf.Sign(_moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(_moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(_moveAmount.x);
                            float descend_moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            _moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(_moveAmount.x);
                            _moveAmount.y -= descend_moveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 _moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if(slopeAngle > maxSlopeAngle)
            {
                _moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(_moveAmount.y - hit.distance)) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownSlope = true;
            }
        }
    }
}
