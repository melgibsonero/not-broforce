using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PassableObstacle : Activatable
    {
        [SerializeField]
        private bool activatedByDefault;

        private BoxCollider2D boxCollider;

        private List<Switch> compatibleSwitches;

        private PathFinding1 pf;
        private Grid1 grid;
        private Node1[] takenNodes;

        public override void Awake()
        {
            base.Awake();

            InitCollider();

            compatibleSwitches = new List<Switch>();

            if (!CodeIsUnusable())
            {
                FindCompatibleSwitches();
            }
        }

        private void Start()
        {
            InitPathfinding();

            if (activatedByDefault)
            {
                MakePassable();
                UpdatePathfinding();
            }
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
            Switch[] allSwitches = FindObjectsOfType<Switch>();

            // Goes through the switch list and adds any switch which
            // has a compatible activation code to compatibleSwitches
            foreach (Switch activator in allSwitches)
            {
                if (activator.CodesMatch(GetCode()))
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
            CheckActivators();
        }

        private void CheckActivators()
        {
            // The old and the new activation state of the obstacle
            bool oldState = IsActivated();
            bool newState = activatedByDefault;

            foreach (Switch activator in compatibleSwitches)
            {
                newState = activator.IsActivated();

                // If the obstacle is activated
                // by default, the activator being  
                // activated yields the opposite result
                if (activatedByDefault)
                {
                    newState = !newState;
                }

                // If the new state is opposite of the obstacle's
                // default state, the search is stopped
                if (newState != activatedByDefault)
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
            Debug.Log("Passable, default activation: " + activatedByDefault);

            Activate();

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }

        private void MakeImpassable()
        {
            Debug.Log("Impassable, default activation: " + activatedByDefault);

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
