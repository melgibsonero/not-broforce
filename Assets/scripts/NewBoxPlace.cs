using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class NewBoxPlace : MonoBehaviour
    {
        [SerializeField]
        private GameObject box;

        [SerializeField]
        private Utils.Direction side = Utils.Direction.Up;

        [SerializeField]
        private bool nextToPlayer;

        private PlayerController player;

        private float boxSideLength;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            if (box != null)
            {
                boxSideLength = box.GetComponent<SpriteRenderer>().bounds.size.x;

                if (nextToPlayer)
                {
                    player = FindObjectOfType<PlayerController>();
                    side = Utils.Direction.Right;
                }
            }
        }

        public Box ParentBox
        {
            get
            {
                if (!nextToPlayer)
                {
                    return box.GetComponent<Box>();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            if (box != null)
            {
                if (nextToPlayer)
                {
                    UpdatePositionNextToPlayer();
                }
                else
                {
                    UpdatePositionNextToBox();
                }
            }
        }

        private void UpdatePositionNextToPlayer()
        {
            // Calculates the ground level based on the player character's position
            float groundY = player.transform.position.y -
                player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            // Sets the side based the player character's looking direction
            side = player.GetComponent<SpriteRenderer>().flipX ? Utils.Direction.Left : Utils.Direction.Right;

            // Calculates the x-coordinate
            float x = (player.transform.position + Utils.DirectionToVector3(side) / 2).x;

            // Sets the selector's position
            transform.position = new Vector3(x, groundY + boxSideLength / 2);
        }

        private void UpdatePositionNextToBox()
        {
            Vector3 boxPosition = box.transform.position;

            switch (side)
            {
                case Utils.Direction.Up:
                    {
                        transform.position = boxPosition + new Vector3(0, boxSideLength);
                        break;
                    }
                case Utils.Direction.Down:
                    {
                        transform.position = boxPosition + new Vector3(0, -1 * boxSideLength);
                        break;
                    }
                case Utils.Direction.Left:
                    {
                        transform.position = boxPosition + new Vector3(-1 * boxSideLength, 0);
                        break;
                    }
                case Utils.Direction.Right:
                    {
                        transform.position = boxPosition + new Vector3(boxSideLength, 0);
                        break;
                    }
            }
        }
    }
}
