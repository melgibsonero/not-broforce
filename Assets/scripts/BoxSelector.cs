using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class BoxSelector : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private LevelController level;

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
        private Color generalInvalidColor;

        [SerializeField]
        private float maxDistanceFromPlayer = 3;

        /// <summary>
        /// The renderer of the object. Needed to
        /// hide the object but still update it.
        /// </summary>
        private Renderer visibility;

        /// <summary>
        /// The sprite renderer of the object.
        /// Needed to change the object's color.
        /// </summary>
        private SpriteRenderer sr;

        /// <summary>
        /// The layer mask for raycast
        /// </summary>
        //private int mask;

        private Vector2 gridCoordinates;

        private List<Box> placedBoxes;
        private List<NewBoxPlace> newBoxPlaces;
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
        /// Does the selector collide with the player character
        /// or any part of the environment
        /// </summary>
        private bool collidesWithObstacle;

        /// <summary>
        /// Is the selector close enough to the player character
        /// </summary>
        private bool closeEnoughToPlayer;

        /// <summary>
        /// Is the player character standing on the ground
        /// </summary>
        private bool playerGrounded;

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set { gridCoordinates = value; }
        }

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            // Checks if any necessary objects are not attached
            CheckForMissingObjects();

            // Initializes the box list
            placedBoxes = boxController.GetPlacedBoxes();

            // Initializes the new box place list
            InitNewBoxPlaceList();

            // Initializes visibility
            visibility = GetComponent<Renderer>();
            visibility.enabled = false;

            // Sets the selector's size
            SetSize();

            // Sets the selector's starting position (testing purposes only)
            gridCoordinates = Vector2.zero;
            transform.position = level.GridOffset;

            sr = GetComponent<SpriteRenderer>();

            //mask = LayerMask.GetMask("Environment", "PlacedBoxes");

            validPlacement = false;
            validRemove = false;
            closeEnoughToPlayer = true;
            playerGrounded = true;
        }

        /// <summary>
        /// Gets whether the selector usable at its current state.
        /// Returns true if the selector is visible, close enough to
        /// the player character and the player character is on ground.
        /// </summary>
        /// <returns>is the selector usable at its current state</returns>
        private bool IsUsable()
        {
            return (visibility.enabled &&
                    closeEnoughToPlayer && playerGrounded);
        }

        private bool BoxCanBePlaced()
        {
            return (IsUsable() && validPlacement &&
                    !collidesWithObstacle);
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
            if (level == null)
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

            if (cursor == null)
            {
                throw new System.NullReferenceException
                    ("Mouse cursor not set to the selector.");
            }
        }

        private void InitNewBoxPlaceList()
        {
            newBoxPlaces = new List<NewBoxPlace>();

            int amountBefore = 0;
            int amountAfter = 0;

            // The new box place next to the player
            if (newBoxPlaceNextToPlayer != null)
            {
                newBoxPlaces.Add(newBoxPlaceNextToPlayer);
            }
            amountAfter = newBoxPlaces.Count;

            // Prints debug info
            Debug.Log("New box places found next to player: " + (amountAfter - amountBefore));

            // New box places next to placed boxes
            amountBefore = amountAfter;
            FetchNewBoxPlacesNextToPlacedBoxes();
            amountAfter = newBoxPlaces.Count;

            // Prints debug info
            Debug.Log("New box places found next to placed boxes: " + (amountAfter - amountBefore));

            // New box places in the level
            amountBefore = amountAfter;
            FetchNewBoxPlacesInLevel();
            amountAfter = newBoxPlaces.Count;

            // Prints debug info
            Debug.Log("New box places found in the level: " + (amountAfter - amountBefore));

            // Prints debug info
            Debug.Log("New box places found, total: " + amountAfter);
        }

        private void FetchNewBoxPlacesNextToPlacedBoxes()
        {
            foreach (Box box in placedBoxes)
            {
                for (int i = 0; i < 4; i++)
                {
                    Utils.Direction side = (Utils.Direction) i;

                    // Testing purposes only
                    // TODO: add IGridObject to Box and use GridCoordinates
                    Vector2 boxGridCoordinates =
                        Utils.GetGridCoordinates(box.transform.position,
                        level.GridCellWidth, level.GridOffset);

                    // The new box place's grid coordinates
                    Vector2 gridCoordinatesNBP =
                        Utils.GetAdjacentGridCoordinates(boxGridCoordinates,
                                                         side);

                    // Only if there is not already a new box place at
                    // the same coordinates, the nbp is created
                    if (GetExistingNewBoxPlace(gridCoordinatesNBP) == null)
                    {
                        NewBoxPlace nbp = new NewBoxPlace();

                        nbp.Initialize(gridCoordinatesNBP, level,
                                       NewBoxPlace.Parent.Box);

                        //nbp.Initialize(
                        //    Utils.GetGridCoordinates(box.GridCoordinates, side),
                        //    levelController,
                        //    NewBoxPlace.Parent.Box);

                        newBoxPlaces.Add(nbp);
                    }
                }
            }
        }

        private void FetchNewBoxPlacesInLevel()
        {
            foreach (NewBoxPlace nbp in FindObjectsOfType<NewBoxPlace>())
            {
                // Only if there is not already a new box place at
                // the same coordinates, the nbp is added to the list
                if (GetExistingNewBoxPlace(nbp.GridCoordinates) == null)
                {
                    newBoxPlaces.Add(nbp);
                }
            }
        }

        /// <summary>
        /// Removes all new box places next to
        /// placed boxes and fetches them all again.
        /// </summary>
        /// <returns>was anything changed</returns>
        private bool ResetNewBoxPlacesNextToPlacedBoxes()
        {
            int amountBefore = newBoxPlaces.Count;
            int amountAfter = 0;

            foreach (NewBoxPlace nbp in newBoxPlaces)
            {
                if (nbp.Owner == NewBoxPlace.Parent.Box)
                {
                    newBoxPlaces.Remove(nbp);
                }
            }

            FetchNewBoxPlacesNextToPlacedBoxes();

            amountAfter = newBoxPlaces.Count;
            return (amountBefore != amountAfter);
        }

        /// <summary>
        /// Removes all new box places in the level
        /// from the list and fetches them all again.
        /// </summary>
        /// <returns>was anything changed</returns>
        private bool ResetNewBoxPlacesInLevel()
        {
            int amountBefore = newBoxPlaces.Count;
            int amountAfter = 0;

            foreach (NewBoxPlace nbp in newBoxPlaces)
            {
                if (nbp.Owner == NewBoxPlace.Parent.Environment)
                {
                    newBoxPlaces.Remove(nbp);
                }
            }

            FetchNewBoxPlacesInLevel();

            amountAfter = newBoxPlaces.Count;
            return (amountBefore != amountAfter);
        }

        private NewBoxPlace GetExistingNewBoxPlace(Vector2 gridCoordinatesNBP)
        {
            foreach (NewBoxPlace existingNBP in newBoxPlaces)
            {
                if (existingNBP.GridCoordinates == gridCoordinatesNBP)
                {
                    return existingNBP;
                }
            }

            return null;
        }

        private void SetSize()
        {
            Vector3 gridScale =
                level.GetGridScale(GetComponent<SpriteRenderer>().bounds.size);

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
                level.GridCellWidth,
                level.GridOffset);

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
        public void MoveToGridCoordinates()
        {
            transform.position = Utils.GetPosFromGridCoord(
                gridCoordinates,
                level.GridCellWidth,
                level.GridOffset);

            UnselectAll();

            // Prints debug info
            //Debug.Log("BoxSelector - New grid coordinates: " + gridCoordinates);
        }

        private void ShowSelector()
        {
            // Makes the selector visible
            visibility.enabled = true;

            // The selector is moved with directional buttons
            if (!cursor.Visible)
            {
                // Does the player character look left or right
                bool playerLooksLeft = player.GetComponent<SpriteRenderer>().flipX;

                // Calculates a new position next to the player character
                Vector3 newPosition = player.transform.position +
                    (playerLooksLeft ? Vector3.left : Vector3.right) * level.GridCellWidth;

                // Gets the grid coordinates of the position
                gridCoordinates = Utils.GetGridCoordinates(newPosition, level.GridCellWidth, level.GridOffset);

                // Moves the selector to the grid coordinates
                MoveToGridCoordinates();

                //transform.position = newBoxPlaceNextToPlayer.transform.position;

                closeEnoughToPlayer = true;
            }
            // The selector is moved with the mouse
            else
            {
                MouseMovevent();
            }
        }

        private void HideSelector()
        {
            visibility.enabled = false;
            collidesWithObstacle = false;
            UnselectAll();
        }

        private void PlaceBox()
        {
            if (BoxCanBePlaced())
            {
                boxController.PlaceBox(transform.position);
                newBoxPlaces.Remove(GetExistingNewBoxPlace(gridCoordinates));

                if (boxController.MovingBoxAmount() == 0)
                {
                    // Prints debug info
                    Debug.Log("Out of boxes to place");
                }
            }
        }

        private void RemoveBox()
        {
            if (BoxCanBeRemoved())
            {
                boxController.RemovePlacedBox(selectedBox);
                UnselectBox();

                // TODO: Make sure resetting new box places works. Or come up with a new system entirely.

                // Resets new box places so the removed box leaves a place for another
                // (NOTE: not every removed box should leave an nbp)
                bool nextToBox = ResetNewBoxPlacesNextToPlacedBoxes();
                if (!nextToBox)
                {
                    ResetNewBoxPlacesInLevel();
                }
            }
        }

        private void SelectBox(Box box)
        {
            if (box != selectedBox)
            {
                selectedBox = box;
                selectedNewBoxPlace = null;
                validPlacement = false;
                validRemove = true;

                ChangeColor();

                // Prints debug info
                Debug.Log("Box selected");
            }
        }

        private void SelectNewBoxPlace(NewBoxPlace newBoxPlace)
        {
            if (newBoxPlace != selectedNewBoxPlace)
            {
                selectedBox = null;
                selectedNewBoxPlace = newBoxPlace;
                validPlacement = true;
                validRemove = false;

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
                //validPlacement = false;
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
                //validRemove = false;

                ChangeColor();

                // Prints debug info
                Debug.Log("NewBoxPlace unselected");
            }
        }

        /// <summary>
        /// Unselects a possible selected box or a new box place.
        /// </summary>
        private void UnselectAll()
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
            // If the selector is too far away from the player,
            // placing and removing boxes are made invalid
            if (IsTooFarAwayFromPlayer())
            {
                // (This condition is here to prevent unnecessary invalidation)
                if (closeEnoughToPlayer)
                {
                    closeEnoughToPlayer = false;
                    collidesWithObstacle = false;
                    UnselectAll();
                }
            }
            else if (!closeEnoughToPlayer)
            {
                closeEnoughToPlayer = true;
            }

            // If the selector is in a usable state, it's 
            // checked what is in the current grid coordinates
            if (IsUsable())
            {
                CheckGridCoordinates();
            }

            // Sets the selector's color based on its status
            ChangeColor();
        }

        /// <summary>
        /// Checks the content of the current grid coordinates.
        /// If there's a placed box or a new box place, it is selected.
        /// </summary>
        private void CheckGridCoordinates()
        {
            // TODO: Fix a box never being added to the placed box list
            //Debug.Log("Placed boxes: " + placedBoxes.Count);

            // Checks if there's a placed box in the same grid coordinates
            foreach (Box placedBox in placedBoxes)
            {
                // Testing purposes only
                // TODO: add IGridObject to Box and use GridCoordinates
                Vector2 boxGridCoordinates =
                    Utils.GetGridCoordinates(placedBox.transform.position,
                    level.GridCellWidth, level.GridOffset);

                if (gridCoordinates == boxGridCoordinates)
                {
                    SelectBox(placedBox);
                    return;
                }
            }

            // Checks if there's a new box place in the same grid coordinates
            // (only if there are placeable boxes following the player)
            if (boxController.MovingBoxAmount() > 0)
            {
                foreach (NewBoxPlace newBoxPlace in newBoxPlaces)
                {
                    if (gridCoordinates == newBoxPlace.GridCoordinates)
                    {
                        SelectNewBoxPlace(newBoxPlace);
                        return;
                    }
                }
            }

            // If nothing was selected, placement and removing are invalid
            UnselectAll();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsUsable())
            {
                // The collider which collides with the selector
                BoxCollider2D trigger = other.GetComponent<BoxCollider2D>();

                if (trigger != null)
                {
                    // If the collision is deeper than just a touch,
                    // the selector is set to collide with an obstacle
                    if (Utils.CollidersIntersect(GetComponent<BoxCollider2D>(), trigger, 0.9f))
                    {
                        collidesWithObstacle = true;
                    }
                    // Otherwise the collision is ignored
                    else
                    {
                        collidesWithObstacle = false;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsUsable())
            {
                collidesWithObstacle = false;
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
        //private bool SelectBoxUnderSelector(float modifier)
        //{
        //    Box box = GetBoxUnderSelector(modifier);

        //    if (box != null)
        //    {
        //        SelectBox(box);

        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Goes through the box list and checks if a box
        /// intersects with the selector. The amount of overlap
        /// needed can be controlled with the modifier.
        /// </summary>
        /// <param name="modifier">the required overlap modifier
        /// (1 = any overlap)</param>
        /// <returns>a box which the selector intersects with</returns>
        //private Box GetBoxUnderSelector(float modifier)
        //{
        //    // Goes through each placed box
        //    foreach (Box box in placedBoxes)
        //    {
        //        bool intersects = false;

        //        if (cursor.Visible)
        //        {
        //            intersects = Utils.ColliderContainsPoint(
        //                box.GetComponent<BoxCollider2D>(),
        //                cursor.Position);
        //        }
        //        else
        //        {
        //            intersects = Utils.CollidersIntersect(
        //                GetComponent<BoxCollider2D>(),
        //                box.GetComponent<BoxCollider2D>(),
        //                modifier);
        //        }

        //        // Checks if the selector intersects with an
        //        // existing box and if so, makes that box selected
        //        if (intersects)
        //        {
        //            return box;
        //        }
        //    }

        //    return null;
        //}

        /// <summary>
        /// Sets the selector's color based on its status.
        /// </summary>
        private void ChangeColor()
        {
            if (!closeEnoughToPlayer || !playerGrounded || collidesWithObstacle)
            {
                sr.color = generalInvalidColor;
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
                sr.color = invalidPlacementColor;
            }
        }
    }
}