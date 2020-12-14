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
