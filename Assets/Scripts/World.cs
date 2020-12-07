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

    public Camera mainCam;

    /// <summary>
    /// List of node primitives that are currently in the "view range" of the main camera
    /// </summary>
    private List<NodePrimitive> lookedAtPrimitives;

    /// <summary>
    /// Flag to indicate whether an object is being held in the world
    /// </summary>
    private bool objectHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(BaseNode != null);
        Debug.Assert(mainCam != null);
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 startPos = Matrix4x4.identity;
        BaseNode.CompileTransform(ref startPos);

        if (!objectHeld)
        {
            lookedAtPrimitives = BaseNode.ObjectLookedAt(mainCam.transform);
        }
    }

    /// <summary>
    /// If no objects are currently being held and we're currently lookin at objects, attempt to grab hold
    /// of the closet object.
    /// </summary>
    /// <returns></returns>
    public bool TryHoldObject()
    {
        if (!objectHeld && lookedAtPrimitives != null && lookedAtPrimitives.Count > 0)
        {
            NodePrimitive closestPrimitive = lookedAtPrimitives[0];
            foreach(NodePrimitive primitive in lookedAtPrimitives)
            {
                Vector3 closestPos = closestPrimitive.currentTransform.GetColumn(2);
                Vector3 primPos = primitive.currentTransform.GetColumn(2);
                Vector3 camToClosest = closestPos - Camera.main.transform.position;
                Vector3 camToPrim = primPos - Camera.main.transform.position;

                if (camToPrim.magnitude < camToClosest.magnitude)
                {
                    closestPrimitive = primitive;
                }
            }

            closestPrimitive.PerformHoldActions(Camera.main.transform);
            objectHeld = true;
        }
        return objectHeld;
    }

    /// <summary>
    /// If an object is currently held in the world, attempt to release it.
    /// </summary>
    public void ReleaseObject()
    {
        if (objectHeld)
        {
            objectHeld = !BaseNode.ReleaseHeldObject();
            if (objectHeld)
            {
                Debug.LogError("An error occurred release the currently held object in the world");
            }
        }
    }

    /// <summary>
    /// Determines if an object is currently being held in the world.
    /// </summary>
    /// <returns></returns>
    public bool isObjectHeld()
    {
        return objectHeld;
    }
}
