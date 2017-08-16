using UnityEngine;
using System.Collections;

namespace Retro.Grid
{
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        private Grid _TargetGrid;
        
        private void Start()
        {
            _TargetGrid = GetComponent<Grid>();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public Block GetGridBlock(Ray ray)
        {
            // project finger position to the grid plane
            float fDist;
            if(_TargetGrid.GetPlane().Raycast(ray, out fDist))
            {
                Vector3 selectionPos = ray.origin + (ray.direction * fDist);
                Block hoverBlock = _TargetGrid.FindNearestBlock(selectionPos);
                return hoverBlock;
            }
            return null;
        }

    }
}