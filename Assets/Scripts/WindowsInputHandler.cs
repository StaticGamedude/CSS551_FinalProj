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
    public int GetXInputMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            return -1;
        }
        return 0;
    }

    public int GetYInputMovement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            return -1;
        }
        return 0;
    }

    public int GetZInputMovmeent()
    {
        if (Input.GetKey(KeyCode.W))
        {
            return 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return -1;
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
}
