using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float rotationSpeed = 720f;
    

    private GroundGenerator ground;
    private CubeLevelManager cubeLevelManager;

    private PlayerInputActions input;
    private CubeController cubeSelected = null;  // Changed from CubeModel to CubeController
    private bool isRolling = false;
    
    private Vector2 moveInput;
    private Animator animator;

    public void Initialize(GroundGenerator groundGen, CubeLevelManager cubeManager)
    {
        ground = groundGen;
        cubeLevelManager = cubeManager;

        // Subscribe to the events to do something
        cubeLevelManager.OnRollStarted += () => { isRolling = true; };
        cubeLevelManager.OnRollFinished += () => { isRolling = false; };
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        input = new PlayerInputActions();

        
    }

    void OnEnable()
    {
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        input.Player.CubeSelect.performed += OnCubeSelect;
        input.Player.CubeBlow.performed += OnCubeBlow;
        input.Player.CubeGreenReaction.performed += OnCubeGreenReaction;
        input.Player.CubeRoll.performed += OnCubeRoll;
        input.Player.Enable();
    }
    void OnDisable()
    {
        input.Player.Disable();
    }

    void Start()
    {
 
    }

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
        // Only process on performed phase to avoid double triggers
        if (!ctx.performed)
            return;
        
        CubeController groundCube = ground.GetGroundCubeUnderWorldPos(transform.position);
        
        if(groundCube != null)
        {
            if(cubeSelected == groundCube)
            {
                groundCube.Unselect();  // Use Controller method
                cubeSelected = null;
            }
            else
            {
                if(cubeSelected != null) // Cube selected already
                {
                    cubeSelected.Unselect();  // Use Controller method
                }
                groundCube.Select();  // Use Controller method
                cubeSelected = groundCube;

                Debug.Log($"Cube selected: {cubeSelected.Model.GridPos.ToString()}");
            }
        }
    }

    void OnCubeBlow(InputAction.CallbackContext ctx)
    {
        // Only process on performed phase to avoid double triggers
        if (!ctx.performed)
        {
            Debug.Log("No blow action requested");
            return;
        }
            

        if (cubeSelected == null)
        {
            Debug.Log("No cube selected to blow");
            return;
        }

        Debug.Log($"Blowing cube at {cubeSelected.Model.GridPos}, type: {cubeSelected.Model.ClassicType}");
        
        // CubeBlow handles the melting and destruction
        if (cubeLevelManager.CubeBlow(cubeSelected))
        {
            // Clear the selection since the cube is being destroyed
            cubeSelected = null;
            Debug.Log("Cube blown successfully");
        }
    }

    void OnCubeGreenReaction(InputAction.CallbackContext ctx)
    {
        // Only process on performed phase to avoid double triggers
        if (!ctx.performed)
            return;

        Debug.Log("Green reaction triggered - blowing all green ground cubes");
        cubeLevelManager.TriggerGreenReaction();
    }

    void OnCubeRoll(InputAction.CallbackContext ctx)
    {
        
        if((!ctx.performed) || isRolling)
            return;

        Debug.Log("Cube roll triggered manually");
        
        StartCoroutine(cubeLevelManager.RollForwardByRowsCoroutine());
    }

    void Update()
    {
        // Don't update if not initialized yet
        if (ground == null || cubeLevelManager == null)
        {
            Debug.LogError("GroundGenerator or CubeLevelManager not initialized");
            return;
        }
                    
        // Test if movement asked
        if (moveInput.sqrMagnitude < 0.001f)
        {
            animator.SetBool("isRunning", false);
            return;
        }
        
        Vector3 moveDir = new Vector3(-moveInput.x, 0f, -moveInput.y);
        Vector3 nextPos3d = transform.position + moveDir * moveSpeed * Time.deltaTime;
    
        // Border detection
        if (BorderIsAlmostThere(moveInput, transform.position, 0.1f))
        {
            Debug.Log("[PlayerController] Border close => Can't move");
            animator.SetBool("isRunning", false);
            return;
        }

        
        // TODO : get this working
        // if (borderClose && !rightDirection)
        // {
        //     Debug.Log("Can't move");
        //     animator.SetBool("isRunning", false);
        //     return;
        // }

        //Vector3 nextPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

        // TODO : get this working
        // if (cubeLevelManager.IsWorldPositionBLocked(nextPos3d))
        // {
        //     Debug.Log("Player blocked !");
        //     animator.SetBool("isRunning", false);
        //     return;
        // }

        animator.SetBool("isRunning", true);
        // Debug.Log(transform.position);
        moveDir.Normalize();

        // Rotation vers la direction
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        // Déplacement
        transform.position = nextPos3d;
    }

    private bool BorderIsAlmostThere(Vector2 _moveDir, Vector3 _playerPos, float margin = 0.5f)
    {
        Vector3 nextPos = _playerPos - new Vector3(_moveDir.x, 0, _moveDir.y)*margin;
        return ground.GetGroundCubeUnderWorldPos(nextPos) == null;
    }


}
