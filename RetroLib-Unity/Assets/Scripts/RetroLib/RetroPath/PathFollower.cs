using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Retro.Path
{
	public class PathFollower : MonoBehaviour 
	{
		[SerializeField]
		private float _MovementSpeed;

		[SerializeField]
		private RetroPath _PathToFollow;

		private PathNode _curNode;

		// Use this for initialization
		void Start () 
		{
			_curNode = _PathToFollow.BeginningNode;
			if(_curNode != null)
			{
				transform.position = _curNode.transform.position;
			}
		}
		
		// Update is called once per frame
		void LateUpdate ()
		{
			if(_curNode != null)
			{
				transform.position = Vector3.MoveTowards(transform.position, _curNode.transform.position, _MovementSpeed * Time.deltaTime);
				Quaternion newRot = Quaternion.LookRotation((_curNode.transform.position - transform.position).normalized, Vector3.up);
				transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 1.0f * Time.deltaTime);


				float fDist = Vector3.Distance(transform.position, _curNode.transform.position);
				if(fDist < 0.01f)
				{
					_curNode = _curNode.NextNode;
				}
			}
		
		}
	}
}
