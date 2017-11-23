using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelController : MonoBehaviour
    {
        public static float gridCellWidth = 1;
        
        public static Vector2 gridOffset = new Vector2(0.5f, 0.5f);

        public float GridCellWidth
        {
            get { return gridCellWidth; }
        }

        public Vector2 GridOffset
        {
            get { return gridOffset; }
        }

        private void Start()
        {
            if (gridCellWidth <= 0)
            {
                gridCellWidth = 1;

                Debug.LogError("The grid cell width cannot be " +
                               "less than or equal to 0.");
            }
        }

        /// <summary>
        /// Gets a scale which makes the given size into the grid's size.
        /// Works more reliably if the scale is originally [1, 1, 0].
        /// </summary>
        public Vector3 GetGridScale(Vector3 size)
        {
            float scaleX =
                gridCellWidth / size.x;
            float scaleY =
                gridCellWidth / size.y;

            return new Vector3(scaleX, scaleY);
        }

        public static Vector3 GetPosFromGridCoord(Vector2 gridCoordinates)
        {
            return new Vector3(gridCoordinates.x * gridCellWidth
                                   + gridOffset.x,
                               gridCoordinates.y * gridCellWidth
                                   + gridOffset.y);
        }

        public static Vector3 GetBottomLeftPosFromGridCoord(
                                                       Vector2 gridCoordinates)
        {
            return GetPosFromGridCoord(gridCoordinates) +
                new Vector3(-1 * gridCellWidth / 2, -1 * gridCellWidth / 2);
        }

        public static Vector2 GetGridCoordinates(Vector2 position)
        {
            // Modifies the current position to be
            // in line with the grid coordinates
            float positionX = position.x
                + gridCellWidth / 2
                - gridOffset.x;
            float positionY = position.y
                + gridCellWidth / 2
                - gridOffset.y;

            // Decreases the current position's coordinates by one grid
            // cell width when they're in the negatives (rounds them down)
            // to compensate integer rounding towards zero
            if(positionX < 0)
            {
                positionX -= gridCellWidth;
            }
            if(positionY < 0)
            {
                positionY -= gridCellWidth;
            }

            // Calculates the current position's grid coordinates
            int gridX =
                (int)(positionX / gridCellWidth);
            int gridY =
                (int)(positionY / gridCellWidth);

            // Returns the grid coordinates
            return new Vector2(gridX, gridY);
        }

        public static Node1 GetNodeFromGridCoord(Grid1 grid, Vector2 gridCoordinates)
        {
            Vector3 position = GetPosFromGridCoord(gridCoordinates);
            Node1 node = grid.NodeFromWorldPoint(position);

            return node;
        }

        /// <summary>
        /// Draws help lines at the origin of the grid.
        /// </summary>
        private void OnDrawGizmos()
        {
            // The bottom left corner of the origin cell
            Vector3 startPoint =
                GetBottomLeftPosFromGridCoord(Vector2.zero);

            // Vertical line
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint,
                startPoint + Vector3.up * gridCellWidth);

            // Horizontal line
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint,
                startPoint + Vector3.right * gridCellWidth);

            // Point
            Gizmos.color = Color.cyan;
            //new Color(73/255f, 150/255f, 255/255f); // light blue
            Gizmos.DrawSphere(startPoint, 0.05f);
        }
    }
}
