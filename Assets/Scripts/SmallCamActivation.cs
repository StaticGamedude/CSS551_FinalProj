using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCamActivation : MonoBehaviour
{

    public GameObject smallCamQuad;

    public GameObject sceneNode; 


    // Start is called before the first frame update
    void Start()
    {
        smallCamQuad.SetActive(false); 
    }

    void Update()
    {
        // check if tophat is ontop of scene node
        // check if tophat is certain distance from head sphere
        // distance compared to scale
        // if checks are valid, SmallCamActivate(true); 
        /*
        if(Vector3.Distance(sceneNode.transform.position, transform.position) > 0.5 * localScale.y )
        {

        }
        smallCamQuad.SetActive(true);
        */

    }

    void OnDisable()
    {
        smallCamQuad.SetActive(false); 
    }

}
