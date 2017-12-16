using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class BoxSelector : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private BoxController boxController;

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

        private BoxPlacement placement;

        private Box selectedBox;

        private bool boxesTeleporting;
        private float boxTelStartTime;
        private float boxTelDuration;

        /// <summary>
        /// The sprite renderer of the object.
        /// Needed to hide the object while still
        /// updating it and to change its color.
        /// </summary>
        private SpriteRenderer sr;

        private Vector2 gridCoordinates;

        /// <summary>
        /// Is it possible to place a box to the selector's coordinates
        /// </summary>
        private bool validPlacement;

        /// <summary>
        /// Is it possible to remove a box from the selector's coordinates
        /// </summary>
        private bool validRemove;

        /// <summary>
        /// Is the player character standing on the ground
        /// </summary>
        //private bool playerGrounded;

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
            // Initializes box placement
            InitPlacement();

            // Initializes the sprite renderer
            sr = GetComponent<SpriteRenderer>();
            HideSelector(forceHide: false);

            // Sets the selector's size
            SetSize();

            // Initializes functionality
            validPlacement = false;
            validRemove = false;

            // Initializes box teleport time
            boxTelDuration = 1.2f;

            // Checks if any necessary objects are not attached
            CheckForErrors();
        }

        private void InitPlacement()
        {
            placement = GetComponent<BoxPlacement>();
            placement.SetBoxController(boxController);
        }

        /// <summary>
        /// Gets whether the selector usable at its current state.
        /// Returns true if the selector is visible.
        /// </summary>
        /// <returns>is the selector usable at its current state</returns>
        private bool IsUsable()
        {
            return sr.enabled;
        }

        private bool BoxCanBePlaced()
        {
            return (IsUsable() && validPlacement);
        }

        private bool BoxCanBeRemoved()
        {
            return (IsUsable() && validRemove);
        }

        private void CheckForErrors()
        {
            if (boxController == null)
            {
                Debug.LogError
                    ("BoxController is not set.");
            }

            if (cursor == null)
            {
                Debug.LogError
                    ("Mouse cursor is not set.");
            }
        }

        private void SetSize()
        {
            Vector3 gridScale =
                LevelController.GetGridScale(sr.bounds.size);

            transform.localScale = new Vector3(transform.localScale.x * gridScale.x,
                                               transform.localScale.y * gridScale.y);
        }

        public void Activate()
        {
            // Shows the selector
            if (!sr.enabled)
            {
                ShowSelector();
            }
        }

        public void Deactivate(bool forceHide)
        {
            // Hides the selector
            if (sr.enabled)
            {
                if (forceHide)
                {
                    ShowAlways(false);
                }

                HideSelector(forceHide);
            }
        }

        public void ToggleActivation()
        {
            if (!isAlwaysShown)
            {
                // Displays or hides the selector
                sr.enabled = !sr.enabled;

                // If the selector was made visible, its position is set next to
                // the player character and placement validity is checked
                if (sr.enabled)
                {
                    ShowSelector();
                }
                // Otherwise any selected box is unselected
                else
                {
                    HideSelector(forceHide: false);
                }
            }
        }

        public void InitBoxTeleport()
        {
            boxesTeleporting = true;
            boxTelStartTime = Time.time;
        }

        private void UpdateBoxTeleport()
        {
            if (boxesTeleporting)
            {
                if (Time.time - boxTelStartTime >= boxTelDuration)
                {
                    boxesTeleporting = false;
                }
            }
        }

        private void ShowSelector()
        {
            // Updates the player character's grid coordinates
            placement.UpdatePlayerGridCoord();

            // If the game is played using the
            // mouse, the selector is moved under
            // the mouse cursor which is hidden
            if (cursor.PlayingUsingMouse)
            {
                MouseMovevent();
                //cursor.Visible = false;
            }
            // Otherwise the selector is moved to
            // the player character's coordinates
            else
            {
                PlaceSelectorNextToPlayer();
            }

            // Makes the selector visible
            sr.enabled = true;

            // Shows valid box places
            placement.ShowPlacement();
        }

        private void HideSelector(bool forceHide)
        {
            if (!isAlwaysShown || forceHide)
            {
                sr.enabled = false;
                InvalidateAll();
                placement.HidePlacement();
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

        /// <summary>
        /// Updates the game object once per frame.
        /// </summary>
        private void Update()
        {
            if (sr.enabled)
            {
                // Updates the selector's position if
                // the game is played using the mouse
                if (cursor.PlayingUsingMouse)
                {
                    MouseMovevent();
                }

                UpdateSelection();
            }
            else if (isAlwaysShown)
            {
                ShowSelector();
            }

            UpdateBoxTeleport();
        }

        /// <summary>
        /// Moves the selector to the saved grid coordinates.
        /// </summary>
        public void MoveToGridCoordinates()
        {
            transform.position =
                LevelController.GetPosFromGridCoord(gridCoordinates);

            placement.GridCoordinates = gridCoordinates;

            InvalidateAll();
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
            gridCoordinates = placement.PlayerGridCoord;

            // Moves the selector to the grid coordinates
            MoveToGridCoordinates();
        }

        /// <summary>
        /// Moves the selector to where the cursor is while snapping to a grid.
        /// </summary>
        private void MouseMovevent()
        {
            // Cannot move when paused
            if (Time.timeScale > 0f)
            {
                Vector2 newGridCoordinates =
                    LevelController.GetGridCoordinates(cursor.Position);

                // If the new grid coordinates are different to the
                // old ones, the selector is moved there or to the
                // closest possible coordinates
                if (newGridCoordinates != gridCoordinates)
                {
                    // Moves the selector to the closest valid grid coordinates
                    gridCoordinates = placement.
                        GetClosestValidGridCoord(newGridCoordinates);

                    //gridCoordinates = Utils.GetClosestValidGridCoord(
                    //    newGridCoordinates, playerGridCoord,
                    //    maxDistFromPlayer, maxDistFromPlayer,
                    //    PlayerExtraGridCoordSide());

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
            // The movement to any of the four cardinal directions
            Vector2 movement =
                Utils.GetAdjacentGridCoord(Vector2.zero, direction);

            // Moves the selector to the closest valid grid coordinates
            gridCoordinates = placement.
                GetClosestValidGridCoord(gridCoordinates + movement);

            //gridCoordinates = Utils.GetClosestValidGridCoord(
            //    gridCoordinates + movement,
            //    playerGridCoord,
            //    maxDistFromPlayer, maxDistFromPlayer,
            //    PlayerExtraGridCoordSide());

            // Moves the selector to the coordinates
            MoveToGridCoordinates();
        }

        public void OnPlacementInputDown()
        {
            // Only accepts input for the selector if it is visible
            if (sr.enabled)
            {
                if (BoxCanBeRemoved())
                {
                    RemoveBox();
                }
                else if (BoxCanBePlaced())
                {
                    PlaceBox();
                }
            }
        }

        private void PlaceBox()
        {
            bool placed = boxController.PlaceBox(transform.position);

            if (placed)
            {
                placement.AddReservedBoxPlace();

                // Plays a sound
                SFXPlayer.Instance.Play(Sound.Laser3);
            }
        }

        private void RemoveBox()
        {
            boxController.RemovePlacedBox();
            UnselectBox();

            // Plays a sound
            SFXPlayer.Instance.Play(Sound.Laser4);
        }

        private void SelectBox(Box box)
        {
            if (box != selectedBox)
            {
                validPlacement = false;
                validRemove = true;

                boxController.CheckRemovingBoxes(box);

                // Prints debug info
                //Debug.Log("Box selected");
            }
        }

        private void UnselectBox()
        {
            if (validRemove)
            {
                selectedBox = null;
                validRemove = false;

                boxController.ClearRemovingBoxes();

                // Prints debug info
                //Debug.Log("Box unselected");
            }
        }

        public void RefreshSelectedBox()
        {
            Box temp = selectedBox;
            selectedBox = null;
            boxController.ClearRemovingBoxes();
            SelectBox(temp);
        }

        private void ValidatePlacement()
        {
            if (!validPlacement)
            {
                selectedBox = null;
                validPlacement = true;
                validRemove = false;

                // Prints debug info
                //Debug.Log("New box place selected");
            }
        }

        private void InvalidatePlacement()
        {
            if (validPlacement)
            {
                validPlacement = false;

                //ChangeColor();

                // Prints debug info
                //Debug.Log("NewBoxPlace unselected");
            }
        }

        /// <summary>
        /// Unselects a possible selected box or a new box place.
        /// </summary>
        private void InvalidateAll()
        {
            UnselectBox();
            InvalidatePlacement();
        }

        public void RemoveAllBoxes()
        {
            InvalidateAll();
            placement.ClearReservedBoxPlaces();
        }

        public void RemoveReservedBoxPlace(Vector3 position)
        {
            placement.RemoveReservedBoxPlace(position);
        }

        /// <summary>
        /// Checks if a box can be placed to the selector's position.
        /// </summary>
        /// <returns>can a box be placed to the selector's position</returns>
        private void UpdateSelection()
        {
            // If the selector is in a usable state, it's 
            // checked what is in the current grid coordinates
            if (IsUsable() && !boxesTeleporting)
            {
                CheckSelection();
            }

            // Sets the selector's color based on its status
            ChangeColor();
        }

        /// <summary>
        /// Checks the content of the current grid coordinates.
        /// If there's a placed box or a new box place, it is selected.
        /// </summary>
        private void CheckSelection()
        {
            // TODO: Use the 'moved' bool to limit unnecessary checks
            // when no boxes are being placed or removed.

            // Selects a placed box
            Box box = placement.GetSelectedBox();
            if (box != null)
            {
                SelectBox(box);
            }
            else // if (placement.ValidRemoveOutOfRange())
            {
                UnselectBox();

                // If there are boxes following the player,
                // checks if it's possible to place a box
                // in the grid coordinates
                if (boxController.MovingBoxAmount() > 0)
                {
                    // Validates placement 
                    if (placement.PlacementIsValid())
                    {
                        ValidatePlacement();
                    }

                    // Otherwise the selector cannot be used
                    else
                    {
                        InvalidateAll();
                    }
                }
            }


            /*
            // Selects a placed box in the same grid coordinates as the selector
            Box placedBox = PlacedBoxInSelectorCoord();
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
                    InvalidateAll();
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
            InvalidateAll();
            */
        }

        /// <summary>
        /// Sets the selector's color based on its status.
        /// </summary>
        private void ChangeColor()
        {
            if (validRemove)
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
