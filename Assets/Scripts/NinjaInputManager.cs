﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class NinjaInputManager : MonoBehaviour
{
    private bool isVibrating;
    private DateTime stopVibration;

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

    public bool RopeUp
    {
        get { return InputDevice != null && (InputDevice.LeftTrigger.LastState && !InputDevice.LeftTrigger.IsPressed); }
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

    public bool PressedRight
    {
        get {  return InputDevice != null && (!InputDevice.DPadRight.LastState && InputDevice.DPadRight.IsPressed);}
    }

    public bool PressedLeft
    {
        get {  return InputDevice != null && (!InputDevice.DPadLeft.LastState && InputDevice.DPadLeft.IsPressed);}
    }

    public void AssignInput(XInputDevice device)
    {
        InputDevice = device;
        GetComponent<NinjaController>().InputAssigned();
    }

    public XInputDevice InputDevice { get; private set; }

    //TODO fix: currents turns off controllers
    /*
    public void Vibrate(float intensity, float duration = 0.5f)
    {
        return;
        InputDevice.Vibrate(intensity);
        isVibrating = true;
        stopVibration = DateTime.Now.AddSeconds(duration);
    }

    private void Update()
    {
        if (isVibrating && DateTime.Now >= stopVibration)
        {
            StopVibration();
        }
    }

    private void StopVibration()
    {
        isVibrating = false;
        InputDevice.StopVibration();
    }
    */
}
