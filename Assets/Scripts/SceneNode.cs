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
    /// Snowman node that this scene node represents. Expected that this value is 
    /// set in the Unity editor
    /// </summary>
    public SnowmanNodes node;

    /// <summary>
    /// Starting position of our scene node. This position offsets this node's position after the TRS transform
    /// </summary>
    public Vector3 nodeOrigin = Vector3.zero;

    public Transform smallCam; 

    /// <summary>
    /// Combination of the parent's tranform, our pivot position (if one specified), and our scene node's transform (TRS). 
    /// </summary>
    private Matrix4x4 combinedParentTransform;

    /// <summary>
    /// List of Node primitives under this scene node. Contains the object actually displayed in the scene.
    /// </summary>
    public List<NodePrimitive> PrimitiveList;

    /// <summary>
    /// Reference the axis frame prefab
    /// </summary>
    private GameObject axisFramePrefab;

    /// <summary>
    /// Reference to the axis frame game object that is used to help show the orientation of the scene node
    /// </summary>
    private GameObject axisFrame;

    /// <summary>
    /// Flag to indicate whether or not this scene node is the scene node currently selected in the world
    /// </summary>
    private bool selected;

    // Start is called before the first frame update
    void Start() 
    {
        axisFramePrefab = Resources.Load("AxisFrame", typeof(GameObject)) as GameObject;
        Debug.Assert(axisFramePrefab != null);
        UpdateAxisFramePosition();
    }

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

        if (smallCam != null)
        {
            smallCam.localPosition = combinedParentTransform.MultiplyPoint(new Vector3(0f, 0, -4f));
            smallCam.up = transform.up;
            smallCam.forward = -transform.forward;
        }
        UpdateAxisFramePosition();
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

    /// <summary>
    /// Search the appropriate node primitive held and perform the actions necessary
    /// to release the object
    /// </summary>
    /// <returns></returns>
    public bool ReleaseHeldObject()
    {
        Debug.Log("SceneNode.cs is calling ReleaseHeldObject() for " + gameObject.name);
        bool objectFound = false;

        if (PrimitiveList.Count > 0)
        {
            foreach(NodePrimitive primitive in PrimitiveList)
            {
                if (primitive.ReleaseObject(combinedParentTransform))
                {
                    objectFound = true;
                    break;
                }
            }
        }

        if (!objectFound)
        {
            foreach (Transform child in transform)
            {
                SceneNode childNode = child.GetComponent<SceneNode>();
                if (childNode != null)
                {
                    objectFound = childNode.ReleaseHeldObject();
                    if (objectFound)
                    {
                        break;
                    }
                }
            }
        }

        return objectFound;
    }

    /// <summary>
    /// Get the appropriate snowman scene node based on the provided scene node level provided
    /// </summary>
    /// <param name="desiredNode"></param>
    /// <returns></returns>
    public SceneNode GetSnowmanNode(SnowmanNodes desiredNode)
    {
        if (node == desiredNode)
        {
            return this;
        }

        SceneNode nodeFound = null;
        foreach (Transform child in transform)
        {
            SceneNode childNode = child.GetComponent<SceneNode>();
            if (childNode != null)
            {
                nodeFound = childNode.GetSnowmanNode(desiredNode);
                if (nodeFound != null)
                {
                    break;
                }
            }
        }
        return nodeFound;
    }

    /// <summary>
    /// Add a new node primitive to the current snowman scene node
    /// </summary>
    /// <param name="newPrimitive"></param>
    public void AddPrimitive(NodePrimitive newPrimitive)
    {
        if (newPrimitive != null)
        {
            Vector3 currentSceneNodePosition = combinedParentTransform.GetColumn(3);
            Vector3 updatedLocalPosition = newPrimitive.transform.position - currentSceneNodePosition;
            newPrimitive.transform.localPosition = updatedLocalPosition;
            PrimitiveList.Add(newPrimitive);
            newPrimitive.TryEnableSmallCam();
        }
    }

    /// <summary>
    /// Updates the position and orientation of the axis frame. Also takes care of 
    /// instantiated the axis frame if not done so already. Also takes care of removing it if the
    /// scene node is no longer selected
    /// </summary>
    private void UpdateAxisFramePosition()
    {
        if (selected && axisFrame == null)
        {
            axisFrame = Instantiate(axisFramePrefab);
        }
        else if (!selected && axisFrame != null)
        {
            Destroy(axisFrame);
            axisFrame = null;
        }

        if (axisFrame != null)
        {
            axisFrame.transform.localPosition = combinedParentTransform.GetColumn(3);
            Vector3 up = combinedParentTransform.GetColumn(1).normalized;
            Vector3 forward = combinedParentTransform.GetColumn(2).normalized;

            float angle = Mathf.Acos(Vector3.Dot(Vector3.up, up)) * Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(Vector3.up, up);
            axisFrame.transform.localRotation = Quaternion.AngleAxis(angle, axis);

            angle = Mathf.Acos(Vector3.Dot(axisFrame.transform.forward, forward)) * Mathf.Rad2Deg;
            axis = Vector3.Cross(axisFrame.transform.forward, forward);
            axisFrame.transform.localRotation = Quaternion.AngleAxis(angle, axis) * axisFrame.transform.localRotation;
        }
    }

    /// <summary>
    /// Set the selected flag for the scene node
    /// </summary>
    /// <param name="selected"></param>
    public void UpdateSelections(SnowmanNodes selectedNode)
    {
        selected = selectedNode == node;

        foreach (Transform child in transform)
        {
            SceneNode childNode = child.GetComponent<SceneNode>();
            if (childNode != null)
            {
                childNode.UpdateSelections(selectedNode);
            }
        }
    }
}
