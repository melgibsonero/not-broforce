using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    
    private bool Grounded = true;
    private bool Walled = false;
    private bool CanBox = true;
    private bool left = false;
    private float xoffset = 0.275f;
    private Vector2 walljumpdir;
    public float distancey = 0.4f;
    public float distancex = 0.2f;

    public float jumpray = 0f;
    

    public GameObject box;
    public float speed;
    public float maxSpeed;
    public float deceleration_speed = 0.5f;
    public float thrust;
    public float wallthrust;
    public float boxDistance;
    public float minDistance;
    public float boxSpeed;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animations();
    }
    bool CheckGrounded() //Grounded raycasts
    {
        bool result = false;        
        Vector3 size = new Vector3(bc.size.x, 0, 0);

        RaycastHit2D middle = Physics2D.Raycast(transform.position, Vector2.down, distancey);
        RaycastHit2D left = Physics2D.Raycast(transform.position+-size, Vector2.down, distancey);
        RaycastHit2D right = Physics2D.Raycast(transform.position+size, Vector2.down, distancey);

        if(middle.collider != null || left.collider != null || right.collider != null)
        {
            result = true;
        }

        return result;
    } 

    bool CheckWalljump()
    {
        bool result = false;
        RaycastHit2D walljumpright = Physics2D.Raycast(transform.position, Vector2.right, distancex);
        RaycastHit2D walljumpleft = Physics2D.Raycast(transform.position, Vector2.left, distancex);
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
        else
        { //Walljump raycasts
            Grounded = false;
            if(CheckWalljump())
            {
                Walled = true;
                walljumpdir = left ? new Vector2(wallthrust, thrust) : new Vector2(wallthrust*-1, thrust);
            }
            else
            {
                Walled = false;
            }
        }
    }

    void Move()
    {
        Debug.Log(rb.velocity);
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1) //Horizontal movement
        {
            float horizontal = Input.GetAxis("Horizontal") * speed;
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
            //transform.Translate(horizontal, 0, 0);
            if (rb.velocity.x > maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            if (rb.velocity.x < -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
            rb.AddForce(new Vector2(horizontal,0), ForceMode2D.Force);
            
            
        }
        else
        {
            //zeroing movement speed to 0
            if (Grounded)
            {
                var newSpeed = rb.velocity.x;
                if (Mathf.Approximately(newSpeed, 0))
                {
                    newSpeed = 0;
                }
                rb.velocity = new Vector2(newSpeed*deceleration_speed, rb.velocity.y);
            }
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.W)) //vertical movement
        {
            if (Grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
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
    
}

