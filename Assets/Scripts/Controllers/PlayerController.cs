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
        cubeLevelManager.OnRollStarted += () =>  
        {
            isRolling = true;
            Debug.Log("[[[Rolling]]]");
        };

        cubeLevelManager.OnRollFinished += () =>
        {
            isRolling = false;
            Debug.Log("[[[Not rolling]]]");
        };

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

        // if (ctx.performed)
        // {
        //     Debug.Log($"Move {moveInput}");
        // }

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
                if(cubeSelected != null)
                {
                    cubeSelected.Unselect();  // Use Controller method
                }
                groundCube.Select();  // Use Controller method
                cubeSelected = groundCube;
            }
        }
    }

    void OnCubeBlow(InputAction.CallbackContext ctx)
    {
        // Only process on performed phase to avoid double triggers
        if (!ctx.performed)
            return;

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

    // // Appelé par PlayerInput (Input System)
    // public void OnMove(InputValue value)
    // {
    //     moveInput = value.Get<Vector2>();
    // }
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
            

        // Detecter s'il s'approche trop des bords:
        
        Vector3 moveDir = new Vector3(-moveInput.x, 0f, -moveInput.y);
        bool isMoving = moveDir.sqrMagnitude > 0.001f;

        if (!isMoving)
        {
            animator.SetBool("isRunning", isMoving);
            return;
        }
        
        Vector3 nextPos3d = transform.position + moveDir * moveSpeed * Time.deltaTime;
        Vector2 nextPos2d = new Vector2(nextPos3d.x, nextPos3d.z);

        bool borderClose = ground.IsInsideBounds(transform.position, 0.2f);
        bool rightDirection = ground.IsInsideBounds(transform.position + moveDir*0.2f, 0.1f);

        Vector2 playerPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerDir = new Vector2(moveDir.x, moveDir.z);


        if (borderClose && !rightDirection)
        {
            Debug.Log("Can't move");
            animator.SetBool("isRunning", false);
            return;
        }

        //Vector3 nextPos = transform.position + moveDir * moveSpeed * Time.deltaTime;
        if (cubeLevelManager.IsWorldPositionBLocked(nextPos3d))
        {
            Debug.Log("Player blocked !");
            animator.SetBool("isRunning", false);
            return;
        }

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


}
