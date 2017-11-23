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
        private int maxDistFromPlayer = 3;

        [SerializeField, Range(0, 5)]
        private int maxGroundPlaceDistX = 3;

        [SerializeField, Range(0, 5)]
        private int maxGroundPlaceDistUp = 2;

        [SerializeField, Range(0, 5)]
        private int maxGroundPlaceDistDown = 2;

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
        private Vector2 playerGridCoord;

        private Box selectedBox;
        private NewBoxPlace selectedNewBoxPlace;
        private List<Box> placedBoxes;
        private List<Vector2> reservedBoxPlaceCoords;
        private List<ValidBoxPlace> validBoxPlaces;
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
        /// Does the selector collide with the environment
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

        private Vector3 playerSize;

        private PlayerController playerCtrl;

        //[SerializeField]
        /// <summary>
        /// A mask which covers Environment and PlacedBoxes
        /// </summary>
        private LayerMask groundMask;

        private bool isAlwaysShown;

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
            // Initializes the box list
            placedBoxes = boxController.GetPlacedBoxes();

            // Initializes the list of reserved box place coordinates
            // (used for not allowing more than one box to be placed
            // in the same node)
            reservedBoxPlaceCoords = new List<Vector2>();

            // Places where a box can be placed
            validBoxPlaces = new List<ValidBoxPlace>();

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
            validPlacement = false;
            validRemove = false;
            closeEnoughToPlayer = true;
            playerGrounded = true;

            if (player != null)
            {
                // Gets the player character's size
                playerSize =
                    player.GetComponent<BoxCollider2D>().bounds.size;

                // Gets the player controller
                playerCtrl = player.GetComponent<PlayerController>();
            }

            // Checks if any necessary objects are not attached
            CheckForErrors();
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

        private bool TooFarAwayFromPlayer()
        {
            return (TooFarAwayFromPlayer(transform.position));
        }

        private bool TooFarAwayFromPlayer(Vector3 position)
        {
            return (Utils.Distance(position, player.transform.position)
                    > maxDistFromPlayer);
        }

        private bool TooFarAwayFromPlayer_Coord(Vector2 coord)
        {
            return (Utils.Distance(LevelController.GetPosFromGridCoord(coord),
                                   player.transform.position)
                    > maxDistFromPlayer);
        }

        private void CheckForErrors()
        {
            if (level == null)
            {
                Debug.LogError
                    ("LevelController not set to the selector.");
            }

            if (boxController == null)
            {
                Debug.LogError
                    ("BoxController not set to the selector.");
            }

            if (player == null)
            {
                Debug.LogError
                    ("Player not set to the selector.");
            }

            if (playerCtrl == null)
            {
                Debug.LogError
                    ("PlayerController component not found in player.");
            }

            if (cursor == null)
            {
                Debug.LogError
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
            //Debug.Log("New box places found in the level: "
            //          + newBoxPlacesInLevel.Count);
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

        private Box GetPlacedBoxInCoordinates(Vector2 coordinates)
        {
            Box placedBox = null;

            foreach (Box box in placedBoxes)
            {
                // If the box is in the same coordinates, it is chosen
                if (coordinates == box.GridCoordinates)
                {
                    placedBox = box;
                    break;
                }
            }

            // Returns the placed box
            return placedBox;
        }

        private Box GetPlacedBoxInSelectorCoord()
        {
            return GetPlacedBoxInCoordinates(gridCoordinates);
        }

        private float[] PlayerSideGridCoordXs()
        {
            float[] playerSideGridXCoords = new float[2];

            Vector3 pSize = playerSize;

            // Error correction; it isn't enough if only
            // a pixel is in the bordering grid node
            pSize.x -= 0.1f;

            // Left
            playerSideGridXCoords[0] = LevelController.GetGridCoordinates
                (player.transform.position + new Vector3(-1 * pSize.x / 2, 0)).x;

            // Right
            playerSideGridXCoords[1] = LevelController.GetGridCoordinates
                (player.transform.position + new Vector3(pSize.x / 2, 0)).x;

            return playerSideGridXCoords;
        }

        private int PlayerExtraGridCoordSide()
        {
            // Defaults to middle: 0 (no extra grid coord side)
            int side = 0;

            float[] playerSideGridXCoords = PlayerSideGridCoordXs();

            // Left: -1
            if (playerSideGridXCoords[0] < playerGridCoord.x)
            {
                side--;
            }
            // Right: 1
            else if (playerSideGridXCoords[1] > playerGridCoord.x)
            {
                side++;
            }

            return side;
        }

        public void RemoveReservedBoxPlace(Vector3 position)
        {
            RemoveReservedBoxPlace(
                LevelController.GetGridCoordinates(position));
        }

        public void RemoveReservedBoxPlace(Vector2 gridCoordinates)
        {
            foreach (Vector2 reservedPlace in reservedBoxPlaceCoords)
            {
                if (gridCoordinates == reservedPlace)
                {
                    reservedBoxPlaceCoords.Remove(reservedPlace);

                    //Debug.Log("Reserved space removed. Spaces left: " +
                    //    reservedBoxPlaceCoords.Count);

                    return;
                }
            }
        }

        private bool SelectorIsInReservedBoxPlace()
        {
            foreach (Vector2 reservedPlace in reservedBoxPlaceCoords)
            {
                if (gridCoordinates == reservedPlace)
                {
                    return true;
                }
            }

            return false;
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
            float[] playerSideGridCoordXs = PlayerSideGridCoordXs();

            bool horizontalOK = 
                SelectorIsWithinHorBounds(playerSideGridCoordXs[0],
                                          playerSideGridCoordXs[1]);
            bool verticalOK = SelectorIsWithinVertBounds(playerGridCoord.y);

            if (horizontalOK && verticalOK)
            {
                // Uses raycast to determine if the
                // selector is on top of solid ground
                Vector3 center = transform.position;
                RaycastHit2D grounded =
                    Physics2D.Raycast(center, Vector2.down, 1f, groundMask);

                if (grounded)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the selector's x-coordinate is not
        /// too far from the given x-coordinate.
        /// </summary>
        /// <param name="otherGridCoordX">an x-coordinate</param>
        /// <returns></returns>
        private bool SelectorIsWithinHorBounds(float otherGridCoordX)
        {
            return ((int) Mathf.Abs(otherGridCoordX - gridCoordinates.x)
                <= maxGroundPlaceDistX);
        }

        /// <summary>
        /// Checks if the selector's x-coordinate is not
        /// too far from one of the given x-coordinates.
        /// </summary>
        /// <param name="leftCoordX">x-coordinate of an object's
        /// left side</param>
        /// <param name="rightCoordX">x-coordinate of an object's
        /// right side</param>
        private bool SelectorIsWithinHorBounds(float leftCoordX,
                                               float rightCoordX)
        {
            // Checks if the selector is not too far from the left x-coord
            bool leftOK = (int) Mathf.Abs(leftCoordX - gridCoordinates.x)
                <= maxGroundPlaceDistX;

            // Checks if the selector is not too far from the right x-coord
            bool rightOK = (int) Mathf.Abs(rightCoordX - gridCoordinates.x)
                <= maxGroundPlaceDistX;

            // Returns whether either of the checks was true
            return (leftOK || rightOK);
        }

        private bool SelectorIsWithinVertBounds(float otherGridCoordY)
        {
            // The selector is above or at the
            // same level as the other coordinates
            if (gridCoordinates.y >= otherGridCoordY)
            {
                return ((int) (gridCoordinates.y - otherGridCoordY)
                        <= maxGroundPlaceDistUp);
            }
            // The selector is below the other coordinates
            else
            {
                return ((int) (otherGridCoordY - gridCoordinates.y)
                        <= maxGroundPlaceDistDown);
            }
        }

        /// <summary>
        /// Updates the game object once per frame.
        /// </summary>
        private void Update()
        {
            //HandleInput();

            if (visibility.enabled)
            {
                // Updates the player character's grid coordinates
                playerGridCoord =
                    LevelController.GetGridCoordinates(
                        player.transform.position);

                // Updates the selector's position if
                // the game is played using the mouse
                if (cursor.PlayingUsingMouse)
                {
                    MouseMovevent();
                }
                //else
                //{
                //    DirectionalMovevent();
                //}

                CheckIfPlayerGrounded();
                CheckPlacementValidity();
            }
            else if (isAlwaysShown)
            {
                ShowSelector();
            }
        }

        private void CheckIfPlayerGrounded()
        {
            if (playerCtrl.GetGrounded())
            {
                if (!playerGrounded)
                {
                    playerGrounded = true;
                }
            }
            else if (playerGrounded)
            {
                playerGrounded = false;
            }
        }

        public void OnActivationInputDown()
        {
            // If the selector is just now being made visible,
            // its position is set next to the player character
            // and placement validity is checked
            if (!visibility.enabled)
            {
                ShowSelector();

                // Testing purposes only
                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Score);
            }
        }

        public void OnActivationInputUp()
        {
            // The selector is hidden
            if (visibility.enabled)
            {
                HideSelector();
            }
        }

        public void OnPlacementInputDown()
        {
            // Only accepts input for the selector if it is visible
            if (visibility.enabled)
            {
                if (BoxCanBeRemoved()) //validRemove)
                {
                    RemoveBox();
                }
                else if (BoxCanBePlaced()) //validPlacement)
                {
                    PlaceBox();
                }
            }
        }

        //private void HandleInput()
        //{
        //    HandleSelectorActivation();

        //    // Only accepts input for the selector if it is visible
        //    if (visibility.enabled)
        //    {
        //        if (cursor.PlayingUsingMouse)
        //        {
        //            MouseMovevent();

        //            // Input for placing and removing a box
        //            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        //            {
        //                if (validRemove)
        //                {
        //                    RemoveBox();
        //                }
        //                else if (validPlacement)
        //                {
        //                    PlaceBox();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            DirectionalMovevent();

        //            // Input for placing and removing a box
        //            if (Input.GetKeyDown(KeyCode.E))
        //            {
        //                if (validRemove)
        //                {
        //                    RemoveBox();
        //                }
        //                else if (validPlacement)
        //                {
        //                    PlaceBox();
        //                }
        //            }
        //        }
        //    }
        //}

        //private void HandleSelectorActivation()
        //{
        //    // If the left shift key is held, the selector is visible
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        // If the selector is just now being made visible,
        //        // its position is set next to the player character
        //        // and placement validity is checked
        //        if (!visibility.enabled)
        //        {
        //            ShowSelector();
        //        }
        //    }
        //    // If the left shift key is released, the selector is hidden
        //    else if (visibility.enabled)
        //    {
        //        HideSelector();
        //    }
        //}

        private void ToggleActivation()
        {
            if (!isAlwaysShown)
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
        /// Places the selector next to the player character.
        /// Used when the selector is made visible
        /// and the mouse cursor is not used.
        /// </summary>
        private void PlaceSelectorNextToPlayer()
        {
            // Sets the grid coordinates the same as
            // the player character's coordinates
            gridCoordinates =
                LevelController.GetGridCoordinates(player.transform.position);

            // Moves the selector to the grid coordinates
            MoveToGridCoordinates();

            // The selector is now close enough to player
            closeEnoughToPlayer = true;
        }

        /// <summary>
        /// Moves the selector to where the cursor is while snapping to a grid.
        /// </summary>
        private void MouseMovevent()
        {
            if (!TooFarAwayFromPlayer(cursor.Position))
            {
                Vector2 newGridCoordinates =
                    LevelController.GetGridCoordinates(cursor.Position);

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
        }

        /// <summary>
        /// Moves the selector to the given direction.
        /// </summary>
        public void DirectionalMovement(Utils.Direction direction)
        {
            Vector2 movement = Vector2.zero;

            switch (direction)
            {
                case Utils.Direction.Up:
                {
                    movement.y++;
                    break;
                }
                case Utils.Direction.Down:
                {
                    movement.y--;
                    break;
                }
                case Utils.Direction.Left:
                {
                    movement.x--;
                    break;
                }
                case Utils.Direction.Right:
                {
                    movement.x++;
                    break;
                }
            }

            // Moves the selector to the coordinates if they
            // are not too far away from the player character
            if (TooFarAwayFromPlayer_Coord(gridCoordinates + movement))
            {
                if (TooFarAwayFromPlayer_Coord(gridCoordinates))
                {
                    PlaceSelectorNextToPlayer();
                }
            }
            else
            {
                gridCoordinates += movement;
                MoveToGridCoordinates();
            }
        }

        /// <summary>
        /// Checks input for moving the selector with directional buttons.
        /// </summary>
        private void DirectionalMovement()
        {
            bool moved = false;

            Vector2 movement = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                movement.y++;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                movement.y--;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movement.x--;
                moved = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movement.x++;
                moved = true;
            }

            // Moves the selector to the coordinates if they
            // are not too far away from the player character
            if (moved)
            {
                if (TooFarAwayFromPlayer_Coord(gridCoordinates + movement))
                {
                    if (TooFarAwayFromPlayer_Coord(gridCoordinates))
                    {
                        PlaceSelectorNextToPlayer();
                    }
                }
                else
                {
                    gridCoordinates += movement;
                    MoveToGridCoordinates();
                }
            }
        }

        /// <summary>
        /// Moves the selector to the saved grid coordinates.
        /// </summary>
        public void MoveToGridCoordinates()
        {
            transform.position =
                LevelController.GetPosFromGridCoord(gridCoordinates);

            //UpdateReservedBoxPlaces();

            UnselectAll();
        }

        private void ShowSelector()
        {
            // Makes the selector visible
            visibility.enabled = true;

            // Places the selector next to the player character
            PlaceSelectorNextToPlayer();

            // If the game is played using the
            // mouse, the selector is moved under
            // the mouse cursor which is hidden
            if (cursor.PlayingUsingMouse)
            {
                MouseMovevent();
                cursor.Visible = false;
            }
        }

        private void HideSelector()
        {
            if (!isAlwaysShown)
            {
                visibility.enabled = false;
                collidesWithObstacle = false;
                UnselectAll();

                if (cursor.PlayingUsingMouse)
                {
                    cursor.Visible = true;
                }
            }
        }

        /// <summary>
        /// Set whether the selector will always
        /// be shown and can't be hidden.
        /// </summary>
        /// <param name="showAlways">is the selector always shown</param>
        public void ShowAlways(bool showAlways)
        {
            isAlwaysShown = showAlways;
        }

        private void AddReservedBoxPlace()
        {
            //// Testing purposes only
            //if (reservedBoxPlaceCoords.Count == 3)
            //{
            //    reservedBoxPlaceCoords.Clear();
            //    Debug.Log("Reserved spaces reset");
            //}

            reservedBoxPlaceCoords.Add(gridCoordinates);

            //Debug.Log("Reserved space added. Spaces: " +
            //          reservedBoxPlaceCoords.Count);
        }

        private void UpdateReservedBoxPlaces()
        {
            for (int i = reservedBoxPlaceCoords.Count - 1; i >= 0; i--)
            {
                Vector2 reservedPlace = reservedBoxPlaceCoords[i];
                Box placedBox = GetPlacedBoxInCoordinates(reservedPlace);

                if (placedBox != null)
                {
                    reservedBoxPlaceCoords.RemoveAt(i);

                    //Debug.Log("Reserved space removed. Spaces left: " +
                    //    reservedBoxPlaceCoords.Count);
                }
            }
        }

        private void PlaceBox()
        {
            // Testing purposes only
            // Plays a sound
            SFXPlayer.Instance.Play(Sound.Impact);

            //if (BoxCanBePlaced())
            //{

            bool placed = boxController.PlaceBox(transform.position);

            if (placed)
            {
                AddReservedBoxPlace();
            }

            //if (boxController.MovingBoxAmount() == 0)
            //{
            //    // Prints debug info
            //    Debug.Log("Out of boxes to place");
            //}

            //}
        }

        private void RemoveBox()
        {
            //if (BoxCanBeRemoved())
            //{

            UpdateReservedBoxPlaces();

            boxController.RemovePlacedBox();
            UnselectBox();

            //}
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

        private void RemoveAllBoxes()
        {
            UnselectAll();
            reservedBoxPlaceCoords.Clear();
        }

        /// <summary>
        /// Checks if a box can be placed to the selector's position.
        /// </summary>
        /// <returns>can a box be placed to the selector's position</returns>
        private void CheckPlacementValidity()
        {
            // If the selector is too far away from the player,
            // placing and removing boxes are made invalid
            if (TooFarAwayFromPlayer())
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

            // Selects a placed box in the same grid coordinates as the selector
            Box placedBox = GetPlacedBoxInSelectorCoord();
            if (placedBox != null)
            {
                SelectBox(placedBox);
                return;
            }

            // Checks if there's a new box place in the same grid coordinates
            // (only if there are boxes following the player)
            if (!validRemove && boxController.MovingBoxAmount() > 0)
            {
                if (SelectorIsInReservedBoxPlace())
                {
                    UnselectAll();
                    return;
                }

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
            // TODO: Fix a bug where the selector shows
            // green while colliding with the environment

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
                        //Debug.Log("Selector is blocked");
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
        /// Sets the selector's color based on its status.
        /// </summary>
        private void ChangeColor()
        {
            //Debug.Log("playerGrounded: " + playerGrounded);
            //Debug.Log("collidesWithObstacle: " + collidesWithObstacle);

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

        //private Vector2 SetValidBoxPlaces()
        //{
        //    validBoxPlaces.Clear();

        //    // TODO
        //}

        private Vector2 ValidBoxGroundPlaceAreaPos(int extraGridCoordSide)
        {
            // The bottom left grid cell
            Vector2 bottomLeftGridCoord = playerGridCoord;
            bottomLeftGridCoord.x -= maxGroundPlaceDistX;
            bottomLeftGridCoord.y -= maxGroundPlaceDistDown;

            // Left grid coord may be different when
            // the player character is between cells
            if (extraGridCoordSide < 0)
            {
                bottomLeftGridCoord.x--;
            }

            return bottomLeftGridCoord;
        }

        private Vector2 ValidBoxGroundPlaceAreaSize(int extraGridCoordSide)
        {
            int width = 2 * maxGroundPlaceDistX + 1;
            int height = maxGroundPlaceDistDown + maxGroundPlaceDistUp + 1;

            // Width may be different when the
            // player character is between cells
            if (extraGridCoordSide != 0)
            {
                width++;
            }

            return new Vector2(width, height);
        }

        private Vector2[] BoxPlacesWithinRange(int extraGridCoordSide)
        {
            int yDiameter = 2 * maxDistFromPlayer + 1;
            int xDiameter = yDiameter;

            // Horizontal diameter may be different when
            // the player character is between cells
            if (extraGridCoordSide != 0)
            {
                xDiameter++;
            }

            Vector2[] coords =
                new Vector2[xDiameter * yDiameter];

            int minX = (int) playerGridCoord.x - maxDistFromPlayer;
            int minY = (int) playerGridCoord.y - maxDistFromPlayer;

            // Minimum x-coord may be different when
            // the player character is between cells
            if (extraGridCoordSide < 0)
            {
                minX--;
            }

            int index = 0;

            for (int y = minY; y < minY + yDiameter; y++)
            {
                for (int x = minX; x < minX + xDiameter; x++)
                {
                    if (index < coords.Length)
                    {
                        coords[index] = new Vector2(x, y);
                        //Debug.Log("index: " + index + "; x,y: " + coords[index]);
                        index++;
                    }
                }
            }

            return coords;
        }

        private void OnDrawGizmos()
        {
            if (visibility != null && visibility.enabled)
            {
                int extraGCSide = PlayerExtraGridCoordSide();

                // Draws valid box groundplace area
                DrawValidBoxGroundPlaceArea(extraGCSide);

                // Draws the maximum range of the selector
                DrawMaxRange(extraGCSide);
            }
        }

        private void DrawValidBoxGroundPlaceArea(int extraGCSide)
        {
            // Sets the color of the rectangle
            Gizmos.color = Color.white;

            // The bottom left corner of the bottom left grid cell
            Vector3 bottomLeft =
                LevelController.GetBottomLeftPosFromGridCoord(
                    ValidBoxGroundPlaceAreaPos(extraGCSide));

            // The size of the rectangle
            Vector2 size = ValidBoxGroundPlaceAreaSize(extraGCSide);

            // Draws the rectangle
            Utils.DrawGizmoRectangle(bottomLeft,
                size.x * LevelController.gridCellWidth,
                size.y * LevelController.gridCellWidth);
        }

        private void DrawMaxRange(int extraGCSide)
        {
            // Sets the color of the rectangle based
            // on what can be done with the selector
            if (BoxCanBeRemoved())
            {
                Gizmos.color = removeColor;
            }
            else if (BoxCanBePlaced())
            {
                Gizmos.color = validPlacementColor;
            }
            else
            {
                Gizmos.color = generalInvalidColor;
                //Gizmos.color = invalidPlacementColor;
            }

            // Box places within range
            Vector2[] boxPlacesWithinRange =
                BoxPlacesWithinRange(extraGCSide);

            // The corners
            Vector3 bottomLeft = LevelController.GetBottomLeftPosFromGridCoord(
                                 boxPlacesWithinRange[0]);

            // The size of the rectangle
            int vertDiameter = 2 * maxDistFromPlayer + 1;
            int horDiameter = vertDiameter;

            if (extraGCSide != 0)
            {
                horDiameter++;
            }

            // Draws the rectangle
            Utils.DrawGizmoRectangle(bottomLeft,
                horDiameter * LevelController.gridCellWidth,
                vertDiameter * LevelController.gridCellWidth);

            //Gizmos.DrawWireSphere(player.transform.position, maxDistanceFromPlayer);
        }
    }
}
