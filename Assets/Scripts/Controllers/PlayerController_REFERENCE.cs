using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController - Handles input and coordinates Model/View
/// This is a REFERENCE implementation showing full MVC pattern
/// </summary>
[RequireComponent(typeof(PlayerView_REFERENCE))]
public class PlayerController_REFERENCE : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerView_REFERENCE view;
    [SerializeField] private GroundGenerator ground;
    [SerializeField] private CubeLevelManager cubeLevelManager;
    
    private PlayerModel_REFERENCE model;
    private PlayerInputActions input;
    private Vector2 moveInput;
    
    void Awake()
    {
        // Create the model
        model = new PlayerModel_REFERENCE();
        
        // Initialize view with model
        view.Initialize(model);
        
        // Set up input
        input = new PlayerInputActions();
    }
    
    void OnEnable()
    {
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.CubeSelect.performed += OnCubeSelect;
        input.Player.CubeBlow.performed += OnCubeBlow;
        input.Player.CubeGreenReaction.performed += OnCubeGreenReaction;
        input.Player.Enable();
    }
    
    void OnDisable()
    {
        input.Player.Disable();
    }
    
    // Input handlers
    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        
        if (ctx.canceled)
        {
            moveInput = Vector2.zero;
        }
    }
    
    void OnCubeSelect(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        
        CubeController groundCube = ground.GetGroundCubeUnderWorldPos(transform.position);
        
        if (groundCube != null)
        {
            model.SelectCube(groundCube);
        }
    }
    
    void OnCubeBlow(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        
        if (!model.HasSelectedCube())
        {
            IDebug.Log("No cube selected to blow");
            return;
        }
        
        IDebug.Log($"Blowing cube at {model.SelectedCube.Model.GridPos}");
        
        if (cubeLevelManager.CubeBlow(model.SelectedCube))
        {
            model.UnselectCube();
            IDebug.Log("Cube blown successfully");
        }
    }
    
    void OnCubeGreenReaction(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        
        IDebug.Log("Green reaction triggered");
        cubeLevelManager.TriggerGreenReaction();
    }
    
    // Update loop
    void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        Vector3 moveDir = new Vector3(-moveInput.x, 0f, -moveInput.y);
        bool isMoving = moveDir.sqrMagnitude > 0.001f;
        
        // Update model state
        model.SetMoving(isMoving);
        
        if (!isMoving) return;
        
        // Check boundaries
        Vector3 nextPos = transform.position + moveDir * model.MoveSpeed * Time.deltaTime;
                
        // Check cube collision
        if (cubeLevelManager.IsWorldPositionBLocked(nextPos))
        {
            model.SetMoving(false);
            return;
        }
        
        // Move player
        moveDir.Normalize();
        
        // Update rotation through view
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        view.UpdateRotation(targetRot, model.RotationSpeed);
        
        // Update position through view
        view.UpdatePosition(nextPos);
    }
}
