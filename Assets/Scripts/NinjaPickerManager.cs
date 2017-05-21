using System.Collections.Generic;
using UnityEngine;

public class NinjaPickerManager : MonoBehaviour
{
	[SerializeField]
	protected NinjaBank ninjaBank;

	[SerializeField]
	protected NinjaAccessories ninjaAccessories,
		ninjaBodyBank;

	private List<NinjaDescription> currentDescriptions = new List<NinjaDescription>();

	public List<NinjaDescription> CurrentDescriptions
	{
		get { return currentDescriptions; }
	}

	public AccessoryDescription GetNextHat()
	{
		return ninjaAccessories.GetRandomAccessory();
	}

	public AccessoryDescription[] GetBodies(int count)
	{
		return ninjaBodyBank.GetRandomAccessories(count);
	}

	public NinjaDescription GetNextDescription(NinjaDescription current)
	{
		currentDescriptions.Remove(current);
		var newDescription = ninjaBank.GetNextDescription(current);
		while (currentDescriptions.Contains(newDescription))
		{
			newDescription = ninjaBank.GetNextDescription(newDescription);
		}
		currentDescriptions.Add(newDescription);
		return newDescription;
	}

	public NinjaDescription GetLastDescription(NinjaDescription current)
	{
		currentDescriptions.Remove(current);
		var newDescription = ninjaBank.GetPreviousDescription(current);
		while (currentDescriptions.Contains(newDescription))
		{
			newDescription = ninjaBank.GetPreviousDescription(newDescription);
		}
		currentDescriptions.Add(newDescription);
		return newDescription;
	}

	public NinjaDescription GetDescription()
	{
		var newDescription = ninjaBank.GetRandomNinja(currentDescriptions);
		while (currentDescriptions.Contains(newDescription))
		{
			newDescription = ninjaBank.GetNextDescription(newDescription);
		}
		currentDescriptions.Add(newDescription);
		return newDescription;
	}

	private void Start()
	{
		GameManager.Instance.MatchFinished += OnMatchFinished;
	}

	private void OnMatchFinished()
	{
		currentDescriptions.Clear();
	}
}