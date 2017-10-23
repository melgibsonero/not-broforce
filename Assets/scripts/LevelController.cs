using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField]
        private float gridCellWidth = 1;

        [SerializeField]
        private Vector2 gridOffset;

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

        /// <summary>
        /// Draws help lines at the origin of the grid.
        /// </summary>
        private void OnDrawGizmos()
        {
            // The bottom left corner of the origin cell
            Vector3 startPoint =
                Utils.GetBottomLeftPosFromGridCoord(Vector2.zero,
                                                    gridCellWidth,
                                                    gridOffset);

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
