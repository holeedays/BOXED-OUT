using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Tilemap Tmap;
    #region Misc 
    // for now, we're going to use a cube
    private Vector3 originalPos = new Vector3(-0.5f, 0, -0.5f);
    private bool keyPressed = false;

    private PlayerInputActionsBase inputSystemControls;
    private InputAction move;
    #endregion

    void Start()
    {
        inputSystemControls = new PlayerInputActionsBase();
        move = inputSystemControls.Player.Move;

        this.transform.position = originalPos;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleKeyBasedMovement();
        UpdatePlayer();


        GridController.Tile3D? tile3D = GridController.GetTile3D(Tmap, GridController.WorldToGridPos(Tmap, this.transform.position));

        if (tile3D != null)
            Debug.Log(tile3D?.WorldPos);
    }

    private void UpdatePlayer()
    {
        Vector2 input = GetInput();

        if (keyPressed && input.magnitude == 0)
            keyPressed = false;

        if (keyPressed || input.magnitude == 0 || !move.enabled)
            return;

        if (input.x != 0)
            Move(Vector3.right * input.x);
        else
            Move(Vector3.forward * input.y);

        keyPressed = true;
    }

    private Vector2 GetInput()
    {
        return move.ReadValue<Vector2>();
    }

    private void Move(Vector3 movementVector)
    {
        this.transform.position += movementVector;
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
