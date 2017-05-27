using UnityEngine;

public class RotaryMachine : MonoBehaviour
{
	[SerializeField]
	protected float maxRotationSpeed = 0.25f,
		frequency = 5.0f;

	[SerializeField]
	protected AnimationCurve rotationCurve;

	private float currentRotation, currentTime;
	private bool active;

	private void Update ()
	{
		if (!active)
		{
			return;
		}
		transform.eulerAngles += new Vector3(0, 0, maxRotationSpeed * rotationCurve.Evaluate(currentTime / frequency));
		currentTime += Time.deltaTime;
		if (currentTime >= frequency)
		{
			currentTime -= frequency;
		}
	}

	private void StartRotation()
	{
		active = true;
	}

	private void StopRotation()
	{
		active = false;
	}

	private void Awake()
	{
		GameManager.Instance.RoundStart += StartRotation;
		GameManager.Instance.MatchFinished += StopRotation;
	}
}
