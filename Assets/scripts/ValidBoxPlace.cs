using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class ValidBoxPlace : MonoBehaviour, IGridObject
    {
        private Vector2 gridCoordinates;

        private Utils.Direction direction;

        public bool IsAttachedToBox { get; private set; }

        public void Initialize(Vector2 gridCoordinates,
                               bool attachedToBox,
                               Utils.Direction direction)
        {
            GridCoordinates = gridCoordinates;
            IsAttachedToBox = attachedToBox;

            if (attachedToBox)
            {
                this.direction = direction;
            }
            else
            {
                this.direction = Utils.Direction.Up;
            }

            transform.rotation = 
                Utils.DirectionToQuaternion(this.direction);
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

        public void MoveToGridCoordinates()
        {
            transform.position = LevelController.GetPosFromGridCoord(
                gridCoordinates);
        }
    }
}
