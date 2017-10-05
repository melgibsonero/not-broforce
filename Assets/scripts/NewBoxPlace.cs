using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class NewBoxPlace : MonoBehaviour
    {
        [SerializeField]
        private GameObject parentBox;

        [SerializeField]
        private Utils.Direction side = Utils.Direction.Up;

        private float boxSideLength;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            if (parentBox != null)
            {
                boxSideLength = parentBox.GetComponent<SpriteRenderer>().bounds.size.x;
            }
        }

        public Box ParentBox
        {
            get { return parentBox.GetComponent<Box>(); }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            if (parentBox != null)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector3 boxPosition = parentBox.transform.position;

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
