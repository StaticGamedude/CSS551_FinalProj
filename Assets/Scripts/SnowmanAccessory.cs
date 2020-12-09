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
                renderer.material.SetColor("_Color", color);
            }

            foreach(Transform child in obj.transform)
            {
                SetColorToWholeObject(child.gameObject, color, true);
            }
        }
    }
}
