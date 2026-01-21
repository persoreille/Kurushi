// Assets/Scripts/Controllers/CubeController.cs
using System;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CubeView view;
    [Header("Settings")]
    [SerializeField] private float rollSpeed;

    private CubeModel model;

    public CubeModel Model => model;
    public CubeView View => view;
    public Action changeRollSpeed;
    
    public void Initialize(CubeModel.CubeType initialType, Vector2Int gridPos)
    {
        // Create model
        model = new CubeModel();
        model.RollSpeed = rollSpeed; // Link rollSpeed to RollSpeed

        model.Initialize(gridPos, initialType);
                
        // Initialize view with model
        view.Initialize(model);
        view.InitColor(initialType);
        view.InitPosition(gridPos);
    }

    // Public methods called by external systems
    public void Select()
    {
        if (model.ActualType == CubeModel.CubeType.Selected)
        {
            Debug.Log("[CubeController] Already selected");
            return;
        }
        
        Debug.Log("[CubeController] Selecting cube");
        model.Select();
    }

    public void Unselect()
    {
        Debug.Log("[CubeController] Unselecting cube");
        model.Unselect();
    }

    public void ChangeType(CubeModel.CubeType newType)
    {
        Debug.Log("[CubeController] Changing type to " + newType);
        model.ChangeType(newType);
    }

    public void Roll(Vector2Int direction)
    {
        if (model.IsRolling)
        {
            Debug.Log("[CubeController] Already rolling");
            return;
        }

        Debug.Log("[CubeController] Rolling cube");
        model.IsRolling = true;
        model.TargetGridPos = model.GridPos + direction;
        
        view.PlayRollAnimation(direction, () => {
            model.UpdatePosition(model.TargetGridPos);
            model.NotifyRollFinished();
        });
    }

    public void Appear()
    {
        view.PlayAppearAnimation();
    }

    public void Rise(System.Action onComplete = null)
    {
        view.PlayRiseAnimation(onComplete);
    }

    public void Melt()
    {
        view.PlayMeltAnimation();
    }

    public void DestroyCube()
    {
        model.NotifyDestroyed();
        Destroy(gameObject);
    }

    public string GetPositionString()
    {
        return $"{{{model.GridPos.x};{model.GridPos.y}}}";
    }
}
