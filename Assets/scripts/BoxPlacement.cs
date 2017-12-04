using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    /// <summary>
    /// Checks the grid and determines what the box selector
    /// can do in its current position at the current moment.
    /// </summary>
    public class BoxPlacement : MonoBehaviour
    {
        [SerializeField]
        private GameObject player;

        /// <summary>
        /// A ValidBoxPlace prefab
        /// </summary>
        [SerializeField]
        private ValidBoxPlace validBoxPlacePrefab;

        [SerializeField, Range(0, 10)]
        private int maxDistFromPlayer = 5;

        [SerializeField, Range(0, 10)]
        private int maxGroundPlaceDistX = 5;

        [SerializeField, Range(0, 10)]
        private int maxGroundPlaceDistUp = 5;

        [SerializeField, Range(0, 10)]
        private int maxGroundPlaceDistDown = 5;

        [SerializeField, Range(1, 30)]
        private int maxValidBoxPlaceAmount = 15;

        private BoxController boxController;

        private float[] playerSideGridXCoords;

        private List<Box> placedBoxes;
        private List<Vector2> reservedBoxPlaceCoords;
        private List<ValidBoxPlace> validBoxPlaces;
        private List<NewBoxPlace> newBoxPlacesInLevel;

        private bool visible;

        /// <summary>
        /// Is it possible to place a box to the selector's coordinates
        /// </summary>
        private bool validPlacement;

        /// <summary>
        /// Is it possible to remove a box from the selector's coordinates
        /// </summary>
        private bool validRemove;

        private bool reservedPlacesChanged;

        private Vector3 playerSize;

        private PlayerController playerCtrl;

        //[SerializeField]
        /// <summary>
        /// A mask which covers Environment and PlacedBoxes
        /// </summary>
        private LayerMask groundMask;

        public Vector2 GridCoordinates { get; set; }

        public Vector2 PlayerGridCoord { get; private set; }

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            // Initializes the box list
            // NOTE: SetBoxController() must have been called by BoxSelector
            placedBoxes = boxController.GetPlacedBoxes();

            // Initializes the list of reserved box place coordinates
            // (used for not allowing more than one box to be placed
            // in the same node)
            reservedBoxPlaceCoords = new List<Vector2>();

            // Initializes the level new box place list
            InitNewBoxPlacesInLevel();

            // Sets the default grid coordinates
            GridCoordinates = Vector2.zero;

            validPlacement = false;
            validRemove = false;

            reservedPlacesChanged = true;

            playerSideGridXCoords = new float[2];

            if (player != null)
            {
                // Gets the player character's size
                playerSize =
                    player.GetComponent<BoxCollider2D>().bounds.size;

                // Gets the player controller
                playerCtrl = player.GetComponent<PlayerController>();
            }

            groundMask = LayerMask.GetMask("Environment", "PlacedBoxes");

            // Places where a box can be placed
            //validBoxPlaces = new List<ValidBoxPlace>();
            InitValidBoxPlaces();

            // Checks if any necessary objects are not attached
            CheckForErrors();
        }

        private void CheckForErrors()
        {
            if (player == null)
            {
                Debug.LogError
                    ("Player is not set.");
            }

            if (playerCtrl == null)
            {
                Debug.LogError
                    ("PlayerController component is not found in player.");
            }

            if (validBoxPlacePrefab == null)
            {
                Debug.LogError
                    ("ValidBoxPlace prefab is not set.");
            }
        }

        public void SetBoxController(BoxController boxCtrl)
        {
            boxController = boxCtrl;
        }

        public void ShowPlacement()
        {
            visible = true;

            ShowBoxPlaces();
        }

        public void HidePlacement()
        {
            visible = false;

            HideBoxPlaces(false);
        }


        private void ShowBoxPlaces()
        {
            // Updates valid box places and their
            // markers, then turns them visible
            UpdateValidBoxPlaces();

            // Makes all box places visible
            //if (validBoxPlaces != null)
            //{
            //    foreach (ValidBoxPlace vbp in validBoxPlaces)
            //    {
            //        vbp.IsVisible = true;
            //    }
            //}
        }

        public void HideBoxPlaces(bool hideReserved)
        {
            if (validBoxPlaces != null)
            {
                foreach (ValidBoxPlace vbp in validBoxPlaces)
                {
                    if (hideReserved || !vbp.IsReserved)
                    {
                        vbp.IsVisible = false;
                    }
                }
            }
        }

        public void HideReservedBoxPlaces()
        {
            if (validBoxPlaces != null)
            {
                foreach (ValidBoxPlace vbp in validBoxPlaces)
                {
                    if (vbp.IsReserved)
                    {
                        vbp.IsVisible = false;
                    }
                }
            }
        }

        public bool PlayerIsGrounded()
        {
            if (playerCtrl != null)
            {
                return playerCtrl.GetGrounded();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adjusts the given grid coordinates until
        /// they are within the player's range.
        /// </summary>
        /// <param name="gridCoordinates">grid coordinates</param>
        /// <returns></returns>
        public Vector2 GetClosestValidGridCoord(Vector2 gridCoordinates)
        {
            gridCoordinates = Utils.GetClosestValidGridCoord(
                gridCoordinates, PlayerGridCoord,
                maxDistFromPlayer, maxDistFromPlayer,
                PlayerExtraGridCoordSide());

            return gridCoordinates;
        }

        public void AddReservedBoxPlace()
        {
            reservedBoxPlaceCoords.Add(GridCoordinates);

            reservedPlacesChanged = true;

            //Debug.Log("Reserved space added. Spaces: " +
            //          reservedBoxPlaceCoords.Count);
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

                    reservedPlacesChanged = true;

                    return;
                }
            }
        }

        public void RemoveReservedBoxPlace(int index)
        {
            if (index >= 0 && index < reservedBoxPlaceCoords.Count)
            {
                reservedBoxPlaceCoords.RemoveAt(index);
            }
        }


        public void ClearReservedBoxPlaces()
        {
            reservedBoxPlaceCoords.Clear();
            HideReservedBoxPlaces();
            reservedPlacesChanged = true;
        }

        public void UpdateReservedBoxPlaces()
        {
            for (int i = reservedBoxPlaceCoords.Count - 1; i >= 0; i--)
            {
                // The reserved grid coordinates
                Vector2 reservedPlace = reservedBoxPlaceCoords[i];

                // If there is something in the same
                // coordinates, the reservation is removed
                if (Utils.GridCoordContainsObject(reservedPlace, groundMask))
                {
                    reservedBoxPlaceCoords.RemoveAt(i);
                    reservedPlacesChanged = true;

                    //Debug.Log("Reserved space removed. Spaces left: " +
                    //    reservedBoxPlaceCoords.Count);
                }
            }
        }

        private void UpdateReservedBoxPlaceMarkers()
        {
            if (reservedPlacesChanged)
            {
                reservedPlacesChanged = false;

                HideFilledReservedBoxPlaceMarkers();
                InitReservedBoxPlaceMarkers();
            }
        }

        private void HideFilledReservedBoxPlaceMarkers()
        {
            // This only matters when the selector is hidden
            if (!visible)
            {
                foreach (ValidBoxPlace vbp in validBoxPlaces)
                {
                    if (vbp.IsReserved)
                    {
                        foreach (Vector2 reservedCoord in
                                 reservedBoxPlaceCoords)
                        {
                            if (vbp.GridCoordinates == reservedCoord)
                            {
                                break;
                            }
                        }

                        vbp.IsVisible = false;
                    }
                }
            }
        }

        private void InitReservedBoxPlaceMarkers()
        {
            int reservedAmount = reservedBoxPlaceCoords.Count;

            for (int i = 0; i < reservedAmount; i++)
            {
                InitValidBoxPlace(i,
                    reservedBoxPlaceCoords[i],
                    false, true, true);
            }
        }

        /// <summary>
        /// Gets a selected box. The box selector uses this.
        /// </summary>
        /// <returns>a box in the current grid coordinates</returns>
        public Box GetSelectedBox()
        {
            Box placedBox = null;

            if (PlayerIsGrounded())
            {
                placedBox = GetPlacedBoxInCoord(GridCoordinates);
            }

            // If there's a placed box in the coordinates, validRemove is true
            // (NOTE: validRemove is not used in anything important;
            // instead, BoxSelector has it's own bool which is)
            validRemove = (placedBox != null);
            
            return placedBox;
        }

        private Box GetPlacedBoxInCoord(Vector2 coordinates)
        {
            foreach (Box box in placedBoxes)
            {
                if (coordinates == box.GridCoordinates)
                {
                    return box;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether a box can be placed in the current
        /// grid coordinates. The box selector uses this.
        /// </summary>
        /// <returns>can a box be placed in the
        /// current grid coordinates</returns>
        public bool PlacementIsValid()
        {
            return validPlacement && PlayerIsGrounded();
        }

        /// <summary>
        /// Changes validPlacement's value based on whether there's a
        /// valid and open box place at the current grid coordinates.
        /// </summary>
        private void UpdatePlacementValidity()
        {
            validPlacement = false;

            ValidBoxPlace vbp =
                ValidBoxPlaceInCoord(GridCoordinates);

            // If a box place exists in the selector's coordinates,
            // is visible and is not reserved, placement is made valid
            if (vbp != null && vbp.IsVisible && !vbp.IsReserved)
            {
                validPlacement = true;
            }
        }

        private ValidBoxPlace ValidBoxPlaceInCoord(Vector2 gridCoordinates)
        {
            foreach (ValidBoxPlace vbp in validBoxPlaces)
            {
                if (gridCoordinates == vbp.GridCoordinates)
                {
                    return vbp;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the game object once per frame.
        /// </summary>
        private void Update()
        {
            // Updates reserved box places and their markers
            UpdateReservedBoxPlaces();
            UpdateReservedBoxPlaceMarkers();

            if (visible)
            {
                // Updates the player character's grid coordinates
                UpdatePlayerGridCoord();

                // Updates valid box places and their markers
                UpdateValidBoxPlaces();
                //SetValidBoxPlaces();

                // Updates whether a box can be placed or not
                UpdatePlacementValidity();
            }
        }

        /// <summary>
        /// Updates the player character's grid coordinates
        /// </summary>
        public void UpdatePlayerGridCoord()
        {
            PlayerGridCoord =
                LevelController.GetGridCoordinates(
                    player.transform.position);

            // Updates the player character's
            // left and right sides' grid coordinates
            UpdatePlayerSideGridXCoords();
        }

        private void UpdatePlayerSideGridXCoords()
        {
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
        }

        private int PlayerExtraGridCoordSide()
        {
            // Defaults to middle: 0 (no extra grid coord side)
            int side = 0;

            //float[] playerSideGridXCoords = PlayerSideGridCoordXs();

            // Left: -1
            if (playerSideGridXCoords[0] < PlayerGridCoord.x)
            {
                side--;
            }
            // Right: 1
            else if (playerSideGridXCoords[1] > PlayerGridCoord.x)
            {
                side++;
            }

            return side;
        }

        private void InitValidBoxPlaces()
        {
            validBoxPlaces = new List<ValidBoxPlace>();

            for (int i = 0; i < maxValidBoxPlaceAmount; i++)
            {
                CreateValidBoxPlace(Vector2.zero, false, false);
            }

            HideBoxPlaces(true);

            //Debug.Log("ValidBoxPlaces initialized");
        }

        private void UpdateValidBoxPlaces()
        {
            // TODO: Reserved place markers will not be hidden or replaced
            // until they are filled (also, another marker will not be drawn on them)

            if (validBoxPlaces.Count == 0)
            {
                InitValidBoxPlaces();
            }
            else
            {
                HideBoxPlaces(false);
            }

            Vector2[] boxPlacesWithinRange =
                BoxPlacesWithinRange(PlayerExtraGridCoordSide());

            // The current VBP index beginning after reserved box places;
            // stops the iteration when all VBPs have been set 
            int currentVBP = reservedBoxPlaceCoords.Count;

            for (int i = 0;
                 i < boxPlacesWithinRange.Length &&
                 currentVBP < validBoxPlaces.Count;
                 i++)
            {
                // Are the coordinates on top of
                // the environment or a placed box
                bool onEnvironment = Utils.GridCoordContainsObject(
                    boxPlacesWithinRange[i], groundMask);

                bool emptyPlace =
                    !onEnvironment &&
                    !InReservedBoxPlace(boxPlacesWithinRange[i]);

                // Initializes a VBP if the coordinates
                // are not blocked by environment
                // or are already reserved
                if (emptyPlace)
                {
                    Utils.Direction dirNextToPlacedBox =
                        DirectionNextToPlacedBox(boxPlacesWithinRange[i]);

                    bool nextToPlayer = NextToPlayer(boxPlacesWithinRange[i]);

                    // Determines the VBP's configuration
                    // (when there's multiple possibilities,
                    // favors places on ground)

                    // On top of placed box
                    if (dirNextToPlacedBox == Utils.Direction.Up)
                    {
                        InitValidBoxPlace(currentVBP,
                            boxPlacesWithinRange[i],
                            true, false, dirNextToPlacedBox, true);
                        currentVBP++;
                    }
                    // On the ground next to the player character
                    else if (nextToPlayer)
                    {
                        InitValidBoxPlace(currentVBP,
                            boxPlacesWithinRange[i],
                            false, false, true);
                        currentVBP++;
                    }
                    // Next to a placed box, other than top
                    else if (dirNextToPlacedBox != Utils.Direction.None)
                    {
                        InitValidBoxPlace(currentVBP,
                            boxPlacesWithinRange[i],
                            true, false, dirNextToPlacedBox, true);
                        currentVBP++;
                    }
                    // In a level new box place
                    else if (InLevelNewBoxPlace(boxPlacesWithinRange[i]))
                    {
                        InitValidBoxPlace(currentVBP,
                            boxPlacesWithinRange[i],
                            false, false, Utils.Direction.Middle, true);
                        currentVBP++;
                    }
                }
            }
        }

        #region Deprecated: Destroying and creating VBPs
        private void DestroyValidBoxPlaces()
        {
            for (int i = validBoxPlaces.Count - 1; i >= 0; i--)
            {
                Destroy(validBoxPlaces[i].gameObject);
                validBoxPlaces.RemoveAt(i);
            }
        }

        private void SetValidBoxPlaces()
        {
            DestroyValidBoxPlaces();

            Vector2[] boxPlacesWithinRange =
                BoxPlacesWithinRange(PlayerExtraGridCoordSide());

            //int nextToPlacedBoxNum = 0;
            //int nextToPlayerNum = 0;

            foreach (Vector2 boxPlace in boxPlacesWithinRange)
            {
                // Are the coordinates on top of
                // the environment or a placed box
                bool onEnvironment =
                    Utils.GridCoordContainsObject(boxPlace, groundMask);

                if (!onEnvironment)
                {
                    Utils.Direction dirNextToPlacedBox =
                        DirectionNextToPlacedBox(boxPlace);

                    bool nextToPlayer = NextToPlayer(boxPlace);

                    // TODO: What is the best option for this?

                    // Option 1: favors places on ground
                    if (dirNextToPlacedBox == Utils.Direction.Up)
                    {
                        CreateValidBoxPlace(boxPlace, true, false, dirNextToPlacedBox);

                        //nextToPlacedBoxNum++;
                    }
                    else if (nextToPlayer)
                    {
                        CreateValidBoxPlace(boxPlace, false, false);

                        //nextToPlayerNum++;
                    }
                    else if (dirNextToPlacedBox != Utils.Direction.None)
                    {
                        CreateValidBoxPlace(boxPlace, true, false, dirNextToPlacedBox);

                        //nextToPlacedBoxNum++;
                    }

                    // Option 2: favors places next to placed boxes
                    //if (dirNextToPlacedBox != Utils.Direction.None)
                    //{
                    //    CreateValidBoxPlace(boxPlace, true, false, dirNextToPlacedBox);

                    //    //nextToPlacedBoxNum++;
                    //}
                    //else if (nextToPlayer)
                    //{
                    //    CreateValidBoxPlace(boxPlace, false, false);

                    //    //nextToPlayerNum++;
                    //}

                    // Option 3: mixed
                    //if (dirNextToPlacedBox != Utils.Direction.None)
                    //{
                    //    CreateValidBoxPlace(boxPlace, true, dirNextToPlacedBox);

                    //    if (dirNextToPlacedBox != Utils.Direction.Up &&
                    //        nextToPlayer)
                    //    {
                    ///        CreateValidBoxPlace(boxPlace, false, false);

                    //        //nextToPlayerNum++;
                    //    }

                    //    //nextToPlacedBoxNum++;
                    //}
                    //else if (nextToPlayer)
                    //{
                    //    CreateValidBoxPlace(boxPlace, false, false);

                    //    //nextToPlayerNum++;
                    //}
                }
            }

            //Debug.Log("VBPs next to placed boxes: " + nextToPlacedBoxNum);
            //Debug.Log("VBPs next to player: " + nextToPlayerNum);
            //Debug.Log("Total: " + (nextToPlacedBoxNum + nextToPlayerNum));
        }
        #endregion

        private void CreateValidBoxPlace(Vector2 gridCoordinates,
                                         bool attachedToBox,
                                         bool reservedPlace,
                                         Utils.Direction dir =
                                             Utils.Direction.Up)
        {
            ValidBoxPlace vbp = Instantiate(validBoxPlacePrefab);
            vbp.Set(gridCoordinates, attachedToBox, reservedPlace, dir);
            validBoxPlaces.Add(vbp);
        }

        private void InitValidBoxPlace(int index, Vector2 gridCoord,
            bool attachedToBox, bool reservedPlace, Utils.Direction dir, bool visible)
        {
            validBoxPlaces[index].Set(gridCoord, attachedToBox, reservedPlace, dir);
            validBoxPlaces[index].IsVisible = visible;
        }

        private void InitValidBoxPlace(int index, Vector2 gridCoord,
            bool attachedToBox, bool reservedPlace, bool visible)
        {
            validBoxPlaces[index].Set(gridCoord, attachedToBox, reservedPlace);
            validBoxPlaces[index].IsVisible = visible;
        }

        /// <summary>
        /// Checks if the given grid coordinates are 
        /// outside of the range around the player character.
        /// </summary>
        /// <returns>are the grid coordinates too far away
        /// from the player</returns>
        private bool TooFarAwayFromPlayer_Coord(Vector2 gridCoordinates)
        {
            bool horizontalOK = XCoordIsWithinPlayerRange(gridCoordinates.x);
            bool verticalOK = YCoordIsWithinPlayerRange(gridCoordinates.y);

            return (!horizontalOK || !verticalOK);
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

        private bool InReservedBoxPlace(Vector2 gridCoordinates)
        {
            // Goes through the reserved box place list and checks
            // if any of them is in the same grid coordinates
            foreach (Vector2 reservedPlace in reservedBoxPlaceCoords)
            {
                // If the reserved box place is in the
                // same coordinates, true is returned
                if (gridCoordinates == reservedPlace)
                {
                    return true;
                }
            }

            // There is no reserved box place in the coordinates
            return false;
        }

        private bool InLevelNewBoxPlace(Vector2 gridCoordinates)
        {
            // Goes through the level nbp list and checks
            // if any of them is in the same grid coordinates
            foreach (NewBoxPlace newBoxPlace in newBoxPlacesInLevel)
            {
                // If the nbp is in the same
                // coordinates, true is returned
                if (gridCoordinates == newBoxPlace.GridCoordinates)
                {
                    return true;
                }
            }

            // There is no level nbp in the coordinates
            return false;
        }

        private bool NextToPlacedBox(Vector2 gridCoordinates)
        {
            return DirectionNextToPlacedBox(gridCoordinates)
                != Utils.Direction.None;
        }

        private Utils.Direction DirectionNextToPlacedBox(Vector2 gridCoordinates)
        {
            Utils.Direction direction = Utils.Direction.None;

            foreach (Box box in placedBoxes)
            {
                bool horNextTo = Mathf.Abs(gridCoordinates.x - box.GridCoordinates.x) == 1;
                bool vertNextTo = Mathf.Abs(gridCoordinates.y - box.GridCoordinates.y) == 1;

                bool horTouch = horNextTo && gridCoordinates.y == box.GridCoordinates.y;
                bool vertTouch = vertNextTo && gridCoordinates.x == box.GridCoordinates.x;

                if (horTouch)
                {
                    if (gridCoordinates.x < box.GridCoordinates.x)
                    {
                        direction = Utils.Direction.Left;
                    }
                    else
                    {
                        direction = Utils.Direction.Right;
                    }
                    
                    break;
                }
                else if (vertTouch)
                {
                    if (gridCoordinates.y < box.GridCoordinates.y)
                    {
                        direction = Utils.Direction.Down;
                    }
                    else
                    {
                        direction = Utils.Direction.Up;
                    }

                    break;
                }
            }

            return direction;
        }

        private bool NextToPlayer(Vector2 gridCoordinates)
        {
            //float[] playerSideGridCoordXs = PlayerSideGridCoordXs();

            bool horizontalOK = 
                XCoordIsWithinRange(gridCoordinates.x,
                                    playerSideGridXCoords[0],
                                    playerSideGridXCoords[1]);
            bool verticalOK = YCoordIsWithinRange(gridCoordinates.y,
                                                  PlayerGridCoord.y);

            if (horizontalOK && verticalOK)
            {
                // Uses raycast to determine if the
                // coordinates are on top of solid ground
                Vector3 center =
                    LevelController.GetPosFromGridCoord(gridCoordinates);
                RaycastHit2D grounded =
                    Physics2D.Raycast(center, Vector2.down, LevelController.gridCellWidth, groundMask);

                if (grounded)
                {
                    return true;
                }
            }

            return false;
        }

        private bool SelectorIsWithinRange(float leftCoordX,
                                             float rightCoordX)
        {
            return XCoordIsWithinRange(GridCoordinates.x, leftCoordX, rightCoordX);
        }

        private bool SelectorXIsWithinRange(float leftCoordX,
                                             float rightCoordX)
        {
            return XCoordIsWithinRange(GridCoordinates.x, leftCoordX, rightCoordX);
        }

        private bool SelectorYIsWithinRange(float otherGridCoordY)
        {
            return YCoordIsWithinRange(GridCoordinates.y, otherGridCoordY);
        }

        /// <summary>
        /// Checks if the given x-coordinate is within
        /// range of one of the other x-coordinates.
        /// </summary>
        /// <param name="coordX">an x-coordinate</param>
        /// /// <param name="leftCoordX">x-coordinate of an object's
        /// left side</param>
        /// <param name="rightCoordX">x-coordinate of an object's
        /// right side</param>
        /// <returns>is the x-coordinate within range</returns>
        private bool XCoordIsWithinRange(float coordX,
                                         float leftCoordX,
                                         float rightCoordX)
        {
            // Checks if the selector is not too far from the left x-coord
            bool leftOK = (int) Mathf.Abs(leftCoordX - coordX)
                <= maxGroundPlaceDistX;

            // Checks if the selector is not too far from the right x-coord
            bool rightOK = (int) Mathf.Abs(rightCoordX - coordX)
                <= maxGroundPlaceDistX;

            // Returns whether either of the checks was true
            return (leftOK || rightOK);
        }

        private bool YCoordIsWithinRange(float coordY, float otherCoordY)
        {
            // The coord is above or at the
            // same level as the other coord
            if (coordY >= otherCoordY)
            {
                return ((int) (coordY - otherCoordY)
                        <= maxGroundPlaceDistUp);
            }
            // The coord is below the other coord
            else
            {
                return ((int) (otherCoordY - coordY)
                        <= maxGroundPlaceDistDown);
            }
        }


        /// <summary>
        /// Checks if the given x-coordinate is within
        /// range of one of the other x-coordinates.
        /// </summary>
        /// <param name="coordX">an x-coordinate</param>
        /// /// <param name="playerLeftCoordX">x-coordinate of an object's
        /// left side</param>
        /// <param name="playerRightCoordX">x-coordinate of an object's
        /// right side</param>
        /// <returns>is the x-coordinate within range</returns>
        private bool XCoordIsWithinPlayerRange(float coordX)
        {
            // Checks if the selector is not too far from the left x-coord
            bool leftOK = (int) Mathf.Abs(playerSideGridXCoords[0] - coordX)
                <= maxDistFromPlayer;

            // Checks if the selector is not too far from the right x-coord
            bool rightOK = (int) Mathf.Abs(playerSideGridXCoords[1] - coordX)
                <= maxDistFromPlayer;

            // Returns whether either of the checks was true
            return (leftOK || rightOK);
        }

        private bool YCoordIsWithinPlayerRange(float coordY)
        {
            return Mathf.Abs(PlayerGridCoord.y - coordY) <= maxDistFromPlayer;
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

            int minX = (int) PlayerGridCoord.x - maxDistFromPlayer;
            int minY = (int) PlayerGridCoord.y - maxDistFromPlayer;

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

        private Vector2 ValidBoxGroundPlaceAreaPos(int extraGridCoordSide)
        {
            // The bottom left grid cell
            Vector2 bottomLeftGridCoord = PlayerGridCoord;
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

        private void OnDrawGizmos()
        {
            if (visible)
            {
                int extraGCSide = PlayerExtraGridCoordSide();

                // Draws valid box groundplace area
                DrawValidBoxGroundPlaceArea(extraGCSide);

                // Draws the maximum range of the selector
                DrawMaxRange(extraGCSide);

                // Draws valid box places (testing)
                //DrawValidBoxPlaceMarkers();
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
            if (validRemove)
            {
                Gizmos.color = Color.yellow;
            }
            else if (validPlacement)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
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

        private void DrawValidBoxPlaceMarkers()
        {
            if (validBoxPlaces != null)
            {
                // The size
                float radius = LevelController.gridCellWidth / 2;

                // Draws a marker on each valid box place
                foreach (ValidBoxPlace vbp in validBoxPlaces)
                {
                    // Sets the position
                    Vector3 position =
                        LevelController.GetPosFromGridCoord(
                            vbp.GridCoordinates);

                    // Sets the color
                    if (GridCoordinates == vbp.GridCoordinates)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else if (vbp.IsAttachedToBox)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        // The default color
                        Gizmos.color = Color.cyan;
                    }

                    // Draws the marker
                    Gizmos.DrawWireSphere(position, radius);
                }
            }
        }
    }
}
