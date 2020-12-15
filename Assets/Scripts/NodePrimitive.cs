/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility enum to help determine what type of look at detection we should use
/// </summary>
public enum PrimitiveDetectionType
{
    SPHERE = 0,
    CYLINDER = 1,
}

public class NodePrimitive : MonoBehaviour
{
    //Amount of extra padding to help the user select the object (so they don't have to be so precise to look/select an object)
    private const float SELECTION_BUFFER = 0.5f;

    /// <summary>
    /// Minimum distance away the camera needs to be in order to select the object
    /// </summary>
    private const float ALLOWABLE_SELECTION_DISTANCE = 10;

    /// <summary>
    /// Pivit position of the object (point at which the object rotates around)
    /// </summary>
    public Vector3 PivotPosition;

    /// <summary>
    /// Desired color of the object
    /// </summary>
    public Color PrimitiveColor = Color.blue;

    /// <summary>
    /// Type of look at detection this object uses
    /// </summary>
    public PrimitiveDetectionType detectionType = PrimitiveDetectionType.SPHERE;

    /// <summary>
    /// Current combined transform for this primitive
    /// </summary>
    public Matrix4x4 currentTransform;

    /// <summary>
    /// Flat to determine whether is object is selectable
    /// </summary>
    private bool selectable = false;

    /// <summary>
    /// Tranform of the main camera. This tranform is used to become the parent of this primitive
    /// if the user is attempting to hold this object to move it around
    /// </summary>
    public Transform parentCameraTransform = null;

    /// <summary>
    /// Parent of this node primitive. Value is stored so that it can be reassigned to when the user
    /// releases this primitive
    /// </summary>
    private Transform origParent;

    /// <summary>
    /// Current color of the primitive. Changes if the object is being looked at or interacted with
    /// </summary>
    private Color currentDisplayColor;

    /// <summary>
    /// Reference to the accessory component of the node primitives.
    /// </summary>
    private SnowmanAccessory snowmanAccessory;

    // Start is called before the first frame update
    void Start() 
    {
        snowmanAccessory = GetComponent<SnowmanAccessory>();
        currentDisplayColor = PrimitiveColor;
        GetComponent<Renderer>().material.SetFloat("transparentObj", snowmanAccessory.ContainerObj ? 0f : 1f);

        Debug.Assert(snowmanAccessory != null); //Expected to be on the object already or added during NodePrimitive creation
    }

    // Update is called once per frame
    void Update() { }

    /// <summary>
    /// Compute the official primitive transform (incorporates the primitive's pivot position)
    /// </summary>
    /// <param name="nodeMatrix"></param>
    /// <returns></returns>
    private Matrix4x4 ComputeTransform(ref Matrix4x4 nodeMatrix)
    {
        Matrix4x4 moveToPivotMat = Matrix4x4.TRS(PivotPosition, Quaternion.identity, Vector3.one);
        Matrix4x4 moveOppOfPivotMat = Matrix4x4.TRS(-PivotPosition, Quaternion.identity, Vector3.one);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        Matrix4x4 combinedMatrix = nodeMatrix * moveToPivotMat * trs * moveOppOfPivotMat;
        return combinedMatrix;
    }

    /// <summary>
    /// Compute the transform for this primitive, then load the value into the shader so that it can be rendered
    /// </summary>
    /// <param name="nodeMatrix"></param>
    public void LoadShaderMatrix(ref Matrix4x4 nodeMatrix)
    {
        if (parentCameraTransform == null)
        {
            currentTransform = ComputeTransform(ref nodeMatrix);
            if (snowmanAccessory != null)
            {
                snowmanAccessory.UpdatePosition(ref currentTransform);
            }
            else
            {
                /** Note: This else case should only happen when viewing the scene in the Editor with the 
                 * [ExecuteInEditMode]. the snowmanAccessory component is recognized on start which results in a 
                 * NullReferenceException while working on the scene. 
                 */
                GetComponent<Renderer>().material.SetMatrix("XformMat", currentTransform);
            }
        }
        else
        {
            Matrix4x4 holdingTRS = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            snowmanAccessory.UpdatePosition(ref holdingTRS);
        }
    }

    /// <summary>
    /// Determines whether the camera is looking at this node primitive. 
    /// TODO: The detection only works for spheres so far
    /// </summary>
    /// <param name="nodeMatrix"></param>
    /// <param name="cameraTransform"></param>
    /// <returns></returns>
    public bool ObjectLookedAt(ref Matrix4x4 nodeMatrix, Transform cameraTransform)
    {
        if (parentCameraTransform == null)
        {
            Matrix4x4 combinedTransform = ComputeTransform(ref nodeMatrix);
            BreakdownTransform(combinedTransform, out Vector3 position, out Quaternion rotation, out Vector3 scale);
            switch (detectionType)
            {
                case PrimitiveDetectionType.CYLINDER:
                    return DetermineCylinderLookat(ref position, ref rotation, ref scale, cameraTransform);
                case PrimitiveDetectionType.SPHERE:
                default:
                    return DetermineSphereLookAt(ref position, ref rotation, ref scale, cameraTransform);
            }

            /**The code below is commented since it may be revisited later on if time allowed. The code below
            * attempts to help support scaling of the scene nodes by allowing manipulation of scale node primitives
            */
            //Matrix4x4 inverseParents = nodeMatrix.inverse;
            //Matrix4x4 cameraMat = Matrix4x4.TRS(cameraTransform.position, cameraTransform.rotation, cameraTransform.localScale);
            //cameraMat = inverseParents * cameraMat;
        }
        return false;
    }

    /// <summary>
    /// Handles the look at detection for sphere node primitives
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="cameraTransform"></param>
    /// <returns></returns>
    private bool DetermineSphereLookAt(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, Transform cameraTransform)
    {
        Vector3 camToPrimitive = position - cameraTransform.position;
        float posProjectionOnView = Vector3.Dot(camToPrimitive, cameraTransform.forward);
        Vector3 linearViewPosToObject = cameraTransform.position + (posProjectionOnView * cameraTransform.forward);
        Vector3 centerToLinearPoint = linearViewPosToObject - position;

        if (centerToLinearPoint.magnitude < (scale.x / 2) && camToPrimitive.magnitude < ALLOWABLE_SELECTION_DISTANCE)
        {
            selectable = true;
            SetColor(Color.green);
        }
        else
        {
            selectable = false;
            SetColor(PrimitiveColor);
        }
        return selectable;
    }

    /**The code below is commented since it may be revisited later on if time allowed. The code below
     * attempts to help support scaling of the scene nodes by allowing manipulation of scale node primitives
     */
    //private bool DetermineSphereLookAt(Matrix4x4 cameraTransform)
    //{
    //    Matrix4x4 identity = Matrix4x4.identity;
    //    Matrix4x4 trs = ComputeTransform(ref identity);
    //    GameObject tempObject = new GameObject();
        
    //    BreakdownTransform(trs, out Vector3 objectPos, out Quaternion objRotation, out Vector3 objScale);
    //    BreakdownTransform(cameraTransform, out Vector3 camPos, out Quaternion camRotation, out Vector3 camScale);
    //    tempObject.transform.rotation = camRotation;

    //    Vector3 camToPrimitive = objectPos - camPos;
    //    float posProjectionOnView = Vector3.Dot(camToPrimitive, tempObject.transform.forward);
    //    Vector3 linearViewPosToObject = camPos + (posProjectionOnView * tempObject.transform.forward);
    //    Vector3 centerToLinearPoint = linearViewPosToObject - objectPos;

    //    //objScale.x; //Each scale value should be the same;
    //    if (centerToLinearPoint.magnitude < (objScale.x / 2) && camToPrimitive.magnitude < ALLOWABLE_SELECTION_DISTANCE)
    //    {
    //        selectable = true;
    //        currentDisplayColor = Color.green;
    //    }
    //    else
    //    {
    //        selectable = false;
    //        currentDisplayColor = PrimitiveColor;
    //    }

    //    if (Application.isEditor)
    //    {
    //        /** 
    //         * Since we use [ExecuteInEditMode] to be able to see all of our objects while working on the scene, it seems that Unity is unable
    //         * to delete objects using the normal Destory method while in the editor mode. Rather, DestoryImmediate is needed. Not doing this, yields to
    //         * empty game objects in the scene every time the application is run.
    //         */
    //        DestroyImmediate(tempObject);
    //    }
    //    else
    //    {
    //        Destroy(tempObject);
    //    }

    //    return selectable;
    //}

    /// <summary>
    /// Handles the look at detection for cylinder node primitives
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="cameraTransform"></param>
    /// <returns></returns>
    private bool DetermineCylinderLookat(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, Transform cameraTransform)
    {
        GameObject gameObject = new GameObject();
        Vector3 camToPrimitive = position - cameraTransform.position;

        gameObject.transform.rotation = rotation; //Set the rotation to an empty game object to try to determine the up vector of the cylinder

        Vector3 primitiveUp = gameObject.transform.up;
        Vector3 cylinderBottomPos = position - (scale.y * primitiveUp);
        Vector3 cylinderTopPos = position + (scale.y * primitiveUp);
        Vector3 cylinderLineVec = cylinderTopPos - cylinderBottomPos;
        float posProjectionOnView = Vector3.Dot(camToPrimitive, cameraTransform.forward);
        Vector3 linearViewPosToObject = cameraTransform.position + (posProjectionOnView * cameraTransform.forward);
        Vector3 centerToLinearPoint = linearViewPosToObject - position;

        if (centerToLinearPoint.magnitude < cylinderLineVec.magnitude / 2 && camToPrimitive.magnitude < ALLOWABLE_SELECTION_DISTANCE)
        {
            selectable = true;
            SetColor(Color.green);
        }
        else
        {
            selectable = false;
            SetColor(PrimitiveColor);
        }

        if (Application.isEditor)
        {
            /** 
             * Since we use [ExecuteInEditMode] to be able to see all of our objects while working on the scene, it seems that Unity is unable
             * to delete objects using the normal Destory method while in the editor mode. Rather, DestoryImmediate is needed. Not doing this, yields to
             * empty game objects in the scene every time the application is run.
             */
            DestroyImmediate(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        return selectable;
    }

    /**The code below is commented since it may be revisited later on if time allowed. The code below
     * attempts to help support scaling of the scene nodes by allowing manipulation of scale node primitives
     */
    //private bool DetermineCylinderLookat(Matrix4x4 cameraTransform)
    //{
    //    Matrix4x4 identity = Matrix4x4.identity;
    //    Matrix4x4 trs = ComputeTransform(ref identity);

    //    BreakdownTransform(trs, out Vector3 objectPos, out Quaternion objRotation, out Vector3 objScale);
    //    BreakdownTransform(cameraTransform, out Vector3 camPos, out Quaternion camRotation, out Vector3 camScale);


    //    GameObject tempCopyObj = new GameObject();
    //    GameObject tempCameraObj = new GameObject();
    //    Vector3 camToPrimitive = objectPos - camPos;

    //    tempCopyObj.transform.rotation = objRotation; //Set the rotation to an empty game object to try to determine the up vector of the cylinder
    //    tempCameraObj.transform.rotation = camRotation;

    //    Vector3 primitiveUp = tempCopyObj.transform.up;
    //    Vector3 cylinderBottomPos = objectPos - (objScale.y * primitiveUp);
    //    Vector3 cylinderTopPos = objectPos + (objScale.y * primitiveUp);
    //    Vector3 cylinderLineVec = cylinderTopPos - cylinderBottomPos;
    //    float posProjectionOnView = Vector3.Dot(camToPrimitive, tempCameraObj.transform.forward);
    //    Vector3 linearViewPosToObject = camPos + (posProjectionOnView * tempCameraObj.transform.forward);
    //    Vector3 centerToLinearPoint = linearViewPosToObject - objectPos;

    //    if (centerToLinearPoint.magnitude < cylinderLineVec.magnitude / 2 && camToPrimitive.magnitude < ALLOWABLE_SELECTION_DISTANCE)
    //    {
    //        selectable = true;
    //        currentDisplayColor = Color.green;
    //    }
    //    else
    //    {
    //        selectable = false;
    //        currentDisplayColor = PrimitiveColor;
    //    }

    //    if (Application.isEditor)
    //    {
    //        /** 
    //         * Since we use [ExecuteInEditMode] to be able to see all of our objects while working on the scene, it seems that Unity is unable
    //         * to delete objects using the normal Destory method while in the editor mode. Rather, DestoryImmediate is needed. Not doing this, yields to
    //         * empty game objects in the scene every time the application is run.
    //         */
    //        DestroyImmediate(tempCopyObj);
    //    }
    //    else
    //    {
    //        Destroy(tempCopyObj);
    //    }

    //    return selectable;
    //}

    /// <summary>
    /// Get the position, rotation, and scale values of a given matrix transform
    /// </summary>
    /// <param name="nodeMatrix"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    private void BreakdownTransform(Matrix4x4 nodeMatrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
    {
        Vector3 x = nodeMatrix.GetColumn(0);
        Vector3 y = nodeMatrix.GetColumn(1);
        Vector3 z = nodeMatrix.GetColumn(2);

        scale = new Vector3(x.magnitude, y.magnitude, z.magnitude);
        position = nodeMatrix.GetColumn(3);
        rotation = Quaternion.LookRotation(z / scale.z, y / scale.y);
    }

    /// <summary>
    /// Perform actions necessary before the user to moves the object. We need to update the local position of this object in a 
    /// way such that the object doesn't move in the world space
    /// </summary>
    /// <param name="cameraTransform"></param>
    public void PerformHoldActions(Transform cameraTransform)
    {
        BreakdownTransform(currentTransform, out Vector3 position, out Quaternion rotation, out Vector3 scale);
        origParent = transform.parent;
        transform.parent = null;
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        transform.parent = cameraTransform;
        parentCameraTransform = cameraTransform;
    }

    public bool ReleaseObject(Matrix4x4 nodeMatrix)
    {
        Debug.Log("NodePrimitive.cs is calling ReleaseObject() for " + gameObject.name); 
        if (parentCameraTransform != null)
        {
            BreakdownTransform(currentTransform, out Vector3 originalPosition, out Quaternion originalRotation, out Vector3 originalScale);

            Vector3 changeInPosition = transform.position - originalPosition;
            Vector3 changeInScale = transform.localScale - originalScale;
            //TOOD: Add support for change in rotation
            //TODO: Add support for change in scale

            Matrix4x4 userDelta = Matrix4x4.TRS(changeInPosition, Quaternion.identity, changeInScale);
            Matrix4x4 updatedPos = Matrix4x4.identity;

            updatedPos = nodeMatrix.inverse * userDelta;
            BreakdownTransform(updatedPos, out Vector3 newPos, out Quaternion newRotation, out Vector3 newScale);

            transform.parent = origParent;
            transform.localPosition = originalPosition + newPos;
            
            parentCameraTransform = null;
            if(gameObject.name.Equals("TopHat"))
            {
                GetComponent<SmallCamActivation>().ShowCam(); 
            }
            return true;
        }
        return false;
        
    }

    /// <summary>
    /// Set the color of the game object and any related child objects
    /// </summary>
    /// <param name="color"></param>
    private void SetColor(Color color)
    {
        if (snowmanAccessory != null)
        {
            GetComponent<Renderer>().material.SetColor("desiredColor", color);
            snowmanAccessory.SetColor(color);
            currentDisplayColor = color;
        }
    }
}
