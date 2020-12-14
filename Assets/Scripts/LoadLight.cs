/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLight : MonoBehaviour
{
    public Transform LightPosition;

    void OnPreRender()
    {
        Shader.SetGlobalVector("LightPosition", LightPosition.position); //using world position since the light can be picked up and moved around
    }
}
