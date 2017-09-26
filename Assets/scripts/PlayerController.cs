using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (left)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    void FixedUpdate()
    {
        //Grounded 
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distancey);
        
        if (hit.collider != null)
        {
            Grounded = true;
            Walled = false;
        }
        else{ //Walljump raycasts
            Grounded = false;
            RaycastHit2D walljump = left ? Physics2D.Raycast(transform.position, Vector2.left, distancex) : 
                Physics2D.Raycast(transform.position, Vector2.right, distancex);
            if(walljump.collider != null)
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
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
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
            rb.AddForce(new Vector2(horizontal,0), ForceMode2D.Force);
            if (rb.velocity.x > maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            if(rb.velocity.x < -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
            
        }
        else
        {
            //zeroing movement speed to 0
            if (Grounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.W))
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
            animator.SetBool("setcrate", true);
        }
        if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("non-clickety F");
            box.GetComponent<Rigidbody2D>().simulated = true;
            box.GetComponent<BoxCollider2D>().enabled = true;
            box.GetComponent<SpriteRenderer>().color = Color.white;
            box.transform.parent = null;
            CanBox = true;
            animator.SetBool("setcrate", false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

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
}

