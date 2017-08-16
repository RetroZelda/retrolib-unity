using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Grid
{
    public enum BlockDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    public class Block : MonoBehaviour
    {
        public delegate bool BlockTraversalDelimiter(Block block);

        [SerializeField]
        private Vector3 _LocalAttachPoint = Vector3.zero;

        [SerializeField]
        private Vector3 _LocalAttachAngle = Vector3.zero;

        [SerializeField]
        private Vector3 _LocalAttachScale = Vector3.one;

        [SerializeField]
        private bool _CanAttachObject;

        private List<AttachableObject> _occupants = new List<AttachableObject>();
        private AttachableObject _attachedObject = null;

        public Grid Grid { get; protected set; }

        public Block UpBlock { get; private set; }
        public Block DownBlock { get; private set; }
        public Block LeftBlock { get; private set; }
        public Block RightBlock { get; private set; }

        public int RowIndex { get; private set; }
        public int ColIndex { get; private set; }

        public bool Traversable { get; set; }
        public List<AttachableObject> Occupants { get { return _occupants; } private set { _occupants = value; } }
    
        public float NavigationWeight
        {
            get { return 1.0f + Occupants.Count; }
        }

        public IEnumerable AdjacentBlocks
        {
            get
            {
                if (UpBlock != null)
                    yield return UpBlock;
                if (DownBlock != null)
                    yield return DownBlock;
                if (LeftBlock != null)
                    yield return LeftBlock;
                if (RightBlock != null)
                    yield return RightBlock;
            }
        }

        public bool CanAttachObject
        {
            get { return _CanAttachObject; }
            set { _CanAttachObject = value; }
        }

        public virtual AttachableObject AttachedObject
        {
            get { return _attachedObject; }
            protected set { _attachedObject = value; }
        }

        public Vector3 LocalAttachPoint // for children
        {
            get
            {
                return _LocalAttachPoint;
            }
            set
            {
                _LocalAttachPoint = value;
            }
        }

        public Vector3 LocalAttachAngle // for children
        {
            get
            {
                return _LocalAttachAngle;
            }
            set
            {
                _LocalAttachAngle = value;
            }
        }

        public Vector3 LocalAttachScale // for children
        {
            get
            {
                return _LocalAttachScale;
            }
            set
            {
                _LocalAttachScale = value;
            }
        }

        public Vector3 AttachPoint
        {
            get
            {
                return transform.position + LocalAttachPoint;
            }
        }
        
        public void Awake()
        {
            CanAttachObject = false;
            Traversable = false;
        }

        /// <summary>
        /// Traverse the blocks in a direction
        /// </summary>
        /// <param name="nDepth">The max count to traverse</param>
        /// <param name="direction">The direction to traverse</param>
        /// <param name="delimiter">A delimination check</param>
        /// <returns>Number of blocks traversed</returns>
        public int BlockTraverse(int nDepth, BlockDirection direction, BlockTraversalDelimiter delimiter)
        {
            if (delimiter(this))
                return 0;

            if (nDepth > 0)
            {
                switch (direction)
                {
                    case BlockDirection.Up:
                        if (UpBlock != null)
                            return UpBlock.BlockTraverse(nDepth - 1, direction, delimiter) + 1;
                        break;
                    case BlockDirection.Down:
                        if (DownBlock != null)
                            return DownBlock.BlockTraverse(nDepth - 1, direction, delimiter) + 1;
                        break;
                    case BlockDirection.Left:
                        if (LeftBlock != null)
                            return LeftBlock.BlockTraverse(nDepth - 1, direction, delimiter) + 1;
                        break;
                    case BlockDirection.Right:
                        if (RightBlock != null)
                            return RightBlock.BlockTraverse(nDepth - 1, direction, delimiter) + 1;
                        break;
                }
            }
            return 0;
        }

        public virtual void SetUp(Grid _grid, int _RowIndex, int _ColIndex)
        {
            Grid = _grid;
            RowIndex = _RowIndex;
            ColIndex = _ColIndex;
        }

        public virtual void Refresh()
        {
            UpBlock = Grid.GetBlock(RowIndex, ColIndex + 1);
            DownBlock = Grid.GetBlock(RowIndex, ColIndex - 1);
            LeftBlock = Grid.GetBlock(RowIndex - 1, ColIndex);
            RightBlock = Grid.GetBlock(RowIndex + 1, ColIndex);
        }

        public bool AttachObject(AttachableObject obj)
        {
            // already occupied
            if ((AttachedObject != null && obj != null))
            {
                return false;
            }

            AttachedObject = obj;
            if (obj)
            {
                // if the object is occupied, stop that
                if (obj.OwningBlock != null)
                {
                    obj.OwningBlock.AttachObject(null);
                }

                obj.OwningBlock = this;
                obj.transform.SetParent(transform);
                obj.transform.localPosition = LocalAttachPoint;
                obj.transform.localRotation = Quaternion.Euler(LocalAttachAngle);
                obj.transform.localScale = LocalAttachScale;
            }
            return true;
        }
    }
}