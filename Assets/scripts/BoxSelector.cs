using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class BoxSelector : MonoBehaviour
    {
        [SerializeField]
        private LevelController levelController;

        [SerializeField]
        private BoxController boxController;

        [SerializeField]
        private GameObject player;

        [SerializeField]
        private NewBoxPlace newBoxPlaceNextToPlayer;

        [SerializeField]
        private MouseCursorController cursor;

        [SerializeField]
        private Color validPlacementColor;

        [SerializeField]
        private Color invalidPlacementColor;

        [SerializeField]
        private Color removeColor;

        [SerializeField]
        private float placedBoxSnapAccuracy;

        [SerializeField]
        private float newBoxPlaceSnapAccuracy;

        [SerializeField]
        private float smoothMovingSpeed;

        [SerializeField]
        private float maxDistanceFromPlayer = 2;

        /// <summary>
        /// The renderer of the object. Needed to
        /// hide the object but still update it.
        /// </summary>
        private Renderer visibility;

        private bool showAfterMoving;

        /// <summary>
        /// The sprite renderer of the object.
        /// Needed to change the object's color.
        /// </summary>
        private SpriteRenderer sr;

        /// <summary>
        /// The layer mask for raycast
        /// </summary>
        private int mask;

        //private RaycastHit2D touchesGround;

        private Vector2 gridCoordinates;

        private List<Box> placedBoxes;
        private List<Box> newBoxPlaces;
        private Box selectedBox;
        private NewBoxPlace selectedNewBoxPlace;

        /// <summary>
        /// Is it possible to place a box to the position of the selector
        /// </summary>
        private bool validPlacement;

        /// <summary>
        /// Is it possible to remove a box from the position of the selector
        /// </summary>
        private bool validRemove;

        /// <summary>
        /// Is the selector close enough to the player character
        /// </summary>
        private bool closeEnoughToPlayer;

        /// <summary>
        /// Is the player character standing on the ground
        /// </summary>
        private bool playerGrounded;

        /// <summary>
        /// Does the selector snap to a grid of boxes
        /// </summary>
        private bool snapsToBoxGrid;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            // Checks if any necessary objects are not attached
            CheckForMissingObjects();

            // Sets the box list
            //boxes = new List<GameObject>();
            placedBoxes = boxController.GetPlacedBoxes();


            // TODO: New box place list
            //newBoxPlaces = 


            // Sets the selector's size
            SetSize();

            // Sets the selector's starting position (testing purposes only)
            gridCoordinates = Vector2.zero;
            transform.position = levelController.GridOffset;

            visibility = GetComponent<Renderer>();
            visibility.enabled = false;
            showAfterMoving = false;

            sr = GetComponent<SpriteRenderer>();

            mask = LayerMask.GetMask("Environment", "PlacedBoxes");

            validPlacement = true;
            closeEnoughToPlayer = true;
            playerGrounded = true;
            validRemove = false;
            snapsToBoxGrid = false;
        }

        /// <summary>
        /// Gets whether the selector usable at its current state.
        /// Returns true if the selector is visible and
        /// close enough to the player character.
        /// </summary>
        /// <returns>is the selector usable at its current state</returns>
        private bool IsUsable()
        {
            return (visibility.enabled &&
                    closeEnoughToPlayer && playerGrounded);
        }

        private bool BoxCanBePlaced()
        {
            return (IsUsable() && validPlacement);
        }

        private bool BoxCanBeRemoved()
        {
            return (IsUsable() && validRemove);
        }

        private bool IsTooFarAwayFromPlayer()
        {
            return (IsTooFarAwayFromPlayer(transform.position));
        }

        private bool IsTooFarAwayFromPlayer(Vector3 position)
        {
            return (Utils.Distance(position, player.transform.position)
                    > maxDistanceFromPlayer);
        }

        private void CheckForMissingObjects()
        {
            if (levelController == null)
            {
                throw new System.NullReferenceException
                    ("LevelController not set to the selector.");
            }

            if (boxController == null)
            {
                throw new System.NullReferenceException
                    ("BoxController not set to the selector.");
            }

            if (player == null)
            {
                throw new System.NullReferenceException
                    ("Player not set to the selector.");
            }

            if (newBoxPlaceNextToPlayer == null)
            {
                throw new System.NullReferenceException
                    ("NewBoxPlace next to the player character " +
                     "not set to the selector.");
            }

            if (cursor == null)
            {
                throw new System.NullReferenceException
                    ("Mouse cursor not set to the selector.");
            }
        }

        private void SetSize()
        {
            Vector3 gridScale =
                levelController.GetGridScale(GetComponent<SpriteRenderer>().bounds.size);

            transform.localScale = new Vector3(transform.localScale.x * gridScale.x,
                                               transform.localScale.y * gridScale.y);
        }

        /// <summary>
        /// Updates the game object once per frame.
        /// </summary>
        private void Update()
        {
            HandleInput();

            if (visibility.enabled)
            {
                if (player.GetComponent<PlayerController>().GetGrounded())
                {
                    playerGrounded = true;
                }
                else
                {
                    playerGrounded = false;
                }

                CheckPlacementValidity();
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleActivation();
            }

            // Only accepts input for the selector if it is visible
            else if (visibility.enabled)
            {
                if (cursor.Visible)
                {
                    MouseMovevent();

                    // Input for placing and removing a box
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        PlaceBox();
                    }
                    else if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
                    {
                        RemoveBox();
                    }
                }
                else
                {
                    DirectionalMovevent();

                    // Input for placing and removing a box
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlaceBox();
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        RemoveBox();
                    }
                }
            }
        }

        private void ToggleActivation()
        {
            if (visibility.enabled ||
                player.GetComponent<PlayerController>().GetGrounded())
            {
                // Displays or hides the selector
                visibility.enabled = !visibility.enabled;

                // If the selector was made visible, its position is set next to
                // the player character and placement validity is checked
                if (visibility.enabled)
                {
                    ShowSelector();
                }
                // Otherwise any selected box is unselected
                else
                {
                    HideSelector();
                }
            }
        }

        /// <summary>
        /// Moves the selector to where the cursor is while snapping to a grid.
        /// </summary>
        private void MouseMovevent()
        {
            Vector2 newGridCoordinates = Utils.GetGridCoordinates(
                cursor.Position,
                levelController.GridCellWidth,
                levelController.GridOffset);

            // If the new cell is different to the old one,
            // the selector is moved to the new position
            if (newGridCoordinates != gridCoordinates)
            {
                // Updates the grid coordinates
                gridCoordinates = newGridCoordinates;

                // Moves the selector to the coordinates
                MoveToGridCoordinates();
            }
        }

        ///// <summary>
        ///// Moves the selector to where the cursor is while snapping to a grid.
        ///// </summary>
        //private void MouseMovevent()
        //{
        //    // Calculates the cursor's grid coordinates
        //    float cursorX = cursor.Position.x
        //        + levelController.GridCellWidth / 2
        //        - levelController.GridOffset.x;
        //    float cursorY = cursor.Position.y
        //        + levelController.GridCellWidth / 2
        //        - levelController.GridOffset.y;

        //    // Rounds the cursor's grid coordinates
        //    // down when they're in the negatives
        //    if (cursorX < 0)
        //    {
        //        cursorX -= levelController.GridCellWidth;
        //    }
        //    if (cursorY < 0)
        //    {
        //        cursorY -= levelController.GridCellWidth;
        //    }

        //    // Gets the grid coordinates in which the cursor currently is
        //    int gridX = 
        //        (int) (cursorX / levelController.GridCellWidth);
        //    int gridY = 
        //        (int) (cursorY / levelController.GridCellWidth);

        //    Vector2 newGridCoordinates = new Vector2(gridX, gridY);

        //    // If the new cell is different to the old one,
        //    // the selector is moved to the new position
        //    if (newGridCoordinates != gridCoordinates)
        //    {
        //        // Updates the grid coordinates
        //        gridCoordinates = newGridCoordinates;

        //        // Moves the selector to the coordinates
        //        MoveToGridCoordinates();
        //    }
        //}

        /// <summary>
        /// Checks input for moving the selector with directional buttons.
        /// </summary>
        private void DirectionalMovevent()
        {
            bool moved = false;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gridCoordinates.y++;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gridCoordinates.y--;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gridCoordinates.x--;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gridCoordinates.x++;
                moved = true;
            }

            // Moves the selector to the coordinates
            if (moved)
            {
                MoveToGridCoordinates();
            }
        }

        /// <summary>
        /// Moves the selector to the saved grid coordinates.
        /// </summary>
        private void MoveToGridCoordinates()
        {
            transform.position = Utils.GetPositionFromGridCoord(
                gridCoordinates,
                levelController.GridCellWidth,
                levelController.GridOffset);


            // TODO: Change the selector's color based on what is in the grid cell
            UnselectBox();


            // Prints debug info
            Debug.Log("BoxSelector - New grid coordinates: " + gridCoordinates);
        }

        private void ShowSelector()
        {
            if (!visibility.enabled)
            {
                visibility.enabled = true;
            }

            if (!cursor.Visible)
            {
                transform.position = newBoxPlaceNextToPlayer.transform.position;
                closeEnoughToPlayer = true;
            }
            else
            {
                MouseMovevent();
            }
        }

        private void HideSelector()
        {
            if (visibility.enabled)
            {
                visibility.enabled = false;
            }

            snapsToBoxGrid = false;
            InvalidateAll();
        }

        private void PlaceBox()
        {
            if (BoxCanBePlaced())
            {
                boxController.PlaceBox(transform.position);
            }
        }

        private void RemoveBox()
        {
            if (BoxCanBeRemoved())
            {
                boxController.RemovePlacedBox(selectedBox);
                UnselectBox();

                // TODO: Fix a box not starting to follow the player sometimes when removed

                // TODO: If a box cannot reach its target, it gives up and starts following the player
            }
        }

        private void SelectBox(Box box)
        {
            if (box != selectedBox &&
                !IsTooFarAwayFromPlayer(box.transform.position))
            {
                transform.position = box.transform.position;
                selectedBox = box;
                selectedNewBoxPlace = null;
                validPlacement = false;
                validRemove = true;

                if (!cursor.Visible)
                {
                    snapsToBoxGrid = true;
                }

                ChangeColor();

                // Prints debug info
                //Debug.Log("Box selected");
            }
        }

        private void SelectNewBoxPlace(NewBoxPlace newBoxPlace)
        {
            if (newBoxPlace != selectedNewBoxPlace &&
                !IsTooFarAwayFromPlayer(newBoxPlace.transform.position))
            {
                transform.position = newBoxPlace.transform.position;
                selectedBox = null;
                selectedNewBoxPlace = newBoxPlace;
                validPlacement = true;
                validRemove = false;

                if (!cursor.Visible)
                {
                    snapsToBoxGrid = true;
                }

                ChangeColor();

                // Prints debug info
                Debug.Log("New box place selected");
            }
        }

        private void UnselectBox()
        {
            if (validRemove)
            {
                selectedBox = null;
                validPlacement = true;
                validRemove = false;
                ChangeColor();

                // Prints debug info
                Debug.Log("Box unselected");
            }
        }

        private void UnselectNewBoxPlace()
        {
            if (validPlacement)
            {
                selectedNewBoxPlace = null;
                validPlacement = false;
                validRemove = false;
                ChangeColor();

                // Prints debug info
                Debug.Log("NewBoxPlace unselected");
            }
        }

        private void ValidatePlacement()
        {
            // TODO: Fix placement being valid even though it should not be.
            // Going from red to purple might actually change the selector blue!

            if (!validPlacement &&
                boxController.MovingBoxAmount() > 0)
            {
                validPlacement = true;
            }
        }

        private void InvalidateAll()
        {
            UnselectBox();
            UnselectNewBoxPlace();
        }

        /// <summary>
        /// Checks if a box can be placed to the selector's position.
        /// </summary>
        /// <returns>can a box be placed to the selector's position</returns>
        private void CheckPlacementValidity()
        {
            // TODO: Fix the selector turning red when snapping to a valid new box place

            // TODO: If the player moves, the selector on the new box place next to it becomes red

            // TODO: The selector can select the new box place next to the player character in arrow keys mode 

            // If the selector is too far away from the player,
            // placing and removing boxes are made invalid
            if (IsTooFarAwayFromPlayer())
            {
                if (closeEnoughToPlayer)
                {
                    closeEnoughToPlayer = false;
                    UnselectBox();
                }
            }
            else if (!closeEnoughToPlayer)
            {
                closeEnoughToPlayer = true;
            }

            // If the selector is in valid distance,
            // it's checked what the selector can do
            if (closeEnoughToPlayer && playerGrounded)
            {
                // Checks should a box be selected;
                // the selector must be directly on
                // the center if the placed box
                //if (!validRemove)
                //{
                //    SelectBoxUnderSelector(0.1f);
                //}

                // Placement is invalid if a placed box
                // has not been selected and there
                // are no boxes following the player
                if (validPlacement && !validRemove &&
                    boxController.MovingBoxAmount() == 0)
                {
                    validPlacement = false;

                    // Prints debug info
                    Debug.Log("Out of boxes to place");
                }
            }

            // Sets the selector's color based on its status
            ChangeColor();
        }

        //private void CheckGridCell()
        //{
        //    foreach (Box placedBox in placedBoxes)
        //    {
        //        if (gridCoordinates == placedBox.GridCoordinates)
        //        {
        //            SelectBox(placedBox);
        //            return;
        //        }
        //    }

        //    foreach (NewBoxPlace newBoxPlace in newBoxPlaces)
        //    {
        //        if (gridCoordinates == newBoxPlace.GridCoordinates)
        //        {
        //            SelectNewBoxPlace(newBoxPlace);
        //            return;
        //        }
        //    }
        //}

        private void OnTriggerStay2D(Collider2D other)
        {
            // TODO: The selector determines the object under it from grid coordinates, not from collision

            if (IsUsable())
            {
                Box box = other.gameObject.GetComponent<Box>();
                NewBoxPlace newBoxPlace = other.gameObject.GetComponent<NewBoxPlace>();

                if (box != null && placedBoxes.Contains(box))
                {
                    SelectBox(box);
                }
                else if (newBoxPlace != null && newBoxPlace.GridCoordinates == gridCoordinates
                    && (placedBoxes.Contains(newBoxPlace.ParentBox) || newBoxPlace.NBPOwner == NewBoxPlace.Owner.Environment))
                {
                    SelectNewBoxPlace(newBoxPlace);
                }
                else if (newBoxPlace == null)
                {
                    InvalidateAll();
                }

                //Box box = other.gameObject.GetComponent<Box>();
                //NewBoxPlace newBoxPlace = other.gameObject.GetComponent<NewBoxPlace>();

                //if (box != null && placedBoxes.Contains(box) &&
                //    Utils.CollidersIntersect(GetComponent<BoxCollider2D>(), box.GetComponent<BoxCollider2D>(), 0.5f))
                //{
                //    SelectBox(box);
                //}
                //else if (newBoxPlace != null &&
                //         GetBoxUnderSelector(0.8f) == null &&
                //         Utils.CollidersIntersect(GetComponent<BoxCollider2D>(),
                //                                  newBoxPlace.GetComponent<BoxCollider2D>(), 0.2f))
                //{
                //    SelectNewBoxPlace(newBoxPlace);
                //}
                //else if (newBoxPlace == null)
                //{
                //    InvalidateAll();
                //}
            }
        }

        /// <summary>
        /// Goes through the box list and selects the first box that
        /// intersects with the selector. The amount of overlap needed
        /// can be controlled with the modifier.
        /// </summary>
        /// <param name="modifier">the required overlap modifier
        /// (1 = any overlap)</param>
        /// <returns>was a box selected</returns>
        private bool SelectBoxUnderSelector(float modifier)
        {
            Box box = GetBoxUnderSelector(modifier);

            if (box != null)
            {
                SelectBox(box);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Goes through the box list and checks if a box
        /// intersects with the selector. The amount of overlap
        /// needed can be controlled with the modifier.
        /// </summary>
        /// <param name="modifier">the required overlap modifier
        /// (1 = any overlap)</param>
        /// <returns>a box which the selector intersects with</returns>
        private Box GetBoxUnderSelector(float modifier)
        {
            // Goes through each placed box
            foreach (Box box in placedBoxes)
            {
                bool intersects = false;

                if (cursor.Visible)
                {
                    intersects = Utils.ColliderContainsPoint(
                        box.GetComponent<BoxCollider2D>(),
                        cursor.Position);
                }
                else
                {
                    intersects = Utils.CollidersIntersect(
                        GetComponent<BoxCollider2D>(),
                        box.GetComponent<BoxCollider2D>(),
                        modifier);
                }

                // Checks if the selector intersects with an
                // existing box and if so, makes that box selected
                if (intersects)
                {
                    return box;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the selector's color based on its status.
        /// </summary>
        private void ChangeColor()
        {
            bool invalid = false;

            if (!closeEnoughToPlayer || !playerGrounded)
            {
                invalid = true;
            }
            else if (validRemove)
            {
                sr.color = removeColor;
            }
            else if (validPlacement)
            {
                sr.color = validPlacementColor;
            }
            else
            {
                invalid = true;
            }

            if (invalid)
            {
                sr.color = invalidPlacementColor;
            }
        }
    }
}