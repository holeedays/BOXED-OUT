using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    #region Modifiable Fields
    [Range(0f, 50f)]
    public float MoveSpeed;
    #endregion

    #region Misc Fields
    // this is our script that contains all of our mapped player inputs
    private PlayerInputActionsBase inputSystemControls;
    private InputAction move;
    #endregion

    private void Awake()
    {
        inputSystemControls = new PlayerInputActionsBase();
        // by default, we have a UI mapping and Player input Action Maps, we choose the player one
        // and we also access the move component, since those are the actions we are looking at for our camera
        move = inputSystemControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        // if camera were to be ever disabled, we can disable move controls
        move.Disable();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        this.transform.position += new Vector3(move.ReadValue<Vector2>().x, 0f, move.ReadValue<Vector2>().y) * Time.deltaTime;
    }

}
