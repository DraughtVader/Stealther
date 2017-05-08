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
        get { return InputDevice != null && InputDevice.Action1.IsPressed; }
    }

    public bool Jumped
    {
        get { return InputDevice != null && (!InputDevice.Action1.LastState && InputDevice.Action1.IsPressed); }
    }

    public bool IsRoping
    {
        get { return InputDevice != null && InputDevice.LeftTrigger.IsPressed; }
    }

    public bool Roped
    {
        get { return InputDevice != null && (!InputDevice.LeftTrigger.LastState && InputDevice.LeftTrigger.IsPressed); }
    }

    public bool IsAttacking
    {
        get { return InputDevice != null && InputDevice.RightTrigger.IsPressed; }
    }

    public bool Attacked
    {
        get { return InputDevice != null && (!InputDevice.RightTrigger.LastState && InputDevice.RightTrigger.IsPressed); }
    }

    public Vector2 LeftStick
    {
        get { return InputDevice != null ? InputDevice.LeftStick.Vector : Vector2.zero; }
    }

    public Vector2 RightStick
    {
        get { return InputDevice != null ? InputDevice.RightStick.Vector : Vector2.zero ; }
    }

    public XInputDevice InputDevice { get; set; }
}
