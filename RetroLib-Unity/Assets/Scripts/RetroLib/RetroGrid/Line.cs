using UnityEngine;
using System.Collections;

namespace Retro.Grid
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
    {
        [SerializeField]
        private Color _LineColor = Color.red;

        [SerializeField]
        [HideInInspector]
        private LineRenderer _Renderer;

        [SerializeField]
        [HideInInspector]
        private Vector3 _StartPosition;

        [SerializeField]
        [HideInInspector]
        private Vector3 _EndPosition;

        public LineRenderer RawLine
        {
            get { return _Renderer  == null ? _Renderer = GetComponent<LineRenderer>() : _Renderer; }
        }

        public Color LineColor
        {
            get
            {
                return _LineColor;
            }
            set
            {
                _LineColor = value;
                RawLine.SetColors(value, value);
            }
        }

        public Vector3 StartPosition
        {
            get
            {
                return _StartPosition;
            }
            set
            {
                _StartPosition = value;
                RawLine.SetPosition(0, value);
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                return _EndPosition;
            }
            set
            {
                _EndPosition = value;
                RawLine.SetPosition(1, value);
            }
        }

        public void SetPosition(Vector3 v3Start, Vector3 v3End)
        {
            StartPosition = v3Start;
            EndPosition = v3End;
        }
        
        void Start()
        {
            RawLine.useWorldSpace = false;
            StartPosition = _StartPosition;
            EndPosition = _EndPosition;
            LineColor = _LineColor;
        }
    }
}