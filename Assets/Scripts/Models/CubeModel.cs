// Assets/Scripts/Models/CubeModel.cs
using System;
using UnityEngine;

public class CubeModel
{
    // Data properties
    public Vector2Int GridPos { get; set; }
    public Vector2Int TargetGridPos { get; set; }
    public CubeType ActualType { get; private set; }
    public CubeType ClassicType { get; private set; }
    public bool IsRolling { get; set; }
    public float RollSpeed { get; set; } = 180f;
    
    // Events for state changes
    public event Action<CubeType> OnTypeChanged;
    public event Action<Vector2Int> OnPositionChanged;
    public event Action OnRollFinished;
    public event Action OnDestroyed;

    public enum CubeType
    {
        Gray,
        Green,
        GreenUnder,
        Black,
        Selected
    }

    public void Initialize(Vector2Int gridPos, CubeType initialType)
    {
        GridPos = gridPos;
        ClassicType = initialType;
        ActualType = initialType;
    }

    // Business logic (pure data manipulation)
    public void SetType(CubeType newType)
    {
        if (ActualType == newType) return;
        
        ActualType = newType;
        OnTypeChanged?.Invoke(newType);
    }

    public void ChangeType(CubeType newType)
    {
        ClassicType = newType;
        SetType(newType);
    }

    public void Select()
    {
        SetType(CubeType.Selected);
    }

    public void Unselect()
    {
        SetType(ClassicType);
    }

    public void UpdatePosition(Vector2Int newPos)
    {
        GridPos = newPos;
        OnPositionChanged?.Invoke(newPos);
    }

    public void NotifyRollFinished()
    {
        IsRolling = false;
        OnRollFinished?.Invoke();
    }

    public void NotifyDestroyed()
    {
        OnDestroyed?.Invoke();
    }

    public float GetRollDuration()
    {
        return 90f / RollSpeed;
    }
}
