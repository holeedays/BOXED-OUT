using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    #region Modifiable Variables
    [Header("Tilemaps")]
    public Tilemap InteractablesTmap;

    [Header("GameObjects Representing Tiles")]
    public GameObject Wall;
    public GameObject Moveable;

    [Header("Player Move Distance")]
    public float MoveDistance = 1.0f;
    #endregion

    #region Misc 
    // for now, we're going to use a cube to represent our character
    private Vector3 startingPos = new Vector3(-0.5f, 0, -0.5f);
    private bool hasMoved;

    private PlayerInputActionsBase inputSystemControls;
    private InputAction move;
    #endregion

    void Start()
    {
        inputSystemControls = new PlayerInputActionsBase();
        move = inputSystemControls.Player.Move;

        // set position to a normalized area
        this.transform.position = startingPos;
    }

    void Update()
    {
        ToggleKeyBasedMovement();
        UpdatePlayerKeyBased();
    }

    private void UpdatePlayerKeyBased()
    {
        Vector2 input = GetKeyBasedInput();

        // logic for creating grid-based movement so it functions similar to input.GetKeyDown(Button) 
        if (input.magnitude == 0)
        {
            hasMoved = false;
            return;
        }
        if (hasMoved)
        {
            return;
        }

        // get our relative positions so we can do the movement logic
        Vector3 movementIncrement = NormalizeInput(input) * MoveDistance;
        Vector3 targetPos = this.transform.position + movementIncrement;

        GridController.Tile3D? tile3D = GridController.GetTile3D(InteractablesTmap, GridController.WorldToGridPos(InteractablesTmap, targetPos));

        if (tile3D == null)
        {
            targetPos = this.transform.position + movementIncrement;
        }
        else
        {
            // check if the block ahead is a moveable or a wall
            // the reason for casting is because tile3D is nullable, so we have to convert the value into a non nullable type for basic functions to work
            if ((bool)tile3D?.RefersToSimilarGameObject(Wall))
            {
                return;
            }
            else
            {
                // if there is a moveable, check if there is another block ahead
                GridController.Tile3D? furtherTile3D = GridController.GetTile3D(InteractablesTmap, GridController.WorldToGridPos(InteractablesTmap, targetPos + movementIncrement));
                // if there isn't any, we can move the moveable the same direction we were going
                if (furtherTile3D == null)
                {
                    GridController.MoveTile3D((GridController.Tile3D)tile3D, GridController.WorldToGridPos(InteractablesTmap, targetPos + movementIncrement));
                }
                else
                {
                    return;
                }
            }
        }

        Move(targetPos);
        hasMoved = true;
    }

    private Vector2 GetKeyBasedInput()
    {
        return move.ReadValue<Vector2>();
    }

    private Vector3 NormalizeInput(Vector2 rawInput)
    {
        if (rawInput.x != 0)
            return Vector3.right * rawInput.x;
        else 
            return Vector3.forward * rawInput.y;
    }

    private void Move(Vector3 targetPos)
    {
        this.transform.position = targetPos;    
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
}
