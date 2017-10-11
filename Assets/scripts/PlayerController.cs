using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class PlayerController : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timetoJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    private void Start()
    {
        controller = GetComponent<Controller2D>();

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

        if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public bool GetGrounded()
    {
        return true;
    }

}
    /*bool CheckGrounded() //Grounded raycasts
    {
        bool result = false;        
        Vector3 size = new Vector3(bc.size.x, 0, 0);

        RaycastHit2D middle = Physics2D.Raycast(transform.position, Vector2.down, distancey, mask);
        RaycastHit2D left = Physics2D.Raycast(transform.position+-size, Vector2.down, distancey, mask);
        RaycastHit2D right = Physics2D.Raycast(transform.position+size, Vector2.down, distancey, mask);

        if(middle.collider != null || left.collider != null || right.collider != null)
        {
            result = true;
        }

        return result;
    } 

    bool CheckWalljump()
    {
        bool result = false;
        RaycastHit2D walljumpright = Physics2D.Raycast(transform.position, Vector2.right, distancex, mask);
        RaycastHit2D walljumpleft = Physics2D.Raycast(transform.position, Vector2.left, distancex, mask);
        if(walljumpright.collider != null)
        {
            result = true;
        }
        if(walljumpleft.collider != null && left)
        {
            result = true;
        }
        return result;
    }
    void FixedUpdate()
    {
        //Grounded 
        Physics2D.queriesStartInColliders = false;
        
        if (CheckGrounded())
        {
            Grounded = true;
            Walled = false;
        }
    }
    void ApplyGravity()
    {
        transform.Translate(0, -gravity * Time.deltaTime, 0);
    }
    void Jump()
    {
    }
    void Move()
    {
        //if (!Grounded) ApplyGravity();
        
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1) //Horizontal movement
        {
            float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            if (Grounded)
            {
                if (horizontal < 0)
                {
                    left = true;
                }
                else
                {
                    left = false;
                }
            }
            transform.Translate(horizontal, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.Space)) //vertical movement
        {
            if (Grounded)
            {
                Jumping = true;
                Grounded = false;
            }
            if (Walled)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(walljumpdir, ForceMode2D.Impulse);
                left = !left;
                Walled = false;
            }
        }
        if (Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.F))
        {
            if (CanBox)
            {
                CanBox = false;
                box = Instantiate(box);
                box.GetComponent<Rigidbody2D>().simulated = false;
                box.GetComponent<BoxCollider2D>().enabled = false;
            }
            if (left)
            {
                xoffset = -0.225f;
            }
            else
            {
                xoffset = 0.225f;
            }
            box.transform.position = new Vector3(transform.position.x + xoffset, transform.position.y + 0.025f, 0);
        }
        if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("non-clickety F");
            box.GetComponent<Rigidbody2D>().simulated = true;
            box.GetComponent<BoxCollider2D>().enabled = true;
            box.GetComponent<SpriteRenderer>().color = Color.white;
            box.transform.parent = null;
            CanBox = true;
        }
        Jump();
    }
    
    void Animations()
    {
        if (left)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        if (CanBox)
        {
            animator.SetBool("setcrate", false);
        }
        else
        {
            animator.SetBool("setcrate", true);
        }

        if (Grounded)
        {
            animator.SetBool("jumping", false);
        }
        else
        {
            animator.SetBool("jumping", true);
        }
    }
    */
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 size = new Vector3(bc.size.x, 0, 0);

        Gizmos.DrawLine(transform.position + size, transform.position + size + Vector3.down * distancey);
        Gizmos.DrawLine(transform.position + -size, transform.position + -size + Vector3.down * distancey);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distancey);

        Gizmos.color = Color.green;
        if (!left)
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * distancex);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * distancex);
        }
    }
    //*/


