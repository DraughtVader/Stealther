using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	[SerializeField]
	protected string title = "LevelTitle";

	public string Title
	{
		get { return title; }
	}

	[SerializeField]
	protected SpawnManager spawnManager;

	public SpawnManager SpawnManager
	{
		get { return spawnManager; }
	}
}
