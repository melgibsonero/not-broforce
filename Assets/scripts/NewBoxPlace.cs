using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

namespace not_broforce
{
    public class NewBoxPlace : MonoBehaviour
    {
        [SerializeField]
        private LevelController levelController;

        [SerializeField]
        private Owner owner;

        [SerializeField]
        private GameObject box;

        [SerializeField]
        private Utils.Direction side = Utils.Direction.Up;

        private PlayerController player;

        private Vector2 gridCoordinates;

        public enum Owner { Box, Environment, Player };

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            if (levelController != null)
            {
                // Sets the size
                SetSize();

                // Sets the grid coordinates
                gridCoordinates = Vector2.zero;
                if (owner == Owner.Environment)
                {
                    // Gets the grid coordinates from the curretn position
                    gridCoordinates = Utils.GetGridCoordinates(
                        transform.position,
                        levelController.GridCellWidth,
                        levelController.GridOffset);

                    // Sets the position to the center of the grid cell
                    transform.position = Utils.GetPositionFromGridCoord(
                        gridCoordinates,
                        levelController.GridCellWidth,
                        levelController.GridOffset);
                }
                // Sets everything relating to the player 
                else if (owner == Owner.Player)
                {
                    player = FindObjectOfType<PlayerController>();
                    side = Utils.Direction.Right;
                }
            }
        }

        private void SetSize()
        {
            Vector3 gridScale =
                levelController.GetGridScale(GetComponent<BoxCollider2D>().bounds.size);

            transform.localScale = new Vector3(transform.localScale.x * gridScale.x,
                                               transform.localScale.y * gridScale.y);
        }

        public Owner NBPOwner
        {
            get { return owner; }
        }

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
        }

        public Box ParentBox
        {
            get
            {
                if (owner == Owner.Box)
                {
                    return box.GetComponent<Box>();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            if (levelController != null)
            {
                if (owner == Owner.Player)
                {
                    UpdatePositionNextToPlayer();
                }
                else if (owner == Owner.Box)
                {
                    UpdatePositionNextToBox();
                }
            }
        }

        private void UpdatePositionNextToPlayer()
        {
            // Calculates the ground level based on the player character's position
            float groundY = player.transform.position.y -
                player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            // Sets the side based the player character's looking direction
            side = player.GetComponent<SpriteRenderer>().flipX ? Utils.Direction.Left : Utils.Direction.Right;

            // Calculates the x-coordinate
            float x = (player.transform.position + Utils.DirectionToVector3(side) / 2).x;

            // Sets the position
            transform.position = new Vector3(x, groundY + levelController.GridCellWidth / 2);
        }

        private void UpdatePositionNextToBox()
        {
            Vector3 boxPosition = box.transform.position;

            switch (side)
            {
                case Utils.Direction.Up:
                    {
                        transform.position = boxPosition + new Vector3(0, levelController.GridCellWidth);
                        break;
                    }
                case Utils.Direction.Down:
                    {
                        transform.position = boxPosition + new Vector3(0, -1 * levelController.GridCellWidth);
                        break;
                    }
                case Utils.Direction.Left:
                    {
                        transform.position = boxPosition + new Vector3(-1 * levelController.GridCellWidth, 0);
                        break;
                    }
                case Utils.Direction.Right:
                    {
                        transform.position = boxPosition + new Vector3(levelController.GridCellWidth, 0);
                        break;
                    }
            }


            // TODO: NewBoxPlace updates its grid coordinates when the parent box is placed

            //if (box._takingPosition)
            //{
            //    gridCoordinates = box.GridCoordinates + side;
            //}
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
