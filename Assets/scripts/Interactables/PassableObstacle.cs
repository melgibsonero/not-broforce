using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PassableObstacle : Activatable
    {
        private BoxCollider2D boxCollider;

        private List<Switch> compatibleSwitches;

        private PathFinding1 pf;
        private Grid1 grid;
        private Node1[] takenNodes;

        public override void Awake()
        {
            base.Awake();

            InitCollider();

            FindCompatibleSwitches();
        }

        private void Start()
        {
            InitPathfinding();
        }

        private void InitCollider()
        {
            boxCollider = GetComponent<BoxCollider2D>();

            // If a collider could not be found, an error message is printed
            if (boxCollider == null)
            {
                Debug.LogError("BoxCollider2D component could " +
                               "not be found in the object.");
            }
        }

        private void InitPathfinding()
        {
            // The obstacle's height
            float height = GetComponent<BoxCollider2D>().bounds.size.y;

            // The world y-coordinate of the obstacle's bottom side
            float bottomY =
                (transform.position + new Vector3(0, -1 * height / 2)).y;

            // The amount of nodes the obstacle occupies in the y-axis
            int vertNodes =
                (int) (height / LevelController.gridCellWidth + 0.5f);

            // Initializes the taken node array
            takenNodes = new Node1[vertNodes];

            pf = FindObjectOfType<PathFinding1>();

            if (pf != null)
            {
                grid = pf.GetComponent<Grid1>();
            }
            else
            {
                Debug.LogError("An object with a PathFinding1 component " +
                               "could not be found in the scene.");
            }

            if (grid != null)
            {
                for (int i = 0; i < takenNodes.Length; i++)
                {
                    // The world point of the current node's center
                    Vector3 nodeCenterWP = transform.position;

                    // Set's the node's center's y-coordinate
                    // (bottomY plus i times the whole gridCellWidth
                    // and a half gridCellWidth)
                    nodeCenterWP.y = bottomY +
                                     i * LevelController.gridCellWidth +
                                     LevelController.gridCellWidth / 2;

                    takenNodes[i] = grid.NodeFromWorldPoint(nodeCenterWP);
                }
            }
            else
            {
                Debug.LogError("An object with a Grid1 component " +
                               "could not be found in the scene.");
            }
        }

        private void FindCompatibleSwitches()
        {
            compatibleSwitches = new List<Switch>();

            Switch[] allSwitches = FindObjectsOfType<Switch>();

            // Goes through the switch list and adds any switch which
            // has a compatible activation code to compatibleSwitches
            foreach (Switch activator in allSwitches)
            {
                if (GetCode() == activator.GetCode())
                {
                    compatibleSwitches.Add(activator);
                }
            }

            // If there are no compatible switches, an error message is printed
            if (compatibleSwitches.Count == 0)
            {
                Debug.LogError("No compatible switches could be found " +
                               "in the scene. This PassableObstacle " +
                               "cannot be activated.");
            }
        }

        private void Update()
        {
            // The old and the new activation state of the obstacle
            bool oldState = IsActivated();
            bool newState = false;

            foreach (Switch activator in compatibleSwitches)
            {
                newState = activator.IsActivated();

                if (newState == true)
                {
                    break;
                }
            }

            if (oldState != newState)
            {
                if (newState == true)
                {
                    MakePassable();
                }
                else
                {
                    MakeImpassable();
                } 

                UpdatePathfinding();
            }
        }

        private void MakePassable()
        {
            Activate();

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }

        private void MakeImpassable()
        {
            Deactivate();

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }

        private void UpdatePathfinding()
        {
            if (pf != null)
            {
                foreach (Node1 node in takenNodes)
                {
                    pf.UpdateNode(node.gridX, node.gridY, IsActivated());
                }
            }
        }
    }
}
