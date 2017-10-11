using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class PlayerController : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timetoJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    SpriteRenderer spriterer;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        spriterer = GetComponent<SpriteRenderer>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timetoJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timetoJumpApex;
        print("Gravity: " + gravity + "VelocityJump: " + jumpVelocity);
    }

    private void Update()
    {
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Animate();
    }

    public bool GetGrounded()
    {
        return controller.collisions.below;
    }

    void Animate()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            float directionX = Mathf.Sign(velocity.x);

            if (directionX == -1)
            {
                spriterer.flipX = true;
            }
            if (directionX == 1)
            {
                spriterer.flipX = false;
            }
        }

    }

}


