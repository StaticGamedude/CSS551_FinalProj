using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanAccessory : MonoBehaviour
{
    // Start is called before the first frame update
    public Color PrimaryColor;

    public PrimitiveDetectionType DetectionType;

    public bool ContainerObj = false;

    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void SetColor(Color color)
    {
        SetColorToWholeObject(gameObject, color, !ContainerObj);
    }

    private void SetColorToWholeObject(GameObject obj, Color color, bool colorSelf)
    {
        if (obj != null)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (colorSelf && renderer != null && renderer.material != null)
            {
                renderer.material.SetColor("desiredColor", color);
            }

            foreach(Transform child in obj.transform)
            {
                SetColorToWholeObject(child.gameObject, color, true);
            }
        }
    } 

    public void UpdatePosition(ref Matrix4x4 nodeMatrix)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.SetMatrix("XformMat", nodeMatrix);
            int count = 0;
            foreach (Transform child in transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    Matrix4x4 childTRS = Matrix4x4.TRS(child.localPosition, child.localRotation, child.localScale);
                    Matrix4x4 childTransform = nodeMatrix * childTRS;
                    childRenderer.material.SetMatrix("XformMat", childTransform);
                }
                count++;
            }
        }
    }

    public void UpdateMaterial(Material mat)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;

            foreach (Transform child in transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.material = mat;
                    childRenderer.material.SetFloat("transparentObj", 1f);
                }
            }
        }

    }
}
