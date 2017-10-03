using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce {
    public class BoxController : MonoBehaviour {

        [SerializeField]
        private List<BoxMovement> boxes = new List<BoxMovement>();

        // Use this for initialization
        void Start() {

        }


        public List<BoxMovement> GetBoxes()
        {
            return boxes;
        }

        public int BoxAmount()
        {
            return boxes.Count;
        }

        // Update is called once per frame
        void Update() {
            if(Input.GetKeyDown(KeyCode.L)) {
                removeBox();
            }
        }

        public void addBox (BoxMovement box) {
            boxes.Add(box);
            RefreshFollowTargets();
        }


        // Refreshes following order for active boxes.
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

        // Removes box from follow line.
        public void removeBox() {
            if (boxes.Count > 0) {
                boxes[0].RemoveFollowTarget();
                boxes.RemoveAt(0);
                RefreshFollowTargets();
            }

        }

        // Places box for player pointed position
        public void PlaceBox (Vector3 followTarget) {
            if (boxes.Count > 0) {
                BoxMovement obj = boxes[0];
                removeBox();
                obj.TakePosition(followTarget);
            }
        }
    }
}
