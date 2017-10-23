using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class GridHelper : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private Vector2 gridCoordinates;

        [SerializeField]
        private LevelController levelController;

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set
            {
                gridCoordinates = value;
                MoveToGridCoordinates();
            }
        }

        public void MoveToGridCoordinates()
        {
            transform.position = Utils.GetPosFromGridCoord(
                gridCoordinates,
                levelController.GridCellWidth,
                levelController.GridOffset);
        }

        /// <summary>
        /// Draws a box to the grid coordinates.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Updates grid coordinates when moved in the editor
            if (!Application.isPlaying)
            {
                gridCoordinates = Utils.GetGridCoordinates(
                    transform.position,
                    levelController.GridCellWidth,
                    levelController.GridOffset);
            }

            // The bottom left corner of the grid cell
            Vector3 bottomLeft =
                Utils.GetBottomLeftPosFromGridCoord(
                    gridCoordinates,
                    levelController.GridCellWidth,
                    levelController.GridOffset);

            Vector3 point;
            Vector3 point2;
            Gizmos.color = Color.yellow;

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
}
