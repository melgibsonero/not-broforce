using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class BoxSelector : MonoBehaviour
    {
        private const int Z_AXIS = 0;

        [SerializeField]
        private GameObject referenceBox;

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
        private float smoothMovingSpeed;

        [SerializeField]
        private float maxDistanceFromPlayer = 2;

        /// <summary>
        /// The renderer of the object. Needed to
        /// hide the object but still update it.
        /// </summary>
        private Renderer visibility;

        //private Transform player;
        private List<GameObject> boxes;
        private GameObject selectedBox;
        private Vector2 boxSize;

        /// <summary>
        /// Is it possible to place a box to the position of the selector
        /// </summary>
        private bool validPlacement;

        /// <summary>
        /// Is it possible to remove a box from the position of the selector
        /// </summary>
        private bool validRemove;

        /// <summary>
        /// Does the selector snap to a grid of boxes
        /// </summary>
        private bool snapsToBoxGrid;

        /// <summary>
        /// Was the selector moved during the frame
        /// </summary>
        private bool moved;

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            // Gets the player character
            //player = GameObject.FindGameObjectWithTag("Player").transform;

            // Checks if any necessary objects are not attached
            CheckForMissingObjects();

            // Sets the box list
            boxes = new List<GameObject>();

            // Finds all existing boxes in the level
            FindBoxesInLevel();

            // Finds all existing boxes in the level
            // and adds them to the box list
            SetSize();

            visibility = GetComponent<Renderer>();
            visibility.enabled = false;

            validPlacement = true;
            validRemove = false;
            snapsToBoxGrid = false;
            moved = false;
        }

        private void CheckForMissingObjects()
        {
            if (referenceBox == null)
            {
                throw new System.NullReferenceException
                    ("Box not set to the selector.");
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

        /// <summary>
        /// Gets the boxes' size and sets the selector to the same size as the box.
        /// </summary>
        private void SetSize()
        {
            // Gets the size of the box
            boxSize = referenceBox.GetComponent<SpriteRenderer>().bounds.size;

            float scaleX =
                boxSize.x / GetComponent<SpriteRenderer>().bounds.size.x;
            float scaleY =
                boxSize.y / GetComponent<SpriteRenderer>().bounds.size.y;

            transform.localScale = new Vector3(transform.localScale.x * scaleX,
                                                transform.localScale.y * scaleY, Z_AXIS);
        }

        /// <summary>
        /// Finds all existing boxes in the level and adds them to the box list.
        /// </summary>
        private void FindBoxesInLevel()
        {
            GameObject[] existingBoxes = GameObject.FindGameObjectsWithTag("crate");

            foreach (GameObject box in existingBoxes)
            {
                boxes.Add(box);
            }
        }

        /// <summary>
        /// Updates the game object once per frame.
        /// </summary>
        private void Update()
        {
            HandleInput();

            if (visibility.enabled)
            {
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

        private void MouseMovevent()
        {
            transform.position = cursor.Position;
            moved = true;

            if (snapsToBoxGrid)
            {
                snapsToBoxGrid = false;
            }
        }

        private void DirectionalMovevent()
        {
            moved = false;

            // Checks input for moving the selector
            // (snapping to a box grid)
            if (snapsToBoxGrid)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    transform.position +=
                        new Vector3(0, 1 * boxSize.y);

                    moved = true;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    transform.position +=
                        new Vector3(0, -1 * boxSize.y);

                    moved = true;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.position +=
                        new Vector3(-1 * boxSize.x, 0);

                    moved = true;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.position +=
                        new Vector3(1 * boxSize.x, 0);

                    moved = true;
                }
            }
            // Checks input for moving the selector
            // (smooth horizontal movement)
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.position += Vector3.left *
                        smoothMovingSpeed * Time.deltaTime;

                    moved = true;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.position += Vector3.right *
                        smoothMovingSpeed * Time.deltaTime;

                    moved = true;
                }
            }
        }

        private void ShowSelector()
        {
            if (!visibility.enabled)
            {
                visibility.enabled = true;
            }

            if (!cursor.Visible)
            {
                // Calculates the ground level based on the player character's position
                float groundY = player.transform.position.y -
                    player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

                // Gets the player character's looking direction
                bool lookingLeft = player.GetComponent<SpriteRenderer>().flipX;

                // Calculates the x-coordinate
                float x = (player.transform.position + (lookingLeft ? Vector3.left : Vector3.right) / 2).x;

                // Sets the selector's position
                transform.position = new Vector3(x, groundY + boxSize.y / 2, Z_AXIS);
            }

            moved = true;
        }

        private void HideSelector()
        {
            if (visibility.enabled)
            {
                visibility.enabled = false;
            }

            moved = false;
            snapsToBoxGrid = false;
            UnselectBox();
        }

        private void PlaceBox()
        {
            if (validPlacement)
            {
                // Creates a new instance of the box
                GameObject newBox = Instantiate(referenceBox);

                // Sets the box's position
                newBox.transform.position = transform.position;

                // Sets the box's other features
                newBox.GetComponent<Rigidbody2D>().simulated = true;
                newBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                newBox.GetComponent<BoxCollider2D>().enabled = true;
                newBox.GetComponent<SpriteRenderer>().color = Color.white;

                // Adds the box to the box list
                boxes.Add(newBox);

                // Makes the newly added box selected
                SelectBox(newBox);
            }
        }

        private void RemoveBox()
        {
            if (validRemove)
            {
                boxes.Remove(selectedBox);
                Destroy(selectedBox);
                UnselectBox();
            }
        }

        private void SelectBox(GameObject box)
        {
            selectedBox = box;
            validPlacement = false;
            validRemove = true;

            if (!cursor.Visible)
            {
                snapsToBoxGrid = true;
            }

            ChangeColor();
        }

        private void UnselectBox()
        {
            selectedBox = null;
            validPlacement = true;
            validRemove = false;
            ChangeColor();
        }

        private void InvalidateAll()
        {
            validPlacement = false;
            validRemove = false;
        }

        /// <summary>
        /// Checks if a box can be placed to the selector's position.
        /// </summary>
        /// <returns>can a box be placed to the selector's position</returns>
        private void CheckPlacementValidity()
        {
            if (Utils.Distance(transform.position, player.transform.position) > maxDistanceFromPlayer)
            {
                InvalidateAll();
            }
            // If the selector intersects with the player
            // character, sets box placement validity false
            else if (Utils.CollidersIntersect(GetComponent<BoxCollider2D>(),
                player.GetComponent<BoxCollider2D>()))
            {
                InvalidateAll();
            }
            else if (moved)
            {
                // Sets the default value (true) for the box placement validity
                validPlacement = true;

                // Sets the default value (false) for the box removal validity
                validRemove = false;

                // The selector snaps to a box grid
                if (snapsToBoxGrid)
                {
                    // The modifier is less than 1 because otherwise
                    // the cursor would be stuck on a box
                    CheckIfIntersectsWithBox(0.9f);
                }
                // The selector can be moved smoothly along the ground
                else
                {
                    // TODO:
                    // Checks if the selector intersects the
                    // ground or a wall and if so,
                    // sets placement validity false


                    CheckIfIntersectsWithBox(1f);
                }
            }
            // The selector was too far away but not anymore
            // -> placement is valid
            else if (!validRemove && !validPlacement)
            {
                // The modifier is less than 1 because otherwise
                // the cursor would be stuck on a box
                if ( !CheckIfIntersectsWithBox(0.9f) )
                {
                    validPlacement = true;
                }
            }
            // A box can be placed if there's nothing
            // on the way and removal is not possible
            else if (!validRemove)
            {
                validPlacement = true;
            }

            // Sets the selector's color based on its status
            ChangeColor();
        }

        /// <summary>
        /// Goes through the box list and selects the first box that
        /// intersects with the selector. The amount of overlap needed
        /// can be controlled with the modifier.
        /// </summary>
        /// <param name="modifier">the required overlap modifier
        /// (1 = any overlap)</param>
        /// <returns></returns>
        private bool CheckIfIntersectsWithBox(float modifier)
        {
            // TODO: A "possible position" on each side of a placed box;
            // They affect the selector whether it is moved with the cursor or not

            // Goes through each existing box
            foreach (GameObject box in boxes)
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
                    transform.position = box.transform.position;
                    SelectBox(box);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the selector's color based on its status
        /// </summary>
        private void ChangeColor()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

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

        /// <summary>
        /// Draws a line from the selector's position
        /// to the player character's position.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
}
