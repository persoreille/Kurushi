using UnityEngine;

/// <summary>
/// PlayerView - Handles player visuals and animations
/// MonoBehaviour that updates based on model state
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerView_REFERENCE : MonoBehaviour
{
    private Animator animator;
    private PlayerModel_REFERENCE model;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void Initialize(PlayerModel_REFERENCE playerModel)
    {
        model = playerModel;
        
        // Subscribe to model events
        model.OnMovementStateChanged += HandleMovementChanged;
        model.OnCubeSelected += HandleCubeSelected;
        model.OnCubeUnselected += HandleCubeUnselected;
    }
    
    void OnDestroy()
    {
        if (model != null)
        {
            model.OnMovementStateChanged -= HandleMovementChanged;
            model.OnCubeSelected -= HandleCubeSelected;
            model.OnCubeUnselected -= HandleCubeUnselected;
        }
    }
    
    // Event handlers
    private void HandleMovementChanged(bool isMoving)
    {
        animator.SetBool("isRunning", isMoving);
    }
    
    private void HandleCubeSelected(CubeController cube)
    {
        // Visual feedback when cube is selected
        // e.g., play selection sound, show UI indicator, etc.
        IDebug.Log($"Cube selected at {cube.Model.GridPos}");
    }
    
    private void HandleCubeUnselected()
    {
        // Visual feedback when cube is unselected
        IDebug.Log("Cube unselected");
    }
    
    // Animation methods
    public void UpdateRotation(Quaternion targetRotation, float rotationSpeed)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    
    public void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
