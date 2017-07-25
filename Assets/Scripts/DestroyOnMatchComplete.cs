using UnityEngine;

public class DestroyOnMatchComplete : MonoBehaviour 
{
	protected void Start ()
	{
		GameManager.Instance.MatchFinished += OnMatchComplete;
	}

	private void OnMatchComplete()
	{
		GameManager.Instance.MatchFinished -= OnMatchComplete;
		Destroy(gameObject);
	}
}
