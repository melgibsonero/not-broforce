using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class GridGizmo : MonoBehaviour
    {
        [SerializeField]
        private LevelController levelController;

        [SerializeField]
        private Color color = Color.blue;

        [SerializeField]
        private bool moveInEditor;

        /// <summary>
        /// Draws a square at the grid coordinates.
        /// </summary>
        private void OnDrawGizmos()
        {
            IGridObject gridObject = gameObject.GetComponent<IGridObject>();
            if (gridObject != null && levelController != null)
            {
                // Updates grid coordinates when moved in the editor
                if (moveInEditor && !Application.isPlaying)
                {
                    gridObject.GridCoordinates = LevelController.GetGridCoordinates(
                        transform.position);
                }

                // Sets the color of the square
                Gizmos.color = color;

                // The bottom left corner of the grid cell
                Vector3 bottomLeft =
                    LevelController.GetBottomLeftPosFromGridCoord(
                        gridObject.GridCoordinates);

                // The other three corners
                Vector3 topLeft = bottomLeft + Vector3.up * levelController.GridCellWidth;
                Vector3 topRight = topLeft + Vector3.right * levelController.GridCellWidth;
                Vector3 bottomRight = topRight + Vector3.down * levelController.GridCellWidth;

                // Top side
                Gizmos.DrawLine(topLeft, topRight);

                // Bottom side
                Gizmos.DrawLine(bottomRight, bottomLeft);

                // Left side
                Gizmos.DrawLine(bottomLeft, topLeft);

                // Right side
                Gizmos.DrawLine(topRight, bottomRight);
            }
        }
    }
}
