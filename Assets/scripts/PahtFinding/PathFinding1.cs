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
            Node1 startNode = grid.NodeFromWorldPoint(new Vector3(Mathf.FloorToInt(startPos.x) + grid.noderRadius, Mathf.FloorToInt(startPos.y) + grid.noderRadius, 0));
            Node1 targetNode = grid.NodeFromWorldPoint(new Vector3(Mathf.FloorToInt(targetPos.x) + grid.noderRadius, Mathf.FloorToInt(targetPos.y) + grid.noderRadius, 0));
            List<Node1> openSet = new List<Node1>();
            HashSet<Node1> closedSet = new HashSet<Node1>();
            openSet.Add(startNode);
            int number = 0;
            while(openSet.Count > 0 && number < 100)
            {
                number++;
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
                    grid.clearList();
                    return moveLocations;
                }
               

                foreach (Node1 neighbours in grid.GetNeighbours(currentNode))
                {
                    Node1 node = new Node1 (true,new Vector3(1,1,1),1,1);
                    if(neighbours.used)
                    {
                        if(neighbours.parent != currentNode && !neighbours.duplicated)
                        {
                            neighbours.duplicated = true;
                            node.walkable = neighbours.walkable;
                            node.worldPosition = neighbours.worldPosition;
                            node.gridX = neighbours.gridX;
                            node.gridY = neighbours.gridY;
                            node.duplicated = true;
                            node.oldParent = neighbours.parent;
                        }
                    }
                    Node1 neighbour = new Node1(true, new Vector3(1, 1, 1), 1, 1);
                    if(neighbours.duplicated)
                    {
                        neighbour = node;
                    } else
                    {
                        neighbour = neighbours;
                    }

                    if(!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if(currentNode.isJumping && !neighbour.IsGrounded(grid))
                    {
                        currentNode.isJumping = true;
                    }

                    if(currentNode.hasParent())
                    {
                       
                        if(neighbour.isJumping && currentNode.parent.gridY + 1 < neighbour.gridY)
                        {
                            neighbour.isJumping = false;
                            continue;
                        }

                        if(currentNode.parent.hasParent())
                        {
                            if(currentNode.parent.parent.gridY + 2 < neighbour.gridY)
                            {
                                neighbour.isJumping = false;
                                continue;
                            }
                        }
                        if (currentNode.parent.IsGrounded(grid) && !currentNode.IsGrounded(grid) && neighbour.IsGrounded(grid) && (currentNode.parent.gridX + 1 < neighbour.gridX
                            || currentNode.parent.gridX - 1 > neighbour.gridX))
                        {
                            continue;
                        }

                        if(!currentNode.IsGrounded(grid) && !neighbour.IsGrounded(grid) && currentNode.gridX != currentNode.parent.gridX && currentNode.gridX != neighbour.gridX)
                        {
                            continue;
                        } else if(!currentNode.isJumping && !currentNode.IsGrounded(grid) && currentNode.gridY < neighbour.gridY)
                        {
                            continue;
                        }
                        if(currentNode.isJumping && currentNode.gridY > neighbour.gridY)
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
                        neighbour.used = true;
                        
                        if(!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    } 
                }
            }
            grid.clearList();
            //Debug.Log("Path not found!");
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
            //for(int i = 0; i < path.Count; i++)
            //{
            //    Debug.Log(path[i].gridX + " Y " + path[i].gridY);
            //}
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

        public bool FindBlockedPath( Vector3 startPos, Vector3 targetPos, Box boxToRemove)
        {
            Node1 startNode = grid.NodeFromWorldPoint(startPos);
            Node1 targetNode = grid.NodeFromWorldPoint(targetPos);
            Node1 nodeToRemove = grid.GetNode((int)boxToRemove.transform.position.x,(int) boxToRemove.transform.position.y);
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
                if(grid.GetNode(currentNode.gridX, currentNode.gridY - 1).groundLayer)
                {
                    return true;
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                
                if(currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    grid.clearList();
                    return true;
                }


                foreach(Node1 neighbour in grid.GetNeighbours(currentNode))
                {
                    if(neighbour.gridX == nodeToRemove.gridX && neighbour.gridY == nodeToRemove.gridY)
                    {
                        continue;
                    }
                    if(neighbour.groundLayer)
                    {
                        continue;
                    }
                    if(neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if(grid.GetNode(neighbour.gridX,neighbour.gridY - 1).groundLayer)
                    {
                        return true;
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
            grid.clearList();
            return false;
        }

        public bool isGrounded(Vector3 position)
        {
            Node1 groundedNode = grid.GetNode((int)position.x, (int)position.y);
            if(groundedNode.IsGrounded(grid))
            {
                return true;
            }
            return false;
        }
    }
}
