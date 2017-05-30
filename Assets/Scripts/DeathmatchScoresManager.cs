using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathmatchScoresManager : ScoresManager
{	
	[SerializeField]
	protected DeathMatchUIController uiController;
	
	protected override void AddScore(NinjaController ninja)
	{
		int score = ++GetNinja(ninja).Score;
		if (score >= targetScore)
		{
			GameManager.Instance.FinalRoundEnded();
			GameUiManager.Instance.DisplayFinalScores(participatingNinjas);
		}
	}

	public override void NinjaKilled(NinjaController killedNinja, NinjaController killer)
	{
		if (killer != null)
		{
			AddScore(killer);
			uiController.UpdateScore(killer.PlayerNumber, GetNinja(killer).Score);
		}
		else
		{
			uiController.UpdateScore(killedNinja.PlayerNumber, --GetNinja(killedNinja).Score);
		}
		if (killer == null || GetNinja(killer).Score < targetScore)
		{
			SpawnNinja(killedNinja, Spawner.SpawnPoints[Random.Range(0, Spawner.SpawnPoints.Length)].position);
			killedNinja.StartShield(3.0f);
		}
	}

	protected override void Setup()
	{
		base.Setup();
		uiController.SetUp(aliveNinjas);
	}

	protected override void OnMatchFinished()
	{
		base.OnMatchFinished();
		uiController.Desetup();
	}
}
