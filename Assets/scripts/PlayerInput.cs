using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace not_broforce
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        PlayerController player;
        // Use this for initialization
        void Start()
        {
            player = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            //Left and right movement
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            //Jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.OnJumpInputUp();
            }
            //Other things
        }
    }
}
