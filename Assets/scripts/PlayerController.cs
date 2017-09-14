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

    public float speed;
    public float thrust;
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
        
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.A)){ //left
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            left = true;
            sr.flipX = true;
            animator.SetBool("moving", true);
        }
        if (Input.GetKey(KeyCode.D)){ //right
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            left = false;
            sr.flipX = false;
            animator.SetBool("moving", true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        { //jumpkey
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetBool("moving", false);
        } 
        if (Input.GetKeyDown(KeyCode.F))
        {

            animator.SetBool("setcrate", true);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            animator.SetBool("setcrate", false);
        }
    }
}

