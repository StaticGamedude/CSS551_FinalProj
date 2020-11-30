/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNode : MonoBehaviour
{

    /// <summary>
    /// Starting position of our scene node. This position offsets this node's position after the TRS transform
    /// </summary>
    public Vector3 nodeOrigin = Vector3.zero;

    /// <summary>
    /// Combination of the parent's tranform, our pivot position (if one specified), and our scene node's transform (TRS). 
    /// </summary>
    private Matrix4x4 combinedParentTransform;

    /// <summary>
    /// List of Node primitives under this scene node. Contains the object actually displayed in the scene.
    /// </summary>
    public List<NodePrimitive> PrimitiveList;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    /// <summary>
    /// Calculate the transform for this scene nodes and inform any child scene nodes and primitives to do the same
    /// </summary>
    /// <param name="parentTransform"></param>
    public void CompileTransform(ref Matrix4x4 parentTransform)
    {
        Matrix4x4 originatingPostion = Matrix4x4.Translate(nodeOrigin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        combinedParentTransform = parentTransform * originatingPostion * trs;

        //For any child nodes, we need to tell them to compile their new position as well
        foreach (Transform child in transform)
        {
            SceneNode childNode = child.GetComponent<SceneNode>();
            if (childNode != null)
            {
                childNode.CompileTransform(ref combinedParentTransform);
            }
        }

        //For any items in our primitive list, compile their new position
        if (PrimitiveList.Count > 0)
        {
            foreach (NodePrimitive primitive in PrimitiveList)
            {
                primitive.LoadShaderMatrix(ref combinedParentTransform);
            }
        }
    }

    /// <summary>
    /// Determine whether or not the camera is currently looking at an object.
    /// TODO: Need to determine what we want to do exactly when the camera is looking at an object
    /// </summary>
    /// <param name="nodeMatrix"></param>
    /// <param name="cameraTransform"></param>
    /// <returns></returns>
    public List<NodePrimitive> ObjectLookedAt(Transform cameraTransform)
    {
        List<NodePrimitive> objectsLookedAt = new List<NodePrimitive>();
        //For any child nodes, we need to tell them to compile their new position as well
        foreach (Transform child in transform)
        {
            SceneNode childNode = child.GetComponent<SceneNode>();
            if (childNode != null)
            {
                objectsLookedAt.AddRange(childNode.ObjectLookedAt(cameraTransform));
            }
        }

        //For any items in our primitive list, compile their new position
        if (PrimitiveList.Count > 0)
        {
            foreach (NodePrimitive primitive in PrimitiveList)
            {
                bool lookedAt = primitive.ObjectLookedAt(ref combinedParentTransform, cameraTransform);
                if (lookedAt)
                {
                    objectsLookedAt.Add(primitive);
                }
            }
        }
        return objectsLookedAt;
    }
}
