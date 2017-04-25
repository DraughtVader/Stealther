using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class NinjaInputManager : MonoBehaviour
{
    [SerializeField]
    protected int playerNum;
    public int PlayerNum
    {
        get { return playerNum; }
    }

    public bool IsJumping
    {
        get { return InputDevice.Action1.IsPressed; }
    }

    public bool Jumped
    {
        get { return !InputDevice.Action1.LastState && InputDevice.Action1.IsPressed; }
    }

    public bool IsRoping
    {
        get { return InputDevice.LeftTrigger.IsPressed; }
    }

    public bool Roped
    {
        get { return !InputDevice.LeftTrigger.LastState && InputDevice.LeftTrigger.IsPressed; }
    }

    public bool IsAttacking
    {
        get { return InputDevice.RightTrigger.IsPressed; }
    }

    public bool Attacked
    {
        get { return !InputDevice.RightTrigger.LastState && InputDevice.RightTrigger.IsPressed; }
    }

    public Vector2 LeftStick
    {
        get { return InputDevice.LeftStick.Vector; }
    }

    public Vector2 RightStick
    {
        get { return InputDevice.RightStick.Vector; ; }
    }

    public InputDevice InputDevice
    {
        get
        {
            return (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
        }
    }
}
