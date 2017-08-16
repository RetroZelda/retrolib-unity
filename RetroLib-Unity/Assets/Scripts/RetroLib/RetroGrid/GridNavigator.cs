using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Retro.Grid
{
    public class GridNavigator : AttachableObject
    {
        public delegate void FinishedPathEvent();
        private class NavigationNode
        {
            public NavigationNode Parent;
            public Block Block;
            public float MovementCost;
            public float Heuristic;

            public float Score { get { return MovementCost + Heuristic; } }

            public NavigationNode(NavigationNode parent, Block block, float cost, float heuristic)
            {
                Parent = parent;
                Block = block;
                MovementCost = cost;
                Heuristic = heuristic;
            }

            public bool Equals(NavigationNode n)
            {
                return Block == (n.Block);
            }

            public bool Equals(Block b)
            {
                return Block == (b);
            }
        }

        [SerializeField]
        private float _movementSpeed = 10.0f;

        [SerializeField]
        private float _DistanceFreedom = 1.0f;
        
        private Grid _Grid;
        private List<Block> _Route;
        private List<Vector3> _Path;
        private int _ncurNavBlockIndex;

        public FinishedPathEvent OnFinishedPath;

        public bool HasRoute { get; private set; }
        public bool IsNavigating { get; private set; }
        public Grid Grid
        {
            get
            {
                return _Grid;
            }
            set
            {
                _Grid = value; OwningBlock = _Grid.FindNearestBlock(transform.position);
                OwningBlock.Occupants.Add(this); // TODO: Something like this to mark that this grid block has a new node on it
            }
        }
        
        public void StartNavigation()
        {
            IsNavigating = HasRoute;
        }

        public void StopNavigation()
        {
            IsNavigating = false;
        }

        protected void Update()
        {
            if (IsNavigating)
            {
                // move towards the current block
                Vector3 v3DestinationPoint = _Path[_ncurNavBlockIndex];
                Vector3 v3OldPos = transform.position;
                transform.position = Vector3.MoveTowards(transform.position, v3DestinationPoint, _movementSpeed * Time.deltaTime);

                Vector3 v3LerpDir = Vector3.Lerp(transform.forward, (transform.position - v3OldPos).normalized, 0.25f);
                transform.rotation = Quaternion.LookRotation(v3LerpDir, Vector3.up);


                // move on if we are close enough to the block
                float fDist = Vector3.Distance(transform.position, v3DestinationPoint);
                if (fDist < _DistanceFreedom)
                {
                    // exit if we have reached the end of the path
                    _ncurNavBlockIndex++;
                    if (_ncurNavBlockIndex >= _Path.Count)
                    {
                        StopNavigation();
                        if (OnFinishedPath != null)
                        {
                            OnFinishedPath.Invoke();
                        }
                    }
                }

                // ensure the block is aware of us
                Block newBlock = Grid.FindNearestBlock(transform.position) as Block;
                if (newBlock != OwningBlock)
                {
                    (OwningBlock as Block).Occupants.Remove(this);
                    newBlock.Occupants.Add(this);
                    OwningBlock = newBlock;
                }
            }
        }

        public bool SetNavigationDestination(Block destinationBlock, float fSmoothLevel = 0.0f)
        {
            _ncurNavBlockIndex = 0;
            HasRoute = FindRoute(OwningBlock as Block, destinationBlock, out _Route);

            if (HasRoute)
            {
                _Path = new List<Vector3>(MakePath(_Route.ToArray(), fSmoothLevel));
            }
            
            return HasRoute;
        }

        // A*
        protected bool FindRoute(Block startBlock, Block endBlock, out List<Block> path)
        {
            List<NavigationNode> openList = new List<NavigationNode>();
            List<Block> closedList = new List<Block>();

            // we dont need to consider our starting point
            closedList.Add(startBlock);

            // but we have to start somewhere
            foreach (Block block in startBlock.AdjacentBlocks)
            {
                if (block.Traversable)
                {
                    openList.Add(new NavigationNode(null, block, block.NavigationWeight, CalculateHeuristic(block, endBlock)));
                }
                else
                {
                    closedList.Add(block);
                }
            }
            
            // run the algorithm
            while (openList.Count > 0)
            {
                // get the next block with lowest score
                openList.Sort((x, y) => x.MovementCost.CompareTo(y.MovementCost));
                NavigationNode currentNode = openList[0];
                openList.RemoveAt(0);
                closedList.Add(currentNode.Block);

                // if we are at the end we can leave
                if (currentNode.Block == endBlock)
                {
                    path = new List<Block>();
                    while(currentNode != null)
                    {
                        // path.Insert(path.Count, currentNode.Block);
                        path.Insert(0, currentNode.Block);
                        currentNode = currentNode.Parent;
                    }
                    return true;
                }

                // calculate and add our adjacent blocks to the proper lists
                foreach (Block block in currentNode.Block.AdjacentBlocks)
                {
                    if (closedList.Contains(block))
                        continue;

                    // handle if we are already in the open list
                    int nExistingIndex = openList.FindIndex((x) => x.Block == block);
                    if(nExistingIndex < 0)
                    {
                        if (block.Traversable)
                        {
                            openList.Add(new NavigationNode(currentNode, block, currentNode.MovementCost + block.NavigationWeight, CalculateHeuristic(block, endBlock)));
                        }
                        else
                        {
                            closedList.Add(block);
                        }
                    }
                    else
                    {
                        // if a lower score, update the parent
                        NavigationNode existingNode = openList[nExistingIndex];
                        if (existingNode.Score < currentNode.Score)
                        {
                            existingNode.MovementCost = currentNode.MovementCost + 1.0f;
                            existingNode.Parent = currentNode;
                        }
                    }
                }
            }


            // unable to find route
            path = null;
            return false;
        }

        private float CalculateHeuristic(Block startBlock, Block endBlock)
        {
            return Vector3.Distance(startBlock.transform.position, endBlock.transform.position);
        }

        public static Vector3[] MakePath(Block[] path, float fSmoothLevel)
        {
            Vector3[] pathPoints = new Vector3[path.Length];
            List<Vector3> points;
            List<Vector3> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;

            int nCount = 0;
            foreach(Block gb in path)
            {
                pathPoints[nCount++] = gb.AttachPoint;
            }

            if (fSmoothLevel < 1.0f) return pathPoints;

            pointsLength = path.Length;

            curvedLength = (pointsLength * Mathf.RoundToInt(fSmoothLevel)) - 1;
            curvedPoints = new List<Vector3>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new List<Vector3>(pathPoints);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = ((1 - t) * points[i]) + (t * points[i + 1]);
                    }
                }

                curvedPoints.Add(points[0]);
            }

            return (curvedPoints.ToArray());
        }

    #if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (_Route != null)
            {
                Gizmos.color = Color.blue;
                for (int nVecIndex = 0; nVecIndex < _Route.Count - 1; ++nVecIndex)
                {
                    Gizmos.DrawLine(_Route[nVecIndex].AttachPoint, _Route[nVecIndex + 1].AttachPoint);
                }
            }

            if (_Path != null)
            {
                Gizmos.color = Color.red;
                for (int nVecIndex = 0; nVecIndex < _Path.Count - 1; ++nVecIndex)
                {
                    Gizmos.DrawLine(_Path[nVecIndex], _Path[nVecIndex + 1]);
                }
            }
        }
    #endif
    }
}