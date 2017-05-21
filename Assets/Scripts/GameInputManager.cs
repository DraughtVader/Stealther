using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    [SerializeField]
    protected List<NinjaInputManager> players;

    private void Start()
    {
        InputManager.OnDeviceDetached += OnDeviceDetached;
    }

    private void OnDeviceAttached(InputDevice inputDevice)
    {
        AssignDevice(inputDevice);
    }

    private void Update()
    {
        if (players == null || players.Count == 0)
        {
            return;
        }

        var inputDevice = InputManager.ActiveDevice;

        if (UpButtonWasPressedOnDevice(inputDevice))
        {
            GameManager.Instance.UpButtonPress();
        }
        else if (DownButtonWasPressedOnDevice(inputDevice))
        {
            GameManager.Instance.DownButtonPress();
        }

        if (BButtonWasPressedOnDevice(inputDevice))
        {
            GameUiManager.Instance.BackToTitle();
        }

        if (!JoinButtonWasPressedOnDevice(inputDevice))
        {
            return;
        }
        if (ThereIsNoPlayerUsingDevice( inputDevice ))
        {
            AssignDevice( inputDevice );
        }
    }

    private void AssignDevice(InputDevice inputDevice)
    {
        var xinput = inputDevice as XInputDevice;
        players[xinput.DeviceIndex].AssignInput(xinput);
    }

    private bool JoinButtonWasPressedOnDevice( InputDevice inputDevice )
    {
        return inputDevice.Action1.WasPressed;
    }

    private bool UpButtonWasPressedOnDevice( InputDevice inputDevice )
    {
        return inputDevice.DPadUp.WasPressed;
    }

    private bool DownButtonWasPressedOnDevice( InputDevice inputDevice )
    {
        return inputDevice.DPadDown.WasPressed;
    }

    private bool BButtonWasPressedOnDevice( InputDevice inputDevice )
    {
        return inputDevice.Action2.WasPressed;
    }

    private bool ThereIsNoPlayerUsingDevice( InputDevice inputDevice )
    {
        var xinput = inputDevice as XInputDevice;
        return players[xinput.DeviceIndex].InputDevice == null;
    }

    private void OnDeviceDetached( InputDevice inputDevice )
    {
        var xinput = inputDevice as XInputDevice;
        players[xinput.DeviceIndex].AssignInput(null);
    }
}
