using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		var ropeNode = other.GetComponent<RopeNode>();
		if (ropeNode != null)
		{
			ropeNode.RopeController.DetachRope();
		}
	}
}
