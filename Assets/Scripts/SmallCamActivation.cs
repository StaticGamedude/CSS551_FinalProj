using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCamActivation : MonoBehaviour
{

    public GameObject smallCamQuad;

    public GameObject sceneNode;

    public GameObject directionLine;

    // Start is called before the first frame update
    void Start()
    {
        smallCamQuad.SetActive(false);
        directionLine.SetActive(false);
    }

    public void ShowCam()
    {
        smallCamQuad.SetActive(true);
        directionLine.SetActive(true);
    }

    void OnDisable()
    {
        //smallCamQuad.SetActive(false); 
    }

}
