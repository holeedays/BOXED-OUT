using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region Instance Setup
    public static PlayerController Instance { get; private set; }
    #endregion

    #region Modifiable Variables
    [Header("Player Move Distance")]
    public float MoveDistance = 1.0f;

    [Header("Gyroscope-based Movement Variables")]
    [Range(0f, 90f)]
    public float AngularThreshold;

    #endregion

    #region Misc 
    // following 2 field are to aid in tile-based movment (prevents consistent movement)
    private bool hasMoved;
    private bool hasBeenUndone;

    // used for gyroscopic purposes
    private Vector3 previousRot;

    // player input 
    private PlayerInputActionsBase inputSystemControls;
    private InputAction move;
    private InputAction undo;
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        ToggleKeyBasedMovement();
        CheckForUndos();

        if (move.enabled) 
            UpdatePlayerKeyBased();
        else
        {
            UpdatePlayerGyroBased();
        }
    }

    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    private void Setup()
    {
        inputSystemControls = new PlayerInputActionsBase();
        move = inputSystemControls.Player.Move;
        undo = inputSystemControls.Player.Undo;

        undo.Enable();
    }

    private void UpdatePlayerKeyBased()
    {
        Vector3 rawInput = GetKeyBasedInput();

        // logic for creating grid-based movement so it functions similar to normalizedInput.GetKeyDown(Button) 
        if (rawInput.magnitude == 0)
        {
            hasMoved = false;
            return;
        }
        if (hasMoved)
        {
            return;
        }

        Vector3 normalizedInput  = NormalizeKeyBasedInput(rawInput);
        UpdatePosition(normalizedInput);
        hasMoved = true;
    }

    private void UpdatePlayerGyroBased()
    {
        Vector3 rawInput = GetGyroscopeBasedInput();

        if (!GyroIsBeingRotated(rawInput))
        {
            hasMoved = false;
            return;
        }

        Vector3 normalizedInput = NormalizeGyroBasedInput(rawInput);
        UpdatePosition(normalizedInput);

        previousRot = rawInput;
        hasMoved = true;
    }

    private void CheckForUndos()
    {
        if (LevelManager.Instance == null)
        {
            Debug.Log("Level manager instance doesn't exist, player cannot undo moves.");
            return;
        }

        if (!UndoKeyPressed())
        {
            hasBeenUndone = false;
            return;
        }

        if (!hasBeenUndone)
        {
            Debug.Log("Move has been undone.");

            LevelManager.Instance.RevertToPreviousState();
            hasBeenUndone = true;
        }

    }

    // Universal/Multifunctional Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void UpdatePosition(Vector3 normalizedInput)
    {
        if (LevelManager.Instance == null)
        {
            Debug.Log("There is no level manager found, player cannot move");
            return;
        }

        int movementCount = 0;

        // get our relative positions so we can do the movement logic
        Vector3 movementIncrement = normalizedInput * MoveDistance;
        Vector3 targetPos = this.transform.position + movementIncrement;

        // check if there are any tile3Ds in the tile the player is going to
        IList<GridController.Tile3D> tile3Ds = GridController.GetAllTile3D(
                                                                    LevelManager.Instance.InteractablesTmap, 
                                                                    GridController.WorldToGridPos(LevelManager.Instance.InteractablesTmap, targetPos));

        foreach(GridController.Tile3D tile3D in tile3Ds)
        {
            // if a wall is identified
            if (tile3D.RefersToSimilarGameObject(LevelManager.Instance.Wall))
            {
                // it is confirmed you cannot move
                return;
            }
            // else if a moveable is identified
            else if (tile3D.RefersToSimilarGameObject(LevelManager.Instance.Moveable))
            {
                // check all the tile3Ds in the tile one step away
                IList<GridController.Tile3D> furtherTile3Ds = GridController.GetAllTile3D(
                                                                                    LevelManager.Instance.InteractablesTmap, 
                                                                                    GridController.WorldToGridPos(LevelManager.Instance.InteractablesTmap, targetPos + movementIncrement));
                // check if a wall or moeveable is contained in this tile
                foreach(GridController.Tile3D furtherTile3D in furtherTile3Ds)
                {
                    if (furtherTile3D.RefersToSimilarGameObject(LevelManager.Instance.Wall) || furtherTile3D.RefersToSimilarGameObject(LevelManager.Instance.Moveable))
                    {
                        return;
                    }
                }

                // if there isn't a wall or moveable, we're free to push the moveable block and move as well; I know this method is multifunctional, but it's simple enough
                tile3D.Move(GridController.WorldToGridPos(LevelManager.Instance.InteractablesTmap, targetPos + movementIncrement));
                movementCount++;
            }
        }

        movementCount++;
        LevelManager.Instance.LogMovementVectorAndCount((movementIncrement, movementCount));

        // now actually move the player
        Move(targetPos);
    }

    private void Move(Vector3 targetPos)
    {
        this.transform.position = targetPos;
    }

    // Gyroscope-Based Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private Vector3 GetGyroscopeBasedInput()
    {
        // check if the instance doesn't exist or the port is not open
        if (SerialTest.Instance == null || !SerialTest.Instance.Sr.PortIsActive) 
            return Vector2.zero;

        return SerialTest.Instance.GetParsedSerialData();
    }

    private Vector3 NormalizeGyroBasedInput(Vector3 rawInput)
    {
        Vector3 diffRot = rawInput - previousRot;

        if (diffRot.x > diffRot.z)
            return Vector3.right * diffRot.x / Mathf.Abs(diffRot.x);
        else
            return Vector3.forward * diffRot.x / Mathf.Abs(diffRot.x);
    }

    private bool GyroIsBeingRotated(Vector3 inputRot)
    {
        Vector3 rotDiff = inputRot - previousRot;

        return rotDiff.magnitude > AngularThreshold;
    }

    // Key-Based Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private Vector2 GetKeyBasedInput()
    {
        return move.ReadValue<Vector2>();
    }

    private Vector3 NormalizeKeyBasedInput(Vector2 rawInput)
    {
        if (rawInput.x != 0)
            return Vector3.right * rawInput.x;
        else
            return Vector3.forward * rawInput.y;
    }


    // allows me to use standard wasd or arrow keys to move
    private void ToggleKeyBasedMovement()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        if (!move.enabled)
        {
            move.Enable();
            Debug.Log("Key based movement toggled on");
        }
        else
        {
            move.Disable();
            Debug.Log("Key based movement toggled off");
        }
    }

    private bool UndoKeyPressed()
    {
        return undo.ReadValue<float>() == 1 ? true : false;
    }
}
