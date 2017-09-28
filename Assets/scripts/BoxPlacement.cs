using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class BoxPlacement : MonoBehaviour
    {
        private const int Z_AXIS = 0;

        [SerializeField]
        private GameObject referenceBox;

        [SerializeField]
        private GameObject player;

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
        /// hide the object but still handle input.
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
        /// Was the selector moved
        /// </summary>
        private bool moved;


        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            // Gets the player character
            //player = GameObject.FindGameObjectWithTag("Player").transform;

            if (referenceBox == null)
            {
                throw new System.Exception("Box not set to the selector.");
            }

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

            // Only accepts input for the selector if it is visible
            else if (visibility.enabled)
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

        private void ShowSelector()
        {
            if (!visibility.enabled)
            {
                visibility.enabled = true;
            }

            // Calculates the ground level based on the player character's position
            float groundY = player.transform.position.y -
                player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            // Gets the player character's looking direction
            bool lookingLeft = player.GetComponent<SpriteRenderer>().flipX;

            transform.position = player.transform.position + (lookingLeft ? Vector3.left : Vector3.right) / 2;
            transform.position = new Vector3(transform.position.x, groundY + boxSize.y / 2, Z_AXIS);

            snapsToBoxGrid = false;
            moved = true;
        }

        private void HideSelector()
        {
            if (visibility.enabled)
            {
                visibility.enabled = false;
            }

            moved = false;
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
            snapsToBoxGrid = true;
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
                HideSelector();
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
                    // Goes through each existing box
                    foreach (GameObject box in boxes)
                    {
                        // Checks if the selector is exactly on top of an existing box
                        // and if so, makes that box selected
                        if (transform.position == box.transform.position)
                        {
                            SelectBox(box);

                            break;
                        }
                        // Checks if the selector intersects with an
                        // existing box and if so, makes that box selected
                        // (Note: the amount of overlap needed
                        // can be controlled with the number)
                        else if (Utils.CollidersIntersect(
                            GetComponent<BoxCollider2D>(),
                            box.GetComponent<BoxCollider2D>(),
                            0.9f))
                        {
                            transform.position = box.transform.position;
                            SelectBox(box);

                            break;
                        }
                    }
                }
                // The selector can be moved smoothly along the ground
                else
                {
                    // Checks if the selector intersects the ground or the player
                    // character and if so, sets placement validity false

                    // TODO

                    // Goes through each existing box
                    foreach (GameObject box in boxes)
                    {
                        // Checks if the selector intersects with an
                        // existing box and if so, makes that box selected
                        // (Note: the amount of overlap needed
                        // can be controlled with the number)
                        if (Utils.CollidersIntersect(
                            GetComponent<BoxCollider2D>(),
                            box.GetComponent<BoxCollider2D>(),
                            1f))
                        {
                            transform.position = box.transform.position;
                            SelectBox(box);

                            break;
                        }
                    }
                }
            }
            // If removal is possible, a box can not be placed
            else if (!validRemove)
            {
                validPlacement = true;
            }

            ChangeColor();
        }

        /// <summary>
        /// Sets the selector's color
        /// </summary>
        private void ChangeColor()
        {
            if (validRemove)
            {
                GetComponent<SpriteRenderer>().color = removeColor;
            }
            else if (validPlacement)
            {
                GetComponent<SpriteRenderer>().color = validPlacementColor;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = invalidPlacementColor;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
}
