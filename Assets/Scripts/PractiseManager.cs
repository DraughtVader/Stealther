using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class PractiseManager : MonoBehaviour
{
    [SerializeField]
    protected NinjaController ninja;

    private void Update()
    {
        var inputDevice = InputManager.ActiveDevice;
        ninja.GetComponent<NinjaInputManager>().AssignInput(inputDevice as XInputDevice);
    }
}
