using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatchUIController : UIController
{
	[SerializeField]
	protected Text[] scores;

	public void SetUp(List<NinjaController> ninjas)
	{
		int length = ninjas.Count;
		for (int i = 0; i < length; i++)
		{
			var text = scores[ninjas[i].PlayerNumber];
			text.gameObject.SetActive(true);
			text.color = ninjas[i].NinjaColor;
			if (text.color == Color.black)
			{
				text.GetComponent<Outline>().effectColor = Color.white;
			}
			text.text = "0";
		}
	}

	public void UpdateScore(int index, int score)
	{
		scores[index].text = score.ToString();
	}

	public void Desetup()
	{
		int length = scores.Length;
		for (int i = 0; i < length; i++)
		{
			scores[i].gameObject.SetActive(false);
		}
	}
}