using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

namespace not_broforce
{
    public class NewBoxPlace : MonoBehaviour, IGridObject
    {
        [SerializeField]
        private Vector2 gridCoordinates;

        [SerializeField]
        private Parent owner;

        public enum Parent { Environment, Selector };

        public void Initialize(Vector2 gridCoordinates,
                               LevelController levelController,
                               Parent owner)
        {
            this.gridCoordinates = gridCoordinates;
            this.owner = owner;
        }

        public Parent Owner
        {
            get { return owner; }
        }

        public Vector2 GridCoordinates
        {
            get { return gridCoordinates; }
            set
            {
                gridCoordinates = value;
                MoveToGridCoordinates();
            }
        }

        public void MoveToGridCoordinates()
        {
            transform.position = LevelController.GetPosFromGridCoord(
                gridCoordinates);
        }
    }

    //[CustomEditor(typeof(NewBoxPlace))]
    //public class NewBoxPlaceEditor : Editor
    //{
    //    override public void OnInspectorGUI()
    //    {
    //        var newBoxPlace = target as NewBoxPlace;

    //        int selected = 0;
    //        string[] options = new string[]
    //        {
    //            "Box", "Environment", "Player"
    //        };

    //        selected = EditorGUILayout.Popup("Box", selected, options);

    //        using (var group = new EditorGUILayout.FadeGroupScope(selected))
    //        {
    //            if (!group.visible)
    //            {
    //                newBoxPlace.box = EditorGUILayout.IntSlider("I field:", newBoxPlace.i, 1, 100);
    //            }
    //        }
    //    }
    //}
}
