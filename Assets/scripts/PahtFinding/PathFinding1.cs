using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PathFinding1: MonoBehaviour {

        public Transform seeker, target;

        Grid1 grid;

        private void Awake()
        {
            grid = GetComponent<Grid1>();
        }

        void Update()
        {
        }

        public List<Vector2> FindPath( Vector3 startPos, Vector3 targetPos )
        {
            Node1 startNode = grid.NodeFromWorldPoint(startPos);
            Node1 targetNode = grid.NodeFromWorldPoint(targetPos);

            List<Node1> openSet = new List<Node1>();
            HashSet<Node1> closedSet = new HashSet<Node1>();
            openSet.Add(startNode);
            while(openSet.Count > 0)
            {
                Node1 currentNode = openSet[0];
                for(int i = 1; i < openSet.Count; i++)
                {
                    if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost
                        && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);


                if(currentNode == targetNode)
                {
                    List<Node1> path = RetracePath(startNode, targetNode);
                    List<Vector2> moveLocations = new List<Vector2>();
                    for(int i = 0; i < path.Count; i++)
                    {
                        moveLocations.Add(new Vector2(path[i].gridX + 0.5f , path[i].gridY + 0.5f));
                    }
                    return moveLocations;
                }

                foreach (Node1 neighbour in grid.GetNeighbours(currentNode))
                {
                    if(!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    if(currentNode.hasParent())
                    {
                        if(!currentNode.IsGrounded(grid) && !neighbour.IsGrounded(grid) && currentNode.gridX != currentNode.parent.gridX && currentNode.gridX != neighbour.gridX)
                        {
                            continue;
                        } if(!currentNode.isJumping && !currentNode.IsGrounded(grid) && currentNode.gridY < neighbour.gridY)
                        {
                            continue;
                        }
                    }
                    if(currentNode.IsGrounded(grid) && !neighbour.IsGrounded(grid) && neighbour.gridY > currentNode.gridY)
                    {
                        neighbour.isJumping = true;
                    } else if (neighbour.IsGrounded(grid))
                    {
                        neighbour.isJumping = false;
                    }
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        
                        if(!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        
                        
                       
                    }
                }
            }
            return null;
        }

        List<Node1> RetracePath(Node1 startNode, Node1 endNode)
        {
            List<Node1> path = new List<Node1>();
            Node1 currentNode = endNode;

            while(currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            grid.path = path;
            return path;
        }

        int GetDistance(Node1 nodeA, Node1 nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if(distX > distY)
                 return 14 * distY + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);
        }

        public void UpdateNode (int x, int y, bool canWalk)
        {
            grid.GetNode(x, y).walkable = canWalk;
        }
    }

}
