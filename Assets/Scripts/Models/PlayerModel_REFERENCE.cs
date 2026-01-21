using System;

/// <summary>
/// PlayerModel - Pure data class for player state
/// NO Unity dependencies (no Vector3, no MonoBehaviour)
/// </summary>
public class PlayerModel_REFERENCE
{
    // Player state data
    public float MoveSpeed { get; set; } = 0.5f;
    public float RotationSpeed { get; set; } = 720f;
    public bool IsMoving { get; set; }
    
    // Selected cube reference
    private CubeController selectedCube;
    public CubeController SelectedCube 
    { 
        get => selectedCube;
        private set => selectedCube = value;
    }
    
    // Events for state changes
    public event Action<CubeController> OnCubeSelected;
    public event Action OnCubeUnselected;
    public event Action<bool> OnMovementStateChanged;
    
    // Business logic methods
    public void SelectCube(CubeController cube)
    {
        if (cube == null) return;
        
        // If same cube, unselect it
        if (selectedCube == cube)
        {
            UnselectCube();
            return;
        }
        
        // Unselect previous cube if any
        if (selectedCube != null)
        {
            selectedCube.Unselect();
        }
        
        // Select new cube
        selectedCube = cube;
        cube.Select();
        OnCubeSelected?.Invoke(cube);
    }
    
    public void UnselectCube()
    {
        if (selectedCube != null)
        {
            selectedCube.Unselect();
            selectedCube = null;
            OnCubeUnselected?.Invoke();
        }
    }
    
    public void SetMoving(bool moving)
    {
        if (IsMoving != moving)
        {
            IsMoving = moving;
            OnMovementStateChanged?.Invoke(moving);
        }
    }
    
    public bool HasSelectedCube()
    {
        return selectedCube != null;
    }
}
