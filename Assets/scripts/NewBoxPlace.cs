using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

namespace not_broforce
{
    public class NewBoxPlace : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private Vector2 gridCoordinates;

        [SerializeField]
        private LevelController levelController;

        [SerializeField]
        private Parent owner;

        public enum Parent { Environment, Box, Player };

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            if (levelController != null)
            {
                // Sets the grid coordinates
                //gridCoordinates = Vector2.zero;
                //if (owner == Owner.Environment)
                //{
                //    // Gets the grid coordinates from the current position
                //    gridCoordinates = Utils.GetGridCoordinates(
                //        transform.position,
                //        levelController.GridCellWidth,
                //        levelController.GridOffset);

                //    // Sets the position to the center of the grid cell
                //    transform.position = Utils.GetPosFromGridCoord(
                //        gridCoordinates,
                //        levelController.GridCellWidth,
                //        levelController.GridOffset);
                //}

                // Sets everything relating to the player 
                //if (owner == Owner.Player)
                //{
                //    player = FindObjectOfType<PlayerController>();
                //    side = Utils.Direction.Right;
                //}
            }
        }

        public NewBoxPlace(Vector2 gridCoordinates,
                           LevelController levelController,
                           Parent owner)
        {
            Initialize(gridCoordinates, levelController, owner);
        }

        public void Initialize(Vector2 gridCoordinates,
                               LevelController levelController,
                               Parent owner)
        {
            this.gridCoordinates = gridCoordinates;
            this.levelController = levelController;
            this.owner = owner;
        }

        //public bool Usable
        //{
        //    get { return usable; }
        //}

        public Parent Owner
        {
            get { return owner; }
        }

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set
            {
                gridCoordinates = value;
                MoveToGridCoordinates();
            }
        }

        //public Box ParentBox
        //{
        //    get
        //    {
        //        if (owner == Owner.Box)
        //        {
        //            return box.GetComponent<Box>();
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            if (levelController != null)
            {
                //if (owner == Owner.Player)
                //{
                //    UpdatePositionNextToPlayer();
                //}
                //else if (owner == Owner.Box)
                //{
                //    UpdatePositionNextToBox();
                //}
            }
        }

        public void MoveToGridCoordinates()
        {
            transform.position = Utils.GetPosFromGridCoord(
                gridCoordinates,
                levelController.GridCellWidth,
                levelController.GridOffset);

            // Prints debug info
            //Debug.Log("NewBoxPlace - New grid coordinates: " + gridCoordinates);
        }

        //private void UpdatePositionNextToPlayer()
        //{
        //    // Calculates the ground level based on the player character's position
        //    float groundY = player.transform.position.y -
        //        player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        //    // Sets the side based the player character's looking direction
        //    side = player.GetComponent<SpriteRenderer>().flipX ? Utils.Direction.Left : Utils.Direction.Right;

        //    // Calculates the x-coordinate
        //    float x = (player.transform.position + Utils.DirectionToVector3(side) / 2).x;

        //    // Sets the position
        //    transform.position = new Vector3(x, groundY + levelController.GridCellWidth / 2);
        //}

        //private void UpdatePositionNextToBox()
        //{
        //    Vector3 boxPosition = box.transform.position;

        //    switch (side)
        //    {
        //        case Utils.Direction.Up:
        //            {
        //                transform.position = boxPosition + new Vector3(0, levelController.GridCellWidth);
        //                break;
        //            }
        //        case Utils.Direction.Down:
        //            {
        //                transform.position = boxPosition + new Vector3(0, -1 * levelController.GridCellWidth);
        //                break;
        //            }
        //        case Utils.Direction.Left:
        //            {
        //                transform.position = boxPosition + new Vector3(-1 * levelController.GridCellWidth, 0);
        //                break;
        //            }
        //        case Utils.Direction.Right:
        //            {
        //                transform.position = boxPosition + new Vector3(levelController.GridCellWidth, 0);
        //                break;
        //            }
        //    }


        //    // TODO: NewBoxPlace updates its grid coordinates when the parent box is placed

        //    //if (box._takingPosition)
        //    //{
        //    //    GridCoordinates = box.GridCoordinates + side;
        //    //    usable = true;
        //    //}
        //}

        /// <summary>
        /// Draws a box in the grid cell.
        /// </summary>
        private void OnDrawGizmos()
        {
            // The bottom left corner of the grid cell
            Vector3 bottomLeft =
                Utils.GetBottomLeftPosFromGridCoord(
                    gridCoordinates,
                    levelController.GridCellWidth,
                    levelController.GridOffset);

            Vector3 point;
            Vector3 point2;
            Gizmos.color = Color.blue;

            // Left
            point = bottomLeft + Vector3.up * levelController.GridCellWidth;
            Gizmos.DrawLine(bottomLeft, point);

            // Top
            point2 = point + Vector3.right * levelController.GridCellWidth;
            Gizmos.DrawLine(point, point2);

            // Right
            point = point2 + Vector3.down * levelController.GridCellWidth;
            Gizmos.DrawLine(point2, point);

            // Bottom
            point2 = point + Vector3.left * levelController.GridCellWidth;
            Gizmos.DrawLine(point, point2);
        }
    }

    //[CustomEditor(typeof(NewBoxPlace))]
    //public class NewBoxPlaceEditor : Editor
    //{
    //    override public void OnInspectorGUI()
    //    {
    //        var newBoxPlace = target as NewBoxPlace;

    //        int selected = 0;
    //        string[] options = new string[]
    //        {
    //            "Box", "Environment", "Player"
    //        };

    //        selected = EditorGUILayout.Popup("Box", selected, options);

    //        using (var group = new EditorGUILayout.FadeGroupScope(selected))
    //        {
    //            if (!group.visible)
    //            {
    //                newBoxPlace.box = EditorGUILayout.IntSlider("I field:", newBoxPlace.i, 1, 100);
    //            }
    //        }
    //    }
    //}
}
