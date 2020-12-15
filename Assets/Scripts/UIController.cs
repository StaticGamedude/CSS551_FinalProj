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
    /// Speed at which objects are manipulated in the world using a controller
    /// </summary>
    private const int CONTROLLER_MANIP_SPEED = 5;

    /// <summary>
    /// The object layer referring to game objects that can be added to scene nodes in the world
    /// </summary>
    private const int SNOWMAN_OBJECT_LAYER = 8;

    /// <summary>
    /// The object layer referring to light source game objects that can be moved around
    /// </summary>
    private const int LIGHT_OBJECT_LAYER = 9;

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
    private TransformMode[] transformModeOptions = new TransformMode[] { TransformMode.TRANSLATE, TransformMode.ROTATE };

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

    /// <summary>
    /// Reference to the game object being held as the users moves it around in the scene (either snowman accessory or light source)
    /// </summary>
    private GameObject worldObject = null;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            inputHandler = new WindowsInputHandler();
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            inputHandler = new MYGTInputHandler();
            //TODO: Guessing we'll need a flag here to determine if we should be referencing an occulus controller instead
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
        // Debug.Log("uicontroller"); 
        HandlePlayerMovementInput();
        HandleSceneNodeManipluation();
        HandleNodePrimitiveManipulation();
        HandleNewObjectManipulation();
        UpdateDisplays();
    }

    /// <summary>
    /// Handles any inputs that are associated with the player movement
    /// </summary>
    private void HandlePlayerMovementInput()
    {
        float forwardMovement = inputHandler.GetZInputMovmeent();
        float sideMovement = inputHandler.GetXInputMovement();
        float upMovement = inputHandler.GetYInputMovement();

        //Handle Player walking movments
        if (forwardMovement != 0 || sideMovement != 0 || upMovement != 0)
        {
            Vector3 camForward = mainCam.transform.forward;
            camForward.y = 0;

            Vector3 verticalDirection = camForward * PLAYER_MOVEMENT_SPEED * forwardMovement * Time.smoothDeltaTime;
            Vector3 horizontalDirection = mainCam.transform.right * PLAYER_MOVEMENT_SPEED * sideMovement * Time.smoothDeltaTime;
            Vector3 upwardDireciton = new Vector3(0, PLAYER_MOVEMENT_SPEED * upMovement * Time.smoothDeltaTime, 0);
            Vector3 newDirection = verticalDirection + horizontalDirection + upwardDireciton;

            transform.Translate(newDirection);
        }
    }
    
    /// <summary>
    /// Handle any inputs that are associated with independent scene node manipulation
    /// </summary>
    private void HandleSceneNodeManipluation()
    {
        if (inputHandler.isIncrementButtonPressed())
        {
            UpdateNodeTransform(CONTROLLER_MANIP_SPEED);
        }
        else if (inputHandler.isDecrementButtonPressed())
        {
            UpdateNodeTransform(-1 * CONTROLLER_MANIP_SPEED);
        }
    }

    /// <summary>
    /// Handle any inputs that are associated with manipulating any node primitives
    /// </summary>
    private void HandleNodePrimitiveManipulation()
    {
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
            Debug.Log("uicontroller releaseobject called");
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
    }

    /// <summary>
    /// Handles the inputs for interacting with game objects that can become node primitives in our world. Also handles input for handling the light source
    /// in the scene
    /// </summary>
    private void HandleNewObjectManipulation()
    {
        if (!world.lookingAtNodePrimitive())
        {
            if (inputHandler.isSelectionButtonPressed())
            {
                if (worldObject == null)
                {
                    RaycastHit hit;
                    int snowmanObjectLayer = 1 << SNOWMAN_OBJECT_LAYER;
                    int lightObjectLayer = 1 << LIGHT_OBJECT_LAYER;
                    if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, Mathf.Infinity, snowmanObjectLayer))
                    {
                        worldObject = hit.collider.gameObject;
                        GameObject clonedObject = Instantiate(worldObject); //Duplicate the game object so that users can add a new of of this object later
                        clonedObject.transform.position = worldObject.transform.position;
                        clonedObject.transform.parent = worldObject.transform.parent;
                        worldObject.transform.parent = mainCam.transform;
                        world.SetSkipLookBehavior(true);
                    }
                    else if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, Mathf.Infinity, lightObjectLayer))
                    {
                        worldObject = hit.collider.gameObject;
                        worldObject.transform.parent = mainCam.transform;
                    }
                }
                else
                {
                    worldObject.transform.parent = null;
                    if (worldObject.layer == SNOWMAN_OBJECT_LAYER)
                    {
                        world.AddPrimitiveToCurrentNode(worldObject);
                        world.SetSkipLookBehavior(false);
                    }
                    worldObject = null;
                    
                }
            }
        }
    }

    /// <summary>
    /// Update the lables in the world to provide information to the user as to what the current settings are
    /// </summary>
    private void UpdateDisplays()
    {
        snowmanNodeValueText.text = world.GetCurrentSnowmanNode();
        transformModeValueText.text = GetTransformModeName(transformModeOptions[transformModeIndex]);
        transformAxisValueText.text = GetTransformAxisName(transformAxisOptions[transformAxisIndex]);
    }

    /// <summary>
    /// Get the display name of a given axis type
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the display name of a given transform mode
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Change the transform mode. Cylcles through the transform mode options
    /// </summary>
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

    /// <summary>
    /// Change the transform axis. Cycles through the tranform axis options
    /// </summary>
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

    /// <summary>
    /// Update the transform of the currently selected snowman scene node
    /// </summary>
    /// <param name="speed"></param>
    private void UpdateNodeTransform(int speed)
    {
        TransformMode currentTransformMode = transformModeOptions[transformModeIndex];
        TransformAxis currentTransformAxis = transformAxisOptions[transformAxisIndex];

        switch(currentTransformMode)
        {
            case TransformMode.TRANSLATE:
                TranslateNode(currentTransformAxis, speed);
                break;
            case TransformMode.SCALE:
                ScaleNode(currentTransformAxis, speed);
                break;
            case TransformMode.ROTATE:
                RotateNode(currentTransformAxis, speed);
                break;
        }
    }

    /// <summary>
    /// Translate the currently selected snowman scene node
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="speed"></param>
    private void TranslateNode(TransformAxis axis, int speed)
    {
        Vector3 currentNodePosition = world.GetCurrentNodePosition();
        switch(axis)
        {
            case TransformAxis.X:
                currentNodePosition.x += speed * Time.smoothDeltaTime;
                break;
            case TransformAxis.Y:
                currentNodePosition.y += speed * Time.smoothDeltaTime;
                break;
            case TransformAxis.Z:
                currentNodePosition.z += speed * Time.smoothDeltaTime;
                break;
        }
        world.SetSnowmanNodePosition(currentNodePosition);
    }

    /// <summary>
    /// Rotate the currently selected snowman scene node
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="speed"></param>
    private void RotateNode(TransformAxis axis, int speed)
    {
        Vector3 rotationAxis = world.GetCurrentRotationAxis(axis);
        Quaternion currentRotation = world.GetCurrentRotation();
        Quaternion appliedRotation = Quaternion.AngleAxis(speed, rotationAxis);
        currentRotation = appliedRotation * currentRotation;
        world.SetSnowmanNodeRotation(currentRotation);
    }

    /// <summary>
    /// Scale the currently selected snowman scene node
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="speed"></param>
    private void ScaleNode(TransformAxis axis, int speed)
    {
        Vector3 currentNodeScale = world.GetCurrentScale();
        switch (axis)
        {
            case TransformAxis.X:
                currentNodeScale.x += speed * Time.smoothDeltaTime;
                break;
            case TransformAxis.Y:
                currentNodeScale.y += speed * Time.smoothDeltaTime;
                break;
            case TransformAxis.Z:
                currentNodeScale.z += speed * Time.smoothDeltaTime;
                break;
        }
        world.SetSnowmanNodeScale(currentNodeScale);
    }
}
