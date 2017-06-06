using System;
using UnityEngine;

public class NinjaSpawner : MonoBehaviour
{
	[SerializeField]
	protected float spawnDelay = 1.8f;

	[SerializeField]
	protected ParticleSystem ps;

	private DateTime spawnTime;
	private NinjaController ninjaToSpawn;
	private bool ninjaSpawned;
	private NinjaState ninjaState;
	private AudioSource audioSource;
	
	public void SetUp (NinjaController ninja, NinjaState stateOnSpawn) 
	{
		ninjaToSpawn = ninja;
		ninjaToSpawn.State = NinjaState.Spawning;
		ninjaState = stateOnSpawn;
		spawnTime = DateTime.Now.AddSeconds(spawnDelay);
		var settings = ps.main;
		settings.startColor = ninja.NinjaColor;
		ps.Play();
		audioSource = GetComponent<AudioSource>();
	}
	
	private void Update () 
	{
		if (ninjaSpawned && ps.particleCount == 0 && audioSource.time <= 0)
		{
			Destroy(gameObject);
		}
		if (ninjaToSpawn == null)
		{
			return;
		}
		if (DateTime.Now >= spawnTime)
		{
			ninjaToSpawn.Spawn(transform.position);
			ninjaToSpawn.State = ninjaState;
			ninjaToSpawn = null;
			ninjaSpawned = true;
		}
	}
}
