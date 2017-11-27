using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class ValidBoxPlace : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private Sprite glowSprite;

        [SerializeField]
        private Sprite reservedSprite;

        [SerializeField]
        private Color attachedToBoxColor;

        [SerializeField]
        private Color onGroundColor;

        [SerializeField]
        private Color reservedColor;

        private Vector2 gridCoordinates;

        private Utils.Direction direction;

        private SpriteRenderer sr;

        public bool IsAttachedToBox { get; private set; }

        public bool IsReserved { get; set; }

        public bool IsVisible
        {
            get
            {
                return (sr != null && sr.enabled);
            }
            set
            {
                if (sr != null)
                {
                    sr.enabled = value;
                }
            }
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Set(Vector2 gridCoordinates,
                        bool attachedToBox,
                        bool reservedPlace,
                        Utils.Direction direction =
                          Utils.Direction.Up)
        {
            if (sr == null)
            {
                Init();
            }

            GridCoordinates = gridCoordinates;
            IsAttachedToBox = attachedToBox;
            IsReserved = reservedPlace;

            if (reservedPlace)
            {
                sr.sprite = reservedSprite;
                sr.color = reservedColor;
                this.direction = Utils.Direction.Up;
            }
            else
            {
                sr.sprite = glowSprite;

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
