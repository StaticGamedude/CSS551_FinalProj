/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePrimitive : MonoBehaviour
{
    private float SELECTION_BUFFER = 0.5f;

    /// <summary>
    /// Pivit position of the object (point at which the object rotates around)
    /// </summary>
    public Vector3 PivotPosition;

    /// <summary>
    /// Color of the object
    /// </summary>
    public Color PrimitiveColor = Color.blue;

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
