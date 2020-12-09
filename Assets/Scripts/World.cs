/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SnowmanNodes
{
    BASE = 0,
    TORSO = 1,
    HEAD = 2,
}

[ExecuteInEditMode]
public class World : MonoBehaviour
{
    //Variables to be set in the Unity Editor
    public SceneNode BaseNode; //Reference to the base scene node in the world

    public Camera mainCam; //Reference to the main camera

    public Material stardardObjectMat; //Reference to our custom shader

    /// <summary>
    /// List of node primitives that are currently in the "view range" of the main camera
    /// </summary>
    private List<NodePrimitive> lookedAtPrimitives = new List<NodePrimitive>();

    /// <summary>
    /// Flag to indicate whether an object is being held in the world
    /// </summary>
    private bool objectHeld = false;

    /// <summary>
    /// Collection of the possible snowman nodes. This array is not expected to change during runtime
    /// </summary>
    private SnowmanNodes[] snowmanNodes = { SnowmanNodes.BASE, SnowmanNodes.TORSO, SnowmanNodes.HEAD };

    /// <summary>
    /// Current index referencing the collection of snowman node options.
    /// </summary>
    private int currentNodeIndex = 0;

    /// <summary>
    /// Flag to inciate whether or not we should skip over the logic for determining if we're looking at primitives.
    /// </summary>
    private bool skipLookatBehavior = false;

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

        if (!objectHeld && !skipLookatBehavior)
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

    /// <summary>
    /// Change the node selection of the snowman. Cylces through the different node options
    /// </summary>
    public void ChangeSnowmanNode()
    {
        if (currentNodeIndex == snowmanNodes.Length - 1)
        {
            currentNodeIndex = 0;
        }
        else
        {
            currentNodeIndex++;
        }
    }

    /// <summary>
    /// Get the display name of the current snowman node
    /// </summary>
    /// <returns></returns>
    public string GetCurrentSnowmanNode()
    {
        return GetSnowmanNodeName(snowmanNodes[currentNodeIndex]);
    }

    /// <summary>
    /// Get the display name of a given snowman scene node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private string GetSnowmanNodeName(SnowmanNodes node)
    {
        switch(node)
        {
            case SnowmanNodes.BASE:
                return "Base";
            case SnowmanNodes.TORSO:
                return "Torso";
            case SnowmanNodes.HEAD:
                return "Head";
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// Get the current snowman scene node object
    /// </summary>
    /// <returns></returns>
    private SceneNode GetSnowmanSceneNode()
    {
        SnowmanNodes currentNodeType = snowmanNodes[currentNodeIndex];
        SceneNode sceneNode = BaseNode.GetSnowmanNode(currentNodeType);

        if (sceneNode == null)
        {
            Debug.LogError("Unable to current scene node: " + GetSnowmanNodeName(currentNodeType));
        }
        return sceneNode;
    }

    /// <summary>
    /// Get the current snowman node local position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentNodePosition()
    {
        Vector3 currentPosition = new Vector3();
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            currentPosition = node.transform.localPosition;
        }
        return currentPosition;
    }

    /// <summary>
    /// Get the current snowman node local scale
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentScale()
    {
        Vector3 currentScale = new Vector3();
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            currentScale = node.transform.localScale;
        }
        return currentScale;
    }

    /// <summary>
    /// Get the current snowman local rotation
    /// </summary>
    /// <returns></returns>
    public Quaternion GetCurrentRotation()
    {
        Quaternion currentRotation = Quaternion.identity;
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            currentRotation = node.transform.localRotation;
        }
        return currentRotation;
    }

    /// <summary>
    /// Get the current snowman node axis of rotation (right, up, forward)
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    public Vector3 GetCurrentRotationAxis(TransformAxis axis)
    {
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            switch (axis)
            {
                case TransformAxis.X:
                    return node.transform.right;
                case TransformAxis.Y:
                    return node.transform.up;
                case TransformAxis.Z:
                    return node.transform.forward;
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Set the snowman's node position
    /// </summary>
    /// <param name="newPosition"></param>
    public void SetSnowmanNodePosition(Vector3 newPosition)
    {
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            node.transform.localPosition = newPosition;
        }
    }

    /// <summary>
    /// Set the snowman's node rotation
    /// </summary>
    /// <param name="newRotation"></param>
    public void SetSnowmanNodeRotation(Quaternion newRotation)
    {
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            node.transform.localRotation = newRotation;
        }
    }

    /// <summary>
    /// Set the snowman's node scale
    /// </summary>
    /// <param name="newScale"></param>
    public void SetSnowmanNodeScale(Vector3 newScale)
    {
        SceneNode node = GetSnowmanSceneNode();
        if (node != null)
        {
            node.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Determines if an object is currently being looked at in the world
    /// </summary>
    /// <returns></returns>
    public bool lookingAtNodePrimitive()
    {
        return lookedAtPrimitives.Count > 0;
    }

    /// <summary>
    /// Adds a game object as a node primitive to the world
    /// </summary>
    /// <param name="snowmanObject"></param>
    public void AddPrimitiveToCurrentNode(GameObject snowmanObject)
    {
        if (snowmanObject != null)
        {
            SnowmanAccessory accessory = snowmanObject.GetComponent<SnowmanAccessory>();
            if (accessory != null)
            {
                snowmanObject.layer = 0; //Change the layer since we no longer want to treat this object as an free object
                snowmanObject.GetComponent<Renderer>().material = stardardObjectMat;
                NodePrimitive primitive = snowmanObject.AddComponent<NodePrimitive>();
                primitive.PrimitiveColor = accessory.PrimaryColor;
                primitive.detectionType = accessory.DetectionType;
                SceneNode node = GetSnowmanSceneNode();
                node.AddPrimitive(primitive);
            }
            else
            {
                Debug.LogError("The object grab is not a valid snowman accessory object");
            }
        }
    }

    /// <summary>
    /// Set the skip look behavior flag. Used primarily when the user is interacting with a new game object
    /// 
    /// TODO: This is sort of hacky and doesn't fit in the MVC model all that well. If time permitted, should 
    /// see if we can find away to maybe track new potential game objects in the world?
    /// </summary>
    /// <param name="skip"></param>
    public void SetSkipLookBehavior(bool skip)
    {
        skipLookatBehavior = skip;
    }
}
