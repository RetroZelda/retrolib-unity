using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Retro.Grid
{    
    [CustomEditor(typeof(Grid))]
    public class GridInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}