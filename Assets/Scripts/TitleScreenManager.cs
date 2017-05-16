using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
	public void GoToMultiplayer()
	{
		SceneManager.LoadScene("Multiplayer");
	}

	public void GoToPractise()
	{
		//TODO
		//SceneManager.LoadScene("SinglePlayer");
	}

	private void Update()
	{
		var inputDevice = InputManager.ActiveDevice;
		if (inputDevice == null)
		{
			return;
		}
		if (inputDevice.Action1)
		{
			//TODO
		}
		else if (inputDevice.Action2)
		{
			GoToMultiplayer();
		}
	}
}
