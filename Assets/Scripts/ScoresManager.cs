using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScoresManager : MonoBehaviour
{
	[SerializeField]
	protected GameMode mode;

	public GameMode Mode
	{
		get { return mode; }
	}

	[SerializeField]
	protected int targetScore;

	protected List<ParticipatingNinja> participatingNinjas = new List<ParticipatingNinja>();
	protected List<NinjaController> aliveNinjas = new List<NinjaController>();

	public SpawnManager Spawner { get; set; }

	public List<NinjaController> AliveNinjas
	{
		get { return aliveNinjas; }
	}
	
	public int NinjaCount
	{
		get { return participatingNinjas.Count; }
	}
	
	public abstract void NinjaKilled(NinjaController killedNinja, NinjaController killer);

	public void AddNinjas(List<NinjaController> ninjas)
	{
		foreach (var ninja in ninjas)
		{
			AddNinja(ninja);
		}
		Setup();
	}
	
	protected void AddNinja(NinjaController ninja)
	{
		participatingNinjas.Add(new ParticipatingNinja(ninja));
		aliveNinjas.Add(ninja);
	}
	
	protected abstract void AddScore(NinjaController ninja);

	protected ParticipatingNinja GetNinja(NinjaController ninja)
	{
		var length = participatingNinjas.Count;
		for (var i = 0; i < length; i++)
		{
			var participatingNinja = participatingNinjas[i];
			if (participatingNinja.Ninja == ninja)
			{
				return participatingNinja;
			}
		}
		return null;
	}

	protected virtual void Setup()
	{
		GameManager.Instance.RoundStart += OnRoundStart;
		GameManager.Instance.RoundEnd += OnRoundEnd;
		GameManager.Instance.MatchFinished += OnMatchFinished;
	}

	protected void Desetup()
	{
		GameManager.Instance.RoundStart -= OnRoundStart;
		GameManager.Instance.RoundEnd -= OnRoundEnd;
		GameManager.Instance.MatchFinished -= OnMatchFinished;
	}

	protected virtual void OnMatchFinished()
	{
		foreach (var ninja in AliveNinjas)
		{
			ninja.transform.position = new Vector3(100, 100); //TODO tidy
			ninja.Rigidbody.isKinematic = true;
			ninja.Rigidbody.velocity = Vector2.zero;
		}
		AliveNinjas.Clear();
		participatingNinjas.Clear();
		Desetup();
	}

	protected virtual void OnRoundStart()
	{
		var spawnPoints = Spawner.SpawnPoints;
		var length = AliveNinjas.Count;
		for (var i = 0; i < length; i++)
		{
			SpawnNinja(AliveNinjas[i], spawnPoints[i].position);
		}
	}

	protected void SpawnNinja(NinjaController ninja, Vector2 spawnPosition)
	{
		ninja.Rigidbody.velocity = Vector2.zero;
		ninja.gameObject.SetActive(true);
		ninja.Rigidbody.isKinematic = false;
		ninja.State = NinjaController.NinjaState.Alive;
		ninja.transform.position = spawnPosition;
	}
	
	protected virtual void OnRoundEnd()
	{
		aliveNinjas = participatingNinjas.Select(x => x.Ninja).ToList();
	}
}

[System.Serializable]
public class ParticipatingNinja
{
	public NinjaController Ninja;
	public int Score;
	public int Team;

	public ParticipatingNinja(NinjaController ninja, int score = 0, int team = -1)
	{
		Ninja = ninja;
		Score = score;
		Team = team;
	}
}

public enum GameMode
{
	FreeForAll,
	Deathmatch
}
