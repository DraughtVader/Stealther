using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugStuff : MonoBehaviour
{
	private void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.R))
	    {
	        SceneManager.LoadScene(0);
	    }

	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        Application.Quit();
	    }
	}
}
