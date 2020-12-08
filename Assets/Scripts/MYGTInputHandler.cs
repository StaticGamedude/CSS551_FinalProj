using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MYGTInputHandler : InputHandler
{
    public float GetXInputMovement()
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetYInputMovement()
    {
        return 0f; //TODO: Determine which button on the controller would be best suited for this
    }

    public float GetZInputMovmeent()
    {
        return Input.GetAxis("Vertical");
    }

    public bool isChangeNodeButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton2); //X Button on the controller
    }

    public bool isDecrementButtonPressed()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool isIncrementButtonPressed()
    {
        return Input.GetKey(KeyCode.RightShift);
    }

    public bool isSelectionButtonHeld()
    {
        return Input.GetKey(KeyCode.JoystickButton0); //A button on the controller
    }

    public bool isSelectionButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton0); //A button on the controller
    }

    public bool isTransformAxisButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton1); //B button on the controller
    }

    public bool isTransformModeButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton3); //Y Button on the controller
    }
}
