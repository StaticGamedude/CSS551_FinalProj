﻿/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface to specify the input actions supported.
/// </summary>
public interface InputHandler
{
    int GetZInputMovmeent();

    int GetXInputMovement();

    int GetYInputMovement();

    bool isSelectionButtonPressed();

    bool isSelectionButtonHeld();

    bool isChangeNodeButtonPressed();

    bool isTransformModeButtonPressed();

    bool isTransformAxisButtonPressed();
}
