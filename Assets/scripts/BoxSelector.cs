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

        private Vector2 gridCoordinates;

        private Box selectedBox;
        private NewBoxPlace selectedNewBoxPlace;
        private List<Box> placedBoxes;
        private List<NewBoxPlace> newBoxPlacesInLevel;
        private NewBoxPlace liquidNewBoxPlace;

        /// <summary>
        /// Is it possible to place a box to the selector's coordinates
        /// </summary>
        private bool validPlacement;

        /// <summary>
        /// Is it possible to remove a box from the selector's coordinates
        /// </summary>
        private bool validRemove;

        /// <summary>
        /// Was the selector moved in this frame
        /// </summary>
        private bool moved;

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

        //[SerializeField]
        private LayerMask groundMask;

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
            CheckForErrors();

            // Initializes the box list
            placedBoxes = boxController.GetPlacedBoxes();

            // Initializes the new box place next to a
            // placed box and next to the player character
            liquidNewBoxPlace = GetComponentInChildren<NewBoxPlace>();

            // Initializes the level new box place list
            InitNewBoxPlacesInLevel();

            // Initializes visibility
            visibility = GetComponent<Renderer>();
            sr = GetComponent<SpriteRenderer>();
            visibility.enabled = false;

            //ShowSelector();

            // Sets the selector's size
            SetSize();

            // Sets the selector's starting position (testing purposes only)
            gridCoordinates = Vector2.zero;
            transform.position = level.GridOffset;

            groundMask = LayerMask.GetMask("Environment", "PlacedBoxes");
            //pathFinder = GameObject.FindGameObjectWithTag("PathFinder").GetComponent<PathFinding1>();
            validPlacement = false;
            validRemove = false;
            moved = false;
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

        private void CheckForErrors()
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

        private void SetSize()
        {
            Vector3 gridScale =
                level.GetGridScale(GetComponent<SpriteRenderer>().bounds.size);

            transform.localScale = new Vector3(transform.localScale.x * gridScale.x,
                                               transform.localScale.y * gridScale.y);
        }

        private void InitNewBoxPlacesInLevel()
        {
            newBoxPlacesInLevel = new List<NewBoxPlace>();

            foreach (NewBoxPlace nbp in FindObjectsOfType<NewBoxPlace>())
            {
                // Only if there is not already a new box place at
                // the same coordinates, the nbp is added to the list
                AddLevelNewBoxPlaceIfUnique(nbp);
            }

            // Prints debug info
            Debug.Log("New box places found in the level: "
                      + newBoxPlacesInLevel.Count);
        }

        /// <summary>
        /// Adds the given new box place to the nbp list if
        /// there isn't already one at the same coordinates.
        /// </summary>
        /// <param name="nbp">a new box place</param>
        private void AddLevelNewBoxPlaceIfUnique(NewBoxPlace nbp)
        {
            if (nbp.Owner == NewBoxPlace.Parent.Environment &&
                !LevelNewBoxPlaceExists(nbp.GridCoordinates))
            {
                newBoxPlacesInLevel.Add(nbp);
            }
        }

        private bool LevelNewBoxPlaceExists(Vector2 gridCoordinatesNBP)
        {
            foreach (NewBoxPlace existingNBP in newBoxPlacesInLevel)
            {
                if (existingNBP.GridCoordinates == gridCoordinatesNBP)
                {
                    return true;
                }
            }

            return false;
        }

        //private Vector2[] GetGridCellsOccupiedByPlayer()
        //{
        //    // TODO: Add IGridObject to player, use player.GridCoord???

        //    Vector2[] playerCornerGridCoords = new Vector2[4];

        //    Vector3 playerSize =
        //        player.GetComponent<BoxCollider2D>().bounds.size;

        //    // A small distance to fix the player character
        //    // touching the grid cell beneath it
        //    float offset = 0.1f;

        //    // Top left
        //    playerCornerGridCoords[0] = LevelController.GetGridCoordinates
        //        (player.transform.position + new Vector3(-1 * playerSize.x / 2, playerSize.y / 2));

        //    // Top right
        //    playerCornerGridCoords[1] = LevelController.GetGridCoordinates
        //        (player.transform.position + new Vector3(playerSize.x / 2, playerSize.y / 2));

        //    // Bottom left
        //    playerCornerGridCoords[2] = LevelController.GetGridCoordinates
        //        (player.transform.position + new Vector3(-1 * playerSize.x / 2, -1 * playerSize.y / 2 + offset));

        //    // Bottom right
        //    playerCornerGridCoords[3] = LevelController.GetGridCoordinates
        //        (player.transform.position + new Vector3(playerSize.x / 2, -1 * playerSize.y / 2 + offset));

        //    return playerCornerGridCoords;
        //}

        private float[] GetPlayerSideGridXCoords()
        {
            float[] playerSideGridXCoords = new float[2];

            Vector3 playerSize =
                player.GetComponent<BoxCollider2D>().bounds.size;

            // Left
            playerSideGridXCoords[0] = LevelController.GetGridCoordinates
                (player.transform.position + new Vector3(-1 * playerSize.x / 2, 0)).x;

            // Right
            playerSideGridXCoords[1] = LevelController.GetGridCoordinates
                (player.transform.position + new Vector3(playerSize.x / 2, 0)).x;

            return playerSideGridXCoords;
        }

        private bool SelectorIsNextToPlacedBox()
        {
            foreach (Box box in placedBoxes)
            {
                bool horNextTo = Mathf.Abs(gridCoordinates.x - box.GridCoordinates.x) == 1;
                bool vertNextTo = Mathf.Abs(gridCoordinates.y - box.GridCoordinates.y) == 1;

                bool horTouch = horNextTo && gridCoordinates.y == box.GridCoordinates.y;
                bool vertTouch = vertNextTo && gridCoordinates.x == box.GridCoordinates.x;

                if (horTouch || vertTouch)
                {
                    //Debug.Log("The selector is next to a placed box:\n" +
                    //          "box.x: " + boxGridCoord.x + ", box.y: " + boxGridCoord.y);
                    return true;
                }
            }

            return false;
        }

        private bool SelectorIsNextToPlayer()
        {
            // TODO: Add IGridObject to player, use player.GridCoord
            Vector2 playerGridCoord = 
                LevelController.GetGridCoordinates(player.transform.position);

            float[] playerSideGridXCoords = GetPlayerSideGridXCoords();

            // TODO: MaxDistance should be used and it needs SerializeField
            // (note that distance on either side should only be in one direction,
            // i.e. left to left and right to right)
            //int maxDistance = 1;

            bool horizontalOK =
                (int) (playerSideGridXCoords[0] - gridCoordinates.x) == 1 ||
                (int) (gridCoordinates.x - playerSideGridXCoords[1]) == 1;
            bool verticalOK = (gridCoordinates.y == playerGridCoord.y);

            if (horizontalOK && verticalOK)
            {
                //Debug.Log("The selector is next to the player");

                // Uses raycast to determine if the selector is on top of solid ground
                Vector3 bottomCenter = transform.position + new Vector3(0, -1 * level.GridCellWidth / 2);
                RaycastHit2D grounded = Physics2D.Raycast(bottomCenter, Vector2.down, 0.1f, groundMask);

                // TODO: Ask from Grid1 if the node is grounded
                //Grid1 grid;
                //Node1 node = LevelController.GetNodeFromGridCoord(grid, gridCoordinates);
                //bool onGround = node.IsGrounded(grid);

                if (grounded)
                {
                    //Debug.Log("BoxSelector: rayHit");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Places the selector next to the player character.
        /// Used when the selector is made visible
        /// and the mouse cursor is not used.
        /// </summary>
        private void PlaceSelectorNextToPlayer()
        {
            // The player character's size
            Vector3 playerSize = player.GetComponent<BoxCollider2D>().bounds.size;

            // The new position next to the player character
            Vector3 newPosition = player.transform.position;

            // Does the player character look left or right
            bool playerLooksLeft = player.GetComponent<SpriteRenderer>().flipX;

            // Based on the player character's looking direction, its
            // left or right edge is used to get the grid coordinates
            if (playerLooksLeft)
            {
                newPosition = player.transform.position - new Vector3(playerSize.x / 2, 0);
            }
            else
            {
                newPosition = player.transform.position + new Vector3(playerSize.x / 2, 0);
            }

            // Gets the grid coordinates of the position
            gridCoordinates = LevelController.GetGridCoordinates(newPosition);

            // Calculates the y-value for the selector's new position
            float minimumY = player.transform.position.y - playerSize.y / 2 + level.GridCellWidth / 2;

            // If the selector's position would be too low,
            // its y-coordinate is increased by 1
            if (newPosition.y < minimumY)
            {
                gridCoordinates.y++;
            }

            // The x-coordinate is shifted by one to
            // the player character's looking direction
            if (playerLooksLeft)
            {
                gridCoordinates.x--;
            }
            else
            {
                gridCoordinates.x++;
            }

            // Moves the selector to the grid coordinates
            MoveToGridCoordinates();
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
            HandleSelectorActivation();

            // Only accepts input for the selector if it is visible
            if (visibility.enabled)
            {
                if (cursor.Visible)
                {
                    MouseMovevent();

                    // Input for placing and removing a box
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        if (validPlacement)
                        {
                            PlaceBox();
                        }
                        else if (validRemove)
                        {
                            RemoveBox();
                        }
                    }

                    //if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    //{
                    //    PlaceBox();
                    //}
                    //else if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
                    //{
                    //    RemoveBox();
                    //}
                }
                else
                {
                    DirectionalMovevent();

                    // Input for placing and removing a box
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (validPlacement)
                        {
                            PlaceBox();
                        }
                        else if (validRemove)
                        {
                            RemoveBox();
                        }
                    }
                }
            }
        }

        private void HandleSelectorActivation()
        {
            // If the left shift key is held, the selector is visible
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // If the selector is just now being made visible,
                // its position is set next to the player character
                // and placement validity is checked
                if (!visibility.enabled)
                {
                    ShowSelector();
                }
            }
            // If the left shift key is released, the selector is hidden
            else if (visibility.enabled)
            {
                HideSelector();
            }

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    ToggleActivation();
            //}
        }

        private void ToggleActivation()
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

        /// <summary>
        /// Moves the selector to where the cursor is while snapping to a grid.
        /// </summary>
        private void MouseMovevent()
        {
            moved = false;

            Vector2 newGridCoordinates = LevelController.GetGridCoordinates(
                cursor.Position);

            // If the new cell is different to the old one,
            // the selector is moved to the new position
            if (newGridCoordinates != gridCoordinates)
            {
                moved = true;

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
            moved = false;

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
            transform.position = LevelController.GetPosFromGridCoord(
                gridCoordinates);

            UnselectAll();

            // Prints debug info
            //Debug.Log("BoxSelector - New grid coordinates: " + gridCoordinates);
        }

        private void ShowSelector()
        {
            // Makes the selector visible
            visibility.enabled = true;

            // The selector is moved with directional buttons
            // -> the selector appears next to the player character
            if (!cursor.Visible)
            {
                PlaceSelectorNextToPlayer();
                closeEnoughToPlayer = true;
            }
            // The selector is moved with the mouse
            // -> the selector appears under the mouse cursor
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
                boxController.RemovePlacedBox();
                UnselectBox();
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

                boxController.CheckRemovingBoxes(selectedBox);

                ChangeColor();

                // Prints debug info
                //Debug.Log("Box selected");
            }
        }

        public void RefreshSelectedBox()
        {
            Box temp = selectedBox;
            selectedBox = null;
            boxController.ClearRemovingBoxes();
            SelectBox(temp);
        }

        private void SelectNewBoxPlace(NewBoxPlace newBoxPlace)
        {
            // TODO: Is selectedNewBoxPlace necessary? Remove if not.

            if (newBoxPlace != selectedNewBoxPlace)
            {
                selectedBox = null;
                selectedNewBoxPlace = newBoxPlace;
                validPlacement = true;
                validRemove = false;

                ChangeColor();

                // Prints debug info
                //Debug.Log("New box place selected");
            }
        }

        private void UnselectBox()
        {
            if (validRemove)
            {
                selectedBox = null;
                //validPlacement = false;
                validRemove = false;

                boxController.ClearRemovingBoxes();

                ChangeColor();

                // Prints debug info
                //Debug.Log("Box unselected");
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
                //Debug.Log("NewBoxPlace unselected");
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
            // TODO: Use the 'moved' bool to limit unnecessary checks
            // when no boxes are being placed or removed.

            // TODO: Store the coordinates of a cell where a box was just placed;
            // it should not be possible to place boxes to the same cell
            // until the first one reaches it and is removed later

            // Goes through the placed box list and checks
            // if any of them is in the same grid coordinates
            foreach (Box placedBox in placedBoxes)
            {
                // If the box is in the same
                // coordinates, it is selected
                if (gridCoordinates == placedBox.GridCoordinates)
                {
                    SelectBox(placedBox);
                    return;
                }
            }

            // Checks if there's a new box place in the same grid coordinates
            // (only if there are boxes following the player)
            if (boxController.MovingBoxAmount() > 0)
            {
                if (SelectorIsNextToPlacedBox())
                {
                    liquidNewBoxPlace.GridCoordinates =
                        gridCoordinates;
                    SelectNewBoxPlace(liquidNewBoxPlace);
                    return;
                }

                if (SelectorIsNextToPlayer())
                {
                    liquidNewBoxPlace.GridCoordinates =
                        gridCoordinates;
                    SelectNewBoxPlace(liquidNewBoxPlace);
                    return;
                }

                // Goes through the level nbp list and checks
                // if any of them is in the same grid coordinates
                foreach (NewBoxPlace newBoxPlace in newBoxPlacesInLevel)
                {
                    // If the nbp is in the same
                    // coordinates, it is selected
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
            if (!playerGrounded || collidesWithObstacle) // || !closeEnoughToPlayer
            {
                // Note: invalidPlacementColor is used (instead of
                // generalInvalidColor) because testers said that
                // two colors marking invalidity is confusing

                sr.color = invalidPlacementColor;
                //sr.color = generalInvalidColor;
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