using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.Path
{
	public class RetroPath : MonoBehaviour 
	{
		[SerializeField]
		private PathNode[] _Path;

		public PathNode BeginningNode { get { return _Path[0]; }}

		void Start()
		{
			RefreshPath();
		}

		public void RefreshPath()
		{
			PathNode _prevPath = null;
			foreach(PathNode path in _Path)
			{
				path.PrevNode = _prevPath;
				if(_prevPath != null)
				{
					_prevPath.NextNode = path;
				}
				_prevPath = path;
			}
		}
	}

}