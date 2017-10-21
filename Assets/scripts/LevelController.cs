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
        /// Only works if the scale is originally [1, 1, 0].
        /// </summary>
        public Vector3 GetGridScale(Vector3 size)
        {
            float scaleX =
                gridCellWidth / size.x;
            float scaleY =
                gridCellWidth / size.y;

            return new Vector3(scaleX, scaleY);
        }
    }
}
