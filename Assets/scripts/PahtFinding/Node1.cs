using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace not_broforce
{
    public class Node1
    {
        
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public Node1 parent;
        public bool isJumping = false;

        public bool used = false;
        public bool duplicated = false;
        public Node1 oldParent;

        public bool groundLayer = false;

        public Node1( bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            if(!walkable)
            {
                groundLayer = true;
            }
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public bool hasParent ()
        {
            if(parent != null)
            {
                return true;
            }
            return false;
        }

        public bool IsGrounded (Grid1 grid)
        {
            if(!grid.GetNode(gridX, gridY - 1).walkable)
            {
                return true;
            }

            return false;
        }
        
    }
}