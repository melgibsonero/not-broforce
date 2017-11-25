using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class ValidBoxPlace : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private Color attachedToBoxColor;

        [SerializeField]
        private Color onGroundColor;

        private Vector2 gridCoordinates;

        private Utils.Direction direction;

        private SpriteRenderer sr;

        public bool IsAttachedToBox { get; private set; }

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Vector2 gridCoordinates,
                               bool attachedToBox,
                               Utils.Direction direction =
                                 Utils.Direction.Up)
        {
            if (sr == null)
            {
                Start();
            }

            GridCoordinates = gridCoordinates;
            IsAttachedToBox = attachedToBox;
            //sr = GetComponent<SpriteRenderer>();

            if (attachedToBox)
            {
                this.direction = direction;
                sr.color = attachedToBoxColor;
            }
            else
            {
                this.direction = Utils.Direction.Up;
                sr.color = onGroundColor;
            }

            transform.rotation = 
                Utils.DirectionToQuaternion(this.direction);
        }

        public void SetVisibility(bool active)
        {
            if (sr != null)
            {
                sr.enabled = active;
            }
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
