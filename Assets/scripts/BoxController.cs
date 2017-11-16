using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace not_broforce {
    public class BoxController : MonoBehaviour {

        [SerializeField]
        private List<Box> boxes = new List<Box>();

        [SerializeField]
        private List<Box> placedBoxes = new List<Box>();

        protected List<Box> removingBoxes = new List<Box>();

        [SerializeField]
        private Text roboCount;

        // Use this for initialization
        void Start() {
            if(roboCount == null)
            {
                roboCount = GameObject.Find("RoboCount").GetComponent<Text>();
            }
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

            if(Input.GetKeyDown(KeyCode.L))
            {
                CheckRemovingBoxes(placedBoxes[1]);
            }
            
            if(boxes.Count > 0 && roboCount != null)
            {
                roboCount.text = "Robot count " + boxes.Count;
            } else if (roboCount != null)
            {
                roboCount.text = "";
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
                boxes[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (boxes.Count - i) * 2;
                boxes[i].GetComponent<SpriteRenderer>().sortingOrder = (boxes.Count - i) * 2 - 1;
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
        /// 
        public void CheckRemovingBoxes (Box box)
        {

            for (int i = 0; i < placedBoxes.Count; i++)
            {
                if(placedBoxes[i] == box)
                {
                    removingBoxes.Add(box);
                }
                else if(!placedBoxes[i].FindPathInStructure(new Vector3(placedBoxes[0].transform.position.x, placedBoxes[0].transform.position.y - 1, 0), box))
                {
                    removingBoxes.Add(placedBoxes[i]);
                }
            }
        }

        public void ClearRemovingBoxes ()
        {
            removingBoxes.Clear();
        }

        /// <summary>
        /// Removes given box, and every box placed after given box.
        /// </summary>
        public void RemovePlacedBox () {

            
            while(removingBoxes.Count > 0)
            {
                Box box = removingBoxes[0];
                placedBoxes.Remove(box);
                addBox(box);
                box.BackToLine();
                removingBoxes.RemoveAt(0);

            }
        }

        public bool IsInStructure (Box box)
        {
            if(placedBoxes.Count > 0)
            {
                return box.FindPathInStructure(new Vector3(placedBoxes[0].transform.position.x, placedBoxes[0].transform.position.y - 1, 0), box);
            }
            return box.FindPathInStructure(new Vector3(transform.position.x, transform.position.y - 1, 0), box);
        }
    }
}
