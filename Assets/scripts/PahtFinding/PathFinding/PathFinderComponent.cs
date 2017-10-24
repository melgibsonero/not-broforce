using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class PathFinderComponent: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            // create the tiles map
            float[,] tilesmap = new float[width, height];
            // set values here....
            // every float in the array represent the cost of passing the tile at that position.
            // use 0.0f for blocking tiles.

            // create a grid
            PathFind.Grid grid = new PathFind.Grid(width, height, tilesmap);

            // create source and target points
            PathFind.Point _from = new PathFind.Point(1, 1);
            PathFind.Point _to = new PathFind.Point(10, 10);

            // get path
            // path will either be a list of Points (x, y), or an empty list if no path is found.
            List<PathFind.Point> path = PathFind.Pathfinding.FindPath(grid, _from, _to);
        }

    }
}
