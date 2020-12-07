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

    private List<NodePrimitive> lookedAtPrimitives;

    private NodePrimitive heldObject = null;

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
        lookedAtPrimitives = BaseNode.ObjectLookedAt(Camera.main.transform);
    }


    public bool TryHoldObject()
    {
        bool objectFoundToHold = false;
        if (lookedAtPrimitives != null && lookedAtPrimitives.Count > 0)
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

            if (heldObject != closestPrimitive)
            {
                if (heldObject != null)
                {
                    heldObject.parentCameraTransform = null;
                }

                heldObject = closestPrimitive;
                heldObject.parentCameraTransform = Camera.main.transform;
                heldObject.PerformHoldActions(Camera.main.transform);
                objectFoundToHold = true;
            }
        }
        return objectFoundToHold;
    }

    public void ReleaseObject()
    {
        if (heldObject != null)
        {
            List<Matrix4x4> parentTransforms = new List<Matrix4x4>();
            BaseNode.ReleaseHeldObject(parentTransforms);
            heldObject = null;

            //heldObject.PerformReleaseActions();
            //heldObject.parentCameraTransform = null;
            //heldObject = null;
        }
    }
}
