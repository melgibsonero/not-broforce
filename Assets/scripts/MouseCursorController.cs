using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class MouseCursorController : MonoBehaviour
    {
        /// <summary>
        /// The sprite renderer of the object.
        /// </summary>
        private SpriteRenderer sr;

        /// <summary>
        /// The cursor texture
        /// </summary>
        private Texture2D cursorTexture;

        /// <summary>
        /// The cursor mode (Auto or ForceSoftware)
        /// </summary>
        private CursorMode cursorMode = CursorMode.Auto;

        /// <summary>
        /// The cursor's main point (in pixels)
        /// </summary>
        private Vector2 hotSpot;

        /// <summary>
        /// Is playing with the mouse enabled
        /// </summary>
        private bool playingWithMouse;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            PlayingUsingMouse = true;
            InitSystemCursor();
        }

        /// <summary>
        /// Initializes the cursor.
        /// The system cursor is used but with a custom texture.
        /// </summary>
        private void InitSystemCursor()
        {
            // Gets the sprite renderer
            sr = GetComponent<SpriteRenderer>();

            // Hides the custom cursor
            Visible = false;

            // Gets the cursor texture
            cursorTexture = sr.sprite.texture;

            // Sets the cursor's main point
            hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

            // Gives the game object's sprite to the system cursor
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

        /// <summary>
        /// Initializes the cursor.
        /// The custom cursor is used.
        /// </summary>
        private void InitCustomCursor()
        {
            // Gets the sprite renderer
            sr = GetComponent<SpriteRenderer>();

            // Hides the operating system's cursor
            ShowSystemCursor(false);
        }

        /// <summary>
        /// Shows or hides the operating system's cursor.
        /// </summary>
        /// <param name="show">will the cursor be shown</param>
        private void ShowSystemCursor(bool show)
        {
            Cursor.visible = show;
        }

        /// <summary>
        /// Resets the system cursor to default.
        /// </summary>
        private void ResetSystemCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

        /// <summary>
        /// Gets or sets the mouse cursor's visibility.
        /// </summary>
        public bool Visible
        {
            get
            {
                return sr.enabled;
            }
            set
            {
                sr.enabled = value;
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
        /// Gets or sets whether the game is played with the mouse.
        /// </summary>
        public bool PlayingUsingMouse
        {
            get
            {
                return playingWithMouse;
            }
            set
            {
                playingWithMouse = value;

                if (sr == null)
                {
                    InitSystemCursor();
                }

                //Visible = value;
                ShowSystemCursor(value);
            }
        }

        /// <summary>
        /// Updates the mouse cursor.
        /// </summary>
        private void Update()
        {
            UpdatePosition();
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
                                             transform.position.z);
        }


        public void TogglePlayingUsingMouse()
        {
            PlayingUsingMouse = !PlayingUsingMouse;
        }
    }
}
