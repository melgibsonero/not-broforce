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
    }
}
