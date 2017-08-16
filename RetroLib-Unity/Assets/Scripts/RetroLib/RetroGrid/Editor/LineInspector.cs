using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Retro.Grid
{
    [CustomEditor(typeof(Line))]
    public class LineInspector : Editor
    {
        private static bool _bMoveStart;
        private static bool _bMoveEnd;
        
        
        public override void OnInspectorGUI()
        {
            Line myLine = (Line)target;

            DrawDefaultInspector();
        }
    }
}