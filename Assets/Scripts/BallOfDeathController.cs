using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOfDeathController : MonoBehaviour 
{
	[SerializeField]
	protected GameObject ballOfDeathPrefab;

	[SerializeField]
	protected float speed = 1,
	speedincrease = 1.25f;

	private GameObject ballOfDeath;
	private Rigidbody2D ballRigidBody;
	private float coolDown;
	private Camera mainCamera;

	private void Update ()
	{
		if (ballOfDeath == null )
		{
			return;
		}

		if (coolDown > 0.0f)
		{
			coolDown -= Time.deltaTime;
			return;
		}

		Vector2 viewportPosition = mainCamera.WorldToViewportPoint (ballOfDeath.transform.position);

		var velocity = ballRigidBody.velocity;
		if (viewportPosition.x <= 0 || viewportPosition.x >= 1)
		{
			velocity.x *= -1;
		}
		else if (viewportPosition.y <= 0 || viewportPosition.y >= 1) 
		{
			velocity.y *= -1;
		} 
		else 
		{
			return;
		}
		ballRigidBody.velocity = velocity;
		ballRigidBody.AddForce (velocity * speedincrease);
		coolDown = 0.5f;
	}

	private void SummonTheBall()
	{
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		if (ballOfDeath == null) 
		{
			ballOfDeath = Instantiate (ballOfDeathPrefab, Vector2.zero, Quaternion.identity);
			ballRigidBody = ballOfDeath.GetComponent<Rigidbody2D>();
		}
		else
		{
			ballOfDeath.transform.position = Vector2.zero;
		}
		var b = ballOfDeath.GetComponent<BallOfDeath>();
		b.CloseMouth ();
		b.Looking = true;
		float x = (Random.value > 0.5f ? 1 : -1) * Random.Range(0.8f, 1.2f),
		      y = (Random.value > 0.5f ? 1 : -1) * Random.Range(0.8f, 1.2f);
		ballRigidBody.velocity = new Vector2(x, y).normalized * speed;
		mainCamera = Camera.main;
	}

	private void BanishTheBall()
	{
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		Destroy (ballOfDeath);
		ballOfDeath = null;
	}

	private void Awake()
	{
		GameManager.Instance.RoundStart += SummonTheBall;
		GameManager.Instance.RoundEnd += OnRoundEnd;
		GameManager.Instance.MatchFinished += BanishTheBall;
	}

	private void OnRoundEnd()
	{
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		var b = ballOfDeath.GetComponent<BallOfDeath>();
		b.Looking = false;
	}
}
