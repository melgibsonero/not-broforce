using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace not_broforce
{
    public class MouseCursorController : MonoBehaviour
    {
        public static bool cursorActive;

        [SerializeField]
        /// <summary>
        /// The cursor texture
        /// </summary>
        private Texture2D cursorTexture;

        /// <summary>
        /// The sprite renderer of the object.
        /// </summary>
        //private SpriteRenderer sr;

        /// <summary>
        /// The cursor's main point (in pixels)
        /// </summary>
        private Vector2 hotSpot;

        /// <summary>
        /// The cursor mode (Auto or ForceSoftware)
        /// </summary>
        private CursorMode cursorMode = CursorMode.Auto;

        /// <summary>
        /// Is playing with the mouse cursor not hidden
        /// </summary>
        private bool playingUsingMouse;

        /// <summary>
        /// Was the mouse moved
        /// </summary>
        private bool mouseMoved;

        /// <summary>
        /// Where was the cursor in the last frame
        /// </summary>
        private Vector3 oldScreenPosition;

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
            //sr = GetComponent<SpriteRenderer>();

            // Hides the custom cursor
            //Visible = false;

            // Gets the cursor texture
            //cursorTexture = sr.sprite.texture;

            // Sets the cursor's main point
            hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

            // Gives the game object's sprite to the system cursor
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

        /// <summary>
        /// Initializes the cursor.
        /// The custom cursor is used.
        /// </summary>
        //private void InitCustomCursor()
        //{
        //    // Gets the sprite renderer
        //    sr = GetComponent<SpriteRenderer>();

        //    // Hides the operating system's cursor
        //    ShowSystemCursor(false);
        //}

        /// <summary>
        /// Shows or hides the operating system's cursor.
        /// </summary>
        /// <param name="show">will the cursor be shown</param>
        private void ShowSystemCursor(bool show)
        {
            // Shows or hides the cursor
            Cursor.visible = show;

            // Activates or deativates the cursor
            //Cursor.
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
        //public bool Visible
        //{
        //    get
        //    {
        //        return sr.enabled;
        //    }
        //    set
        //    {
        //        sr.enabled = value;
        //    }
        //}

        /// <summary>
        /// Gets the mouse cursor's world position.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        /// <summary>
        /// Gets the mouse cursor's screen position.
        /// </summary>
        public Vector3 ScreenPosition { get; private set; }

        /// <summary>
        /// Gets or sets whether the game is played with the mouse.
        /// </summary>
        public bool PlayingUsingMouse
        {
            get
            {
                return playingUsingMouse;
            }
            set
            {
                if (playingUsingMouse != value)
                {
                    playingUsingMouse = value;
                    cursorActive = value;

                    //if (sr == null)
                    //{
                    //    InitSystemCursor();
                    //}

                    //Visible = value;
                    ShowSystemCursor(value);

                    if (playingUsingMouse)
                    {
                        //Debug.Log("cursor shown");

                        EventSystem.current.SetSelectedGameObject(null);
                    }
                    else
                    {
                        //Debug.Log("cursor hidden");

                        ClearCursorHighlight();

                        Utils.SelectButton(FindObjectOfType<Button>());

                        // Prevents the cursor being immediately shown again
                        oldScreenPosition = ScreenPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the mouse cursor.
        /// </summary>
        private void Update()
        {
            UpdatePosition();
            CheckIfPlayingUsingMouse();
        }

        /// <summary>
        /// Updates the mouse cursor's position.
        /// </summary>
        private void UpdatePosition()
        {
            // Sets the cursor's old screen
            // position for checking if it changed
            oldScreenPosition = ScreenPosition;

            // Gets the system mouse cursor's position in screen coordinates
            ScreenPosition = Input.mousePosition;

            // Sets whether the cursor was moved
            //mouseMoved = (ScreenPosition != oldScreenPosition);

            // Translates the screen coordinates to world coordinates
            Vector3 worldPosition =
                Camera.main.ScreenToWorldPoint(ScreenPosition);

            // Sets the game mouse cursor's position
            transform.position = new Vector3(worldPosition.x,
                                             worldPosition.y,
                                             transform.position.z);
        }


        public void TogglePlayingUsingMouse()
        {
            PlayingUsingMouse = !PlayingUsingMouse;

            //if (!PlayingUsingMouse)
            //{
            //   // Prevents the cursor being immediately shown again
            //   cursorPos = cursor.Position;
            //}
        }

        private void CheckIfPlayingUsingMouse()
        {
            if (PlayingUsingMouse)
            {
                if (PlayerInput.GetSelectorDirection()
                    != Utils.Direction.None)
                {
                    PlayingUsingMouse = false;
                }
            }
            else
            {
                // Moving the mouse or using its buttons shows the mouse cursor
                if (ScreenPosition != oldScreenPosition ||
                    Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    PlayingUsingMouse = true;
                }
            }
        }

        private void ClearCursorHighlight()
        {
            PointerEventData pointer =
                new PointerEventData(EventSystem.current);
            pointer.position = ScreenPosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            foreach (RaycastResult raycastResult in raycastResults)
            {
                //Debug.Log(raycastResult.gameObject.name);
                GameObject hoveredObj = raycastResult.gameObject;

                bool success = ClearHighlight(hoveredObj, pointer);

                // Sometimes, the child image may be interactable
                // -> highlight from parent object is cleared
                if (!success)
                {
                    ClearHighlight(
                        hoveredObj.transform.parent.gameObject, pointer);
                }
            }
        }

        private bool ClearHighlight(GameObject uiElement,
                                    PointerEventData pointer)
        {
            // Highlighted button
            Button button = uiElement.GetComponent<Button>();

            // Highlighted toggle
            Toggle toggle = uiElement.GetComponent<Toggle>();

            // Highlighted slider
            Slider slider = uiElement.GetComponent<Slider>();

            if (button != null)
            {
                button.OnPointerExit(pointer);
                //Debug.Log("Button highlight cleared");

                return true;
            }
            if (toggle != null)
            {
                toggle.OnPointerExit(pointer);
                //Debug.Log("Toggle highlight cleared");

                return true;
            }
            if (slider != null)
            {
                slider.OnPointerExit(pointer);
                //Debug.Log("Slider highlight cleared");

                return true;
            }

            return false;
        }
    }
}
