using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class MouseCursorController : MonoBehaviour
    {
        private const int Z_AXIS = 0;

        /// <summary>
        /// The renderer of the object. Needed to
        /// hide the object but still update it.
        /// </summary>
        private Renderer visibility;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            visibility = GetComponent<Renderer>();
            visibility.enabled = true;
        }

        /// <summary>
        /// Gets or sets the mouse cursor's visibility.
        /// </summary>
        public bool Visible
        {
            get
            {
                return visibility.enabled;
            }
            set
            {
                visibility.enabled = value;
            }
        }

        /// <summary>
        /// Gets the mouse cursor's position.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        /// <summary>
        /// Updates the mouse cursor.
        /// </summary>
        private void Update()
        {
            UpdatePosition();

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Visible = !Visible;
            }
        }

        /// <summary>
        /// Updates the mouse cursor's position.
        /// </summary>
        private void UpdatePosition()
        {
            // Gets the system mouse cursor's position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            // Translates the screen coordinates to world coordinates
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Sets the game mouse cursor's position
            transform.position = new Vector3(mousePosition.x,
                                             mousePosition.y,
                                             Z_AXIS);
        }
    }
}
