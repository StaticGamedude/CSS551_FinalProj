/**
 * Tori Salvatore, Drew Nelson - Final Project
 * CSS 551 - Advance Computer Graphics
 * Autumn 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TransformMode
{
    TRANSLATE = 0,
    SCALE = 1,
    ROTATE = 2,
}

public enum TransformAxis
{
    X = 0,
    Y = 1,
    Z = 2,
}

/// <summary>
/// Handles all the movment and inputs from the user
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// Reference to the UI text for displaying the current snowman node selected in the world
    /// </summary>
    public Text snowmanNodeValueText;

    /// <summary>
    /// Reference to the UI text for dispalying the current transform mode
    /// </summary>
    public Text transformModeValueText;

    /// <summary>
    /// Reference to the UI text for displaying the current transform axis
    /// </summary>
    public Text transformAxisValueText;

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

    /// <summary>
    /// Possible transform mode options
    /// </summary>
    private TransformMode[] transformModeOptions = new TransformMode[] { TransformMode.TRANSLATE, TransformMode.SCALE, TransformMode.ROTATE };

    /// <summary>
    /// Possible transform axis options
    /// </summary>
    private TransformAxis[] transformAxisOptions = new TransformAxis[] { TransformAxis.X, TransformAxis.Y, TransformAxis.Z };

    /// <summary>
    /// Index to help determine what the current transform mode is
    /// </summary>
    private int transformModeIndex = 0;

    /// <summary>
    /// Index to help determine what the current transform axis is
    /// </summary>
    private int transformAxisIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            inputHandler = new WindowsInputHandler();
        }

        Debug.Assert(inputHandler != null);
        Debug.Assert(mainCam != null);
        Debug.Assert(snowmanNodeValueText != null);
        Debug.Assert(transformModeValueText != null);
        Debug.Assert(transformAxisValueText != null);
    }

    // Update is called once per frame
    void Update()
    {
        float forwardMovement = inputHandler.GetZInputMovmeent();
        float sideMovement = inputHandler.GetXInputMovement();

        //Handle Player walking movments
        if (forwardMovement != 0 || sideMovement != 0)
        {
            Vector3 camForward = mainCam.transform.forward;
            camForward.y = 0;

            Vector3 verticalDirection = camForward * PLAYER_MOVEMENT_SPEED * forwardMovement * Time.smoothDeltaTime;
            Vector3 horizontalDirection = mainCam.transform.right * PLAYER_MOVEMENT_SPEED * sideMovement * Time.smoothDeltaTime;
            Vector3 newDirection = verticalDirection + horizontalDirection;

            transform.Translate(newDirection);
        }

        //Handle object manipulation inputs
        if (!world.isObjectHeld())
        {
            if (inputHandler.isChangeNodeButtonPressed())
            {
                world.ChangeSnowmanNode();
            }
            else if (inputHandler.isTransformModeButtonPressed())
            {
                ChangeTransformMode();
            }
            else if (inputHandler.isTransformAxisButtonPressed())
            {
                ChangeTransformAxis();
            }
            else if (inputHandler.isSelectionButtonPressed())
            {
                world.TryHoldObject();
            }
        } 
        else if (inputHandler.isSelectionButtonPressed())
        {
            world.ReleaseObject();
        }

        //Leaving the code below commented for now in case we decided to switch to a "click and hold to move object"
        //as opposed to "click to toggle holding an object"

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







        UpdateDisplays();
    }

    private void UpdateDisplays()
    {
        snowmanNodeValueText.text = world.GetCurrentSnowmanNode();
        transformModeValueText.text = GetTransformModeName(transformModeOptions[transformModeIndex]);
        transformAxisValueText.text = GetTransformAxisName(transformAxisOptions[transformAxisIndex]);
    }

    private string GetTransformAxisName(TransformAxis axis)
    {
        switch(axis)
        {
            case TransformAxis.X:
                return "X";
            case TransformAxis.Y:
                return "Y";
            case TransformAxis.Z:
                return "Z";
            default:
                return string.Empty;
        }
    }

    private string GetTransformModeName(TransformMode mode)
    {
        switch (mode)
        {
            case TransformMode.TRANSLATE:
                return "Translate";
            case TransformMode.SCALE:
                return "Scale";
            case TransformMode.ROTATE:
                return "Rotate";
            default:
                return string.Empty;
        }
    }

    private void ChangeTransformMode()
    {
        if (transformModeIndex == transformModeOptions.Length - 1)
        {
            transformModeIndex = 0;
        }
        else
        {
            transformModeIndex++;
        }
    }

    private void ChangeTransformAxis()
    {
        if (transformAxisIndex == transformAxisOptions.Length - 1)
        {
            transformAxisIndex = 0;
        }
        else
        {
            transformAxisIndex++;
        }
    }
}
