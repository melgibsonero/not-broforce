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

        // Update is called once per frame
        void Update() {

        }

        public void addBox (BoxMovement box) {
            boxes.Add(box);
            RefreshFollowTargets();
        }

        private void RefreshFollowTargets() {
            for(int i = 0; i < boxes.Count; i++) {
                if(i == 0) {
                    boxes[i].AddFollowTarget(GameObject.FindGameObjectWithTag("Player").transform);
                } else {
                    boxes[i].AddFollowTarget(boxes[i - 1].transform);
                } 
            }
        }
    }
}
