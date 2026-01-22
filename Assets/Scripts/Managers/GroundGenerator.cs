// Assets/Scripts/Managers/GroundGenerator.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GroundGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CubeFactory cubeFactory;
    
    private Dictionary<Vector2Int, CubeController> groundCubes = new Dictionary<Vector2Int, CubeController>();


    public Action OnSpawnGroundSequenceFinished;
    public IEnumerator SpawnSequence(int width, int depth)
    // public void SpawnSequence(int width, int depth)
    {
        // Create ground cubes using the factory
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);
                
                // Use factory to create ground cubes
                CubeController groundCube = cubeFactory.CreateCube(
                    pos, 
                    CubeModel.CubeType.Gray, // Ground cubes are gray
                    transform
                );
                
                if (groundCube != null)
                {
                    groundCubes[pos] = groundCube;
                }
                
                
                yield return null; // Stagger creation
            }
        }
        OnSpawnGroundSequenceFinished?.Invoke();
    }

    public CubeController GetGroundCubeAt(Vector2Int pos)
    {
        groundCubes.TryGetValue(pos, out CubeController cube);
        return cube;
    }

    public CubeController GetGroundCubeUnderWorldPos(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.z)
        );
        return GetGroundCubeAt(gridPos);
    }

    public List<CubeController> GetAllGreenGroundCubes()
    {
        List<CubeController> greenCubes = new List<CubeController>();
        
        foreach (var kvp in groundCubes)
        {
            if (kvp.Value.Model.ActualType == CubeModel.CubeType.Green)
            {
                greenCubes.Add(kvp.Value);
            }
        }
        
        return greenCubes;
    }

    // public bool IsInsideBounds(Vector3 worldPos, float margin = 0f)
    // {
    //     // Implementation depends on your ground dimensions
    //     // This is just an example
    //     return worldPos.x >= margin && worldPos.z >= margin;
    // }

    // public bool IsInsideBounds(Vector3 worldPos, float margin = 0f)
    // {
        
    //     return
    //         pos.x >= margin-0.5 &&
    //         pos.x < (width-0.5) - margin &&
    //         pos.z >= margin-0.5 &&
    //         pos.z < (depth-0.5) - margin;
    // }
}
