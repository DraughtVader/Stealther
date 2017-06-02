using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour 
{
	[SerializeField]
	protected GameObject bloodSplatter;

	[SerializeField]
	protected Sprite[] sprites;
	
	private Color currentColor = Color.white;
	
	public static Transform bloodSpatterParent;
	private static List<GameObject> parentedBlood;

	public void SetUp(Color color, Vector2 velocity)
	{
		var s = GetComponent<SpriteRenderer>();
		s.color = currentColor = color;
		s.sprite = sprites[Random.Range(0, sprites.Length)];
		
		var r = GetComponent<Rigidbody2D>();
		r.velocity = velocity;
		
		var random = Random.Range(0.1f, 1.25f);
		transform.localScale *= random ;
		transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359f));
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
