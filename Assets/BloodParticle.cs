using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour 
{
	[SerializeField]
	protected GameObject bloodSplatter;
	
	private Color currentColor = Color.white;
	
	public static Transform bloodSpatterParent;
	private static List<GameObject> parentedBlood;

	public void SetUp(Color color, Vector2 velocity)
	{
		GetComponentInChildren<TrailRenderer>().material.color = currentColor = color;
		GetComponent<SpriteRenderer>().color = currentColor = color;
		GetComponent<Rigidbody2D>().velocity = velocity;
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		var bloodable = other.GetComponent<Bloodable>();
		if (bloodable == null)
		{
			return;
		}
		var blood = Instantiate(bloodSplatter, transform.position,  Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
		blood.GetComponent<SpriteRenderer>().sortingOrder =
			bloodable.GetComponent<SpriteRenderer>().sortingOrder + 1;
		blood.GetComponent<SpriteRenderer>().color = currentColor;
		
		if (bloodable.SetParent)
		{
			blood.transform.parent = (other.transform);
			if (parentedBlood == null)
			{
				parentedBlood = new List<GameObject>();
			}
			parentedBlood.Add(blood);
		}
		else
		{
			if (bloodSpatterParent == null)
			{
				bloodSpatterParent = new GameObject("BloodSpatterParent").transform;
			}
			blood.transform.parent = bloodSpatterParent;
		}
		
		Destroy(gameObject);
	}
	
	public static void DestroyAll()
	{
		if (bloodSpatterParent != null)
		{
			Destroy(bloodSpatterParent.gameObject);
			bloodSpatterParent = null;
		}
		if (parentedBlood != null)
		{
			foreach (var blood in parentedBlood)
			{
				if (blood != null)
				{
					Destroy(blood);
				}
			}
			parentedBlood.Clear();
		}
	}
}
