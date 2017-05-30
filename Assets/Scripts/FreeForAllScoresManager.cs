using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeForAllScoresManager : ScoresManager
{
	protected override void AddScore(NinjaController ninja)
	{
		int score = ++GetNinja(ninja).Score;
		if (score >= targetScore)
		{
			GameManager.Instance.FinalRoundEnded();
			GameUiManager.Instance.DisplayFinalScores(participatingNinjas);
		}
		else
		{
			GameManager.Instance.RoundEnded();
			GameUiManager.Instance.DisplayScores(participatingNinjas);
		}
	}

	public override void NinjaKilled(NinjaController killedNinja, NinjaController killer)
	{
		aliveNinjas.Remove(killedNinja);
		if (aliveNinjas.Count == 1)
		{
			AddScore(aliveNinjas[0]);
		}
	}
}
