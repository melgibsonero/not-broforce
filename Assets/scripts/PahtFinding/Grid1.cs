using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class Grid1: MonoBehaviour
    {
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float noderRadius;
        Node1[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        private void Start()
        {
            
            nodeDiameter = noderRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid();
        }

        void CreateGrid ()
        {
            grid = new Node1[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
            for(int x = 0; x < gridSizeX; x++)
            {
                for(int y = 0; y < gridSizeY; y++)
                {
                    
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + noderRadius) + Vector3.up * (y * nodeDiameter + noderRadius);
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, noderRadius*0.9f,unwalkableMask));
                    grid[x, y] = new Node1(walkable, worldPoint, x, y);

                }
            }

        }

        public List<Node1> GetNeighbours(Node1 node )
        {
            List<Node1> neighbours = new List<Node1>();
            

            for (int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0 || x != 0 && y != 0)
                        continue;
                   

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    
                    if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }

        public Node1 NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = worldPosition.x / gridWorldSize.x;
            float percentY = worldPosition.y / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        public List<Node1> path;
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

            if(grid != null)
            {
                foreach(Node1 n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;

                    if(path != null)
                        if(path.Contains(n))
                            Gizmos.color = Color.black;
                    if(n.gridY == 9 && n.gridX == 26)
                    {
                        Gizmos.color = Color.blue;
                    }
                    Color colour = Gizmos.color;
                    colour.a = 0.4f;
                    Gizmos.color = colour;
                    Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - 0.1f));

                }
            }
        }

        public Node1 GetNode (int x, int y)
        {
            return grid[x, y];
        }

        public void clearList ()
        {
            for(int x = 0; x < gridSizeX; x++)
            {
                for(int y = 0; y < gridSizeY; y++)
                {
                    grid[x, y].duplicated = false;
                    grid[x, y].used = false;
                    grid[x, y].oldParent = null;
                    grid[x, y].isJumping = false;

                }
            }
        }
        

    }
}
