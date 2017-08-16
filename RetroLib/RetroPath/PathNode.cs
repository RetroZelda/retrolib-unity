using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.Path
{
	public class PathNode : MonoBehaviour 
	{
		public PathNode PrevNode { get; set; }
		public PathNode NextNode { get; set; }
	}

}