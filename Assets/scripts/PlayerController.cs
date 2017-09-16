using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    
    private bool CanJump = true;
    private bool left = false;
    private float xoffset = 0.175f;
    private float distance;
    

    public GameObject box;
    public float speed;
    public float thrust;
    public float boxDistance;
    public float minDistance;
    public float boxSpeed;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        box = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (transform.childCount > 0)
        {
            MoveBoxes();
        }
    }

    void FixedUpdate()
    {
        
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.A)){ //left
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            left = true;
            sr.flipX = true;
            //animator.SetBool("moving", true);
        }
        if (Input.GetKey(KeyCode.D)){ //right
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            left = false;
            sr.flipX = false;
            //animator.SetBool("moving", true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        { //jumpkey
            Debug.Log("clickety W");
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            //animator.SetBool("moving", false);
        } 
        if (Input.GetKey(KeyCode.F))
        {
            box.GetComponent<Rigidbody2D>().simulated = false;
            box.GetComponent<BoxCollider2D>().enabled = false;
            if (left)
            {
                xoffset = -0.175f;
            }
            else
            {
                xoffset = 0.175f;
            }
            box.transform.position = new Vector3(transform.position.x + xoffset, transform.position.y + 0.025f, 0);
            animator.SetBool("setcrate", true);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("non-clickety F");
            box.GetComponent<Rigidbody2D>().simulated = true;
            box.GetComponent<BoxCollider2D>().enabled = true;
            box.GetComponent<SpriteRenderer>().color = Color.white;
            box.transform.parent = null;
            animator.SetBool("setcrate", false);
        }
    }
    void MoveBoxes()
    {
        distance = Vector3.Distance(gameObject.transform.position, transform.GetChild(0).gameObject.transform.position);
        Debug.Log(distance);
        Vector3 newPosition = gameObject.transform.position; //basically same as not having it but saved for later use.
        float T = Time.deltaTime * distance * minDistance * boxSpeed;

        if (T > 0.2f)
        {
            T = 0.2f;
        }

        if (distance > boxDistance)
        {
            transform.GetChild(0).gameObject.transform.position = Vector3.Lerp(box.transform.position, newPosition, T);
        }
    }
}

