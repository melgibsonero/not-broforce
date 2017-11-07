using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce {
    public class BoxController : MonoBehaviour {

        [SerializeField]
        private List<Box> boxes = new List<Box>();

        [SerializeField]
        private List<Box> placedBoxes = new List<Box>();

        /// <summary>
        /// Removes boxes from structure.
        /// </summary>
        private bool removeBoxesAfter = false;

        // Use this for initialization
        void Start() {

        }

        /// <summary>
        /// Returns currently following boxes
        /// </summary>
        public List<Box> GetBoxes() {
            return boxes;
        }

        /// <summary>
        /// Returns amount of currently moving boxes
        /// </summary>
        public int MovingBoxAmount() {
            return boxes.Count;
        }

        // Update is called once per frame
        void Update() {
            if(Input.GetKey(KeyCode.P))
            {
                Time.timeScale = 4f;
            } else
            {
                Time.timeScale = 1f;
            }
            }

        /// <summary>
        /// Adds box to follow the player.
        /// </summary>
        public void addBox(Box box) {
            boxes.Add(box);
            RefreshFollowTargets();
        }

        /// <summary>
        /// Adds box to placed box list 
        /// </summary>
        public void addPlacedBox(Box box) {
            placedBoxes.Add(box);
        }

        /// <summary>
        /// Returns currently places boxes.
        /// </summary>
        public List<Box> GetPlacedBoxes (){
            return placedBoxes;
        }





        /// <summary>
        /// Refreshes following order for active boxes.
        /// </summary>
        private void RefreshFollowTargets() {
            for(int i = 0; i < boxes.Count; i++) {
                boxes[i].GetComponent<SpriteRenderer>().sortingOrder = boxes.Count - i;
                if(i == 0) {
                    boxes[i].AddFollowTarget(GameObject.FindGameObjectWithTag("Player").transform);
                } else {
                    boxes[i].AddFollowTarget(boxes[i - 1].transform);
                } 
            }
        }

        /// <summary>
        ///  Removes box from follow line.
        /// </summary>
        public void removeBox() {
            if (boxes.Count > 0) {
                boxes[0].RemoveFollowTarget();
                boxes.RemoveAt(0);
                RefreshFollowTargets();
            }

        }

        /// <summary>
        /// Places box for player pointed position
        /// </summary>
        /// <param name="followTarget"></param>
        public void PlaceBox (Vector3 followTarget) {
            if (boxes.Count > 0) {
                Box obj = boxes[0];
                bool pathFound = obj.TakePosition(followTarget);
                if(pathFound)
                {
                    removeBox();
                }
            }

        }

        /// <summary>
        /// Removes given box, and every box placed after given box.
        /// </summary>
        public void RemovePlacedBox (Box box) {

            List<int> boxIndexes = new List<int>();

            for(int i = 0; i < placedBoxes.Count; i++)
            {
                if(placedBoxes[i] == box)
                {

                    boxIndexes.Add(i);
                    removeBoxesAfter = true;
                }
                else if(removeBoxesAfter && placedBoxes.Count > i)
                {

                    boxIndexes.Add(i);
                }
            }
            boxIndexes.Reverse();
            while(boxIndexes.Count > 0)
            {
                Box boxes = placedBoxes[boxIndexes[0]];
                placedBoxes.RemoveAt(boxIndexes[0]);
                boxIndexes.RemoveAt(0);
                addBox(boxes);
                boxes.BackToLine();

            }
            removeBoxesAfter = false;
        }
    }
}
