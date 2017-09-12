using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luigi : MonoBehaviour {
	private Animator animator;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	public float speed;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
	}

	void Move() {
		if (Input.GetKey (KeyCode.S)){
			animator.SetBool ("moving", false);
			animator.SetBool ("crouch", true);
		}else{
			animator.SetBool ("crouch", false);
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
				animator.SetBool ("moving", true);
			} else {
				animator.SetBool ("moving", false);
			}
			if (Input.GetKey (KeyCode.A)){
				transform.Translate (Vector3.left * Time.deltaTime * speed);
				sr.flipX = true;
			}
			if (Input.GetKey (KeyCode.D)){
				transform.Translate (Vector3.right * Time.deltaTime * speed);
				sr.flipX = false;
			}
			if (Input.GetKeyDown(KeyCode.W)) {
				rb.AddForce (transform.up*speed*10, ForceMode2D.Impulse);

		}
		}
	}
}

