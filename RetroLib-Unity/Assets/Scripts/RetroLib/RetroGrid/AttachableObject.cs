using UnityEngine;
using System.Collections;

namespace Retro.Grid
{
    public abstract class AttachableObject : MonoBehaviour
    {
        public Block OwningBlock { get; set; }
    }
}