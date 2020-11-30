/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the movment and inputs from the user
/// </summary>
public class Movement : MonoBehaviour
{
    /// <summary>
    /// Speed at which the user moves around in the world
    /// </summary>
    private const int PLAYER_MOVEMENT_SPEED = 8;

    /// <summary>
    /// Reference to the main camera
    /// </summary>
    public Camera mainCam;

    /// <summary>
    /// Reference to our world game object which tracks the world's behavior
    /// </summary>
    public World world;

    /// <summary>
    /// Input handler which is used to detect user inputs
    /// </summary>
    private InputHandler inputHandler = null;

    public GameObject camForward;

    private bool objectHeld = false;

    
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            inputHandler = new WindowsInputHandler();
        }

        Debug.Assert(inputHandler != null);
        Debug.Assert(mainCam != null);


        camForward = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        camForward.transform.localScale = new Vector3(0.3f, 4, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        float forwardMovement = inputHandler.GetZInputMovmeent();
        float sideMovement = inputHandler.GetXInputMovement();

        if (forwardMovement != 0 || sideMovement != 0)
        {
            Vector3 camForward = mainCam.transform.forward;
            camForward.y = 0;

            Vector3 verticalDirection = camForward * PLAYER_MOVEMENT_SPEED * forwardMovement * Time.smoothDeltaTime;
            Vector3 horizontalDirection = mainCam.transform.right * PLAYER_MOVEMENT_SPEED * sideMovement * Time.smoothDeltaTime;
            Vector3 newDirection = verticalDirection + horizontalDirection;

            transform.Translate(newDirection);
        }

        if (inputHandler.isSelectionButtonPressed())
        {
            if (objectHeld)
            {
                world.ReleaseObject();
                objectHeld = false;
            }
            else
            {
                objectHeld = world.TryHoldObject();
            }
        }

        //if (inputHandler.isSelectionButtonHeld())
        //{
        //    if (!objectHeld)
        //    {
        //        objectHeld = world.TryHoldObject();
        //    }
        //}
        //else if (objectHeld)
        //{
        //    world.ReleaseObject();
        //    objectHeld = false;
        //}

        camForward.transform.localPosition = mainCam.transform.position + (4 * mainCam.transform.forward);
        camForward.transform.localRotation = Quaternion.FromToRotation(Vector3.up, mainCam.transform.forward);
    }
}
