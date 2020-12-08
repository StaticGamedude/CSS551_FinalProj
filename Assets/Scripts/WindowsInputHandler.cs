/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the inputs when the user is running on Windows
/// </summary>
public class WindowsInputHandler : InputHandler
{
    public float GetXInputMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            return -1f;
        }
        return 0;
    }

    public float GetYInputMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            return 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            return -1f;
        }
        return 0;
    }

    public float GetZInputMovmeent()
    {
        if (Input.GetKey(KeyCode.W))
        {
            return 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return -1f;
        }
        return 0;
    }

    public bool isSelectionButtonHeld()
    {
        return Input.GetMouseButton(0);
    }

    public bool isSelectionButtonPressed()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool isChangeNodeButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.N);
    }

    public bool isTransformModeButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.T);
    }

    public bool isTransformAxisButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.Y);
    }

    public bool isIncrementButtonPressed()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }

    public bool isDecrementButtonPressed()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }
}
