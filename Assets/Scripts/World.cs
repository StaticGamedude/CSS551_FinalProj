/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class World : MonoBehaviour
{
    //Variables to be set in the Unity Editor
    public SceneNode BaseNode;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(BaseNode != null);
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 startPos = Matrix4x4.identity;
        BaseNode.CompileTransform(ref startPos);

        //TODO: we probably want to return a list from this, since we'll need to determine which object
        //is closest to the camera (in the case that there are multiple objects in front of the camera
        BaseNode.ObjectLookedAt(Camera.main.transform);
    }
}
