using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public interface IGridObject
    {
        Vector2 GridCoordinates { get; set; }
        void MoveToGridCoordinates();
    }
}
