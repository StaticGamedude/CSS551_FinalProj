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
    private float SELECTION_BUFFER = 0.5f;

    /// <summary>
    /// Pivit position of the object (point at which the object rotates around)
    /// </summary>
    public Vector3 PivotPosition;

    /// <summary>
    /// Color of the object
    /// </summary>
    public Color PrimitiveColor = Color.blue;

    /// <summary>
    /// Type of look at detection this object uses
    /// </summary>
    public PrimitiveDetectionType detectionType = PrimitiveDetectionType.SPHERE;

    // Start is called before the first frame update
    void Start() { }

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
        Matrix4x4 combinedMatrix = ComputeTransform(ref nodeMatrix);
        GetComponent<Renderer>().material.SetMatrix("XformMat", combinedMatrix);
        GetComponent<Renderer>().material.SetColor("desiredColor", PrimitiveColor);
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

        if (centerToLinearPoint.magnitude < ((scale.x / 2) + SELECTION_BUFFER))
        {
            PrimitiveColor = Color.green;
        }
        else
        {
            PrimitiveColor = Color.white;
        }
        return false;
    }

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
        Vector3 cylinderBottomPos = position - ((scale.y / 2) * primitiveUp);
        Vector3 cylinderTopPos = position + ((scale.y / 2) * primitiveUp);
        Vector3 cylinderLineVec = cylinderTopPos - cylinderBottomPos;
        float posProjectionOnView = Vector3.Dot(camToPrimitive, cameraTransform.forward);
        Vector3 linearViewPosToObject = cameraTransform.position + (posProjectionOnView * cameraTransform.forward);
        Vector3 centerToLinearPoint = linearViewPosToObject - position;

        if (centerToLinearPoint.magnitude < cylinderLineVec.magnitude / 2)
        {
            PrimitiveColor = Color.yellow;
        }
        else
        {
            PrimitiveColor = Color.white;
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
        
        return false;
    }

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
}
