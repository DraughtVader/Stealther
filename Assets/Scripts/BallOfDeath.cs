using System;
using UnityEngine;

public class BallOfDeath : Hazard
{
	[SerializeField]
	protected Transform pupils;

	[SerializeField]
	protected GameObject openMouth;

	[SerializeField]
	protected float maxMovement = 0.15f,
	moveSpeed = 5.0f;

	public bool Looking { get; set; }

	public void CloseMouth()
	{
		openMouth.SetActive (false);
	}

	protected override void NinjaKilled (NinjaController killedNinja)
	{
		base.NinjaKilled (killedNinja);
		openMouth.SetActive (true);
	}

	protected void Update()
	{
		Vector2 target = Vector2.zero;
		if (Looking)
		{
			var ninja = GameManager.Instance.GetClosestNinja(transform.position);
			if (ninja.transform.position.x < 30)
			{
				target = (ninja.transform.position - transform.position).normalized * maxMovement;
			}
		}
		pupils.localPosition = Vector2.Lerp (pupils.localPosition, target, Time.deltaTime * moveSpeed);
	}
}