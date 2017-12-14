using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace not_broforce {
    public class BoxController : MonoBehaviour {

        [SerializeField]
        private List<Box> movingBoxes = new List<Box>();

        [SerializeField]
        private List<Box> placedBoxes = new List<Box>();

        protected List<Box> removingBoxes = new List<Box>();

        protected List<Box> allBoxes = new List<Box>();

        protected BoxSelector selector;

        [SerializeField]
        private Text roboCount;

        // Use this for initialization
        void Start() {
            selector = FindObjectOfType<BoxSelector>();
            if(roboCount == null)
            {
                roboCount = GameObject.Find("RoboCount").GetComponent<Text>();
            }
        }

        /// <summary>
        /// Returns currently following boxes
        /// </summary>
        public List<Box> GetBoxes() {
            return movingBoxes;
        }

        /// <summary>
        /// Returns amount of currently moving boxes
        /// </summary>
        public int MovingBoxAmount() {
            return movingBoxes.Count;
        }

        // Update is called once per frame
        void Update() {
            if(movingBoxes.Count > 0 && roboCount != null)
            {
                roboCount.text = "Box count: " + movingBoxes.Count;
            } else if (roboCount != null)
            {
                roboCount.text = "Box count: 0";
            }
        }

        /// <summary>
        /// Adds box to follow the player.
        /// </summary>
        public void addBox(Box box) {
            if(!allBoxes.Contains(box))
            {
                allBoxes.Add(box);
            }
            movingBoxes.Add(box);
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
            for(int i = 0; i < movingBoxes.Count; i++) {
                movingBoxes[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (movingBoxes.Count - i) * 2;
                movingBoxes[i].GetComponent<SpriteRenderer>().sortingOrder = (movingBoxes.Count - i) * 2 - 1;
                if(i == 0) {
                    movingBoxes[i].AddFollowTarget(GameObject.FindGameObjectWithTag("Player").transform);
                } else {
                    movingBoxes[i].AddFollowTarget(movingBoxes[i - 1].transform);
                } 
            }
        }

        /// <summary>
        ///  Removes box from follow line.
        /// </summary>
        public void removeBox() {
            if (movingBoxes.Count > 0) {
                movingBoxes[0].RemoveFollowTarget();
                movingBoxes.RemoveAt(0);
                RefreshFollowTargets();
            }

        }

        /// <summary>
        /// Places box for player pointed position
        /// </summary>
        /// <param name="followTarget"></param>
        public bool PlaceBox (Vector3 followTarget) {
            bool pathFound = false;
            if (movingBoxes.Count > 0) {
                Box obj = movingBoxes[0];
                pathFound = obj.TakePosition(followTarget);
                if(pathFound)
                {
                    removeBox();
                }
            }
            return pathFound;

        }

        /// <summary>
        /// Removes given box, and every box placed after given box.
        /// </summary>
        /// 
        public void CheckRemovingBoxes (Box box)
        {
            removingBoxes.Clear();
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

        public bool RecallAllBoxes()
        {
            bool success = false;

            if (allBoxes.Count > 0 &&
                !allBoxes[0].IsTeleporting())
            {
                success = true;

                placedBoxes.Clear();
                movingBoxes.Clear();

                foreach (Box box in allBoxes)
                {
                    box.TeleportToPlayer();
                }

                selector.RemoveAllBoxes();
                selector.InitBoxTeleport();
            }

            return success;
        }
    }
}
