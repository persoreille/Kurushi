using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class CubeLevelManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private CubeFactory cubeFactory;
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private PressureManager pressureManager;

    private Dictionary<Vector2Int, CubeController> grid = new Dictionary<Vector2Int, CubeController>();

    public event Action OnRollStarted;
    public event Action OnRollFinished;
    // private bool isRolling = false;
    // public bool IsRolling => isRolling;

    public void Awake()
    {
        pressureManager.OnPressureTimeout += () => StartCoroutine(RollForwardByRowsCoroutine());
    }

    
    public void Register(CubeController cube)
    {
        grid[cube.Model.GridPos] = cube;
        cube.Model.OnDestroyed += () => Unregister(cube);
    }

    public void Unregister(CubeController cube)
    {
        cube.Model.OnDestroyed -= () => Unregister(cube);
        if(grid.TryGetValue(cube.Model.GridPos, out CubeController storedCube))
        {
            if(storedCube == cube)
            {
                grid.Remove(cube.Model.GridPos);
            }
        }
    }

    public CubeController GetCubeAt(Vector2Int pos)
    {
        grid.TryGetValue(pos, out CubeController cube);
        return cube;
    }
    
    public void ChangeCube(Vector2Int pos, CubeModel.CubeType newType)
    {
        CubeController cube = GetCubeAt(pos);
        if(cube == null) return;

        cube.Model.SetType(newType);
    }


    public IEnumerable<CubeController> GetAllCubes()
    {
        return grid.Values;
    }

    public void CreateLevelCubes(LevelStructure levelStructure)
    {
        for(int x = 0; x < levelStructure.width; x++)
        {
            for(int z = 0; z < levelStructure.depth; z++)
            {
                CubeModel.CubeType type = levelStructure.cells[x, z];
                Vector2Int pos = new Vector2Int(x,z);
                CubeController cube = cubeFactory.CreateCube(pos, type, transform);

                if (cube != null)
                {
                    Register(cube);
                }
            }
        }
    }

    private void BeginRoll(CubeController cube)
    {
        grid.Remove(cube.Model.GridPos);
    }

    public IEnumerator RollForwardByRowsCoroutine()
    {
        // isRolling = true;
        OnRollStarted?.Invoke(); // Inform that started for a roll

        var rows = grid
            .Values
            .GroupBy(c => c.Model.GridPos.y)
            .OrderByDescending(g => g.Key)
            .ToList();

        foreach(var row in rows)
        {
            // Calculate delay - use a fraction of roll duration for close following
            // 0.3f means next row starts when current row is 30% done
            float rollDuration = row.First().Model.GetRollDuration();
            float delay = rollDuration * 0.3f;

            foreach(CubeController cube in row)
            {
                BeginRoll(cube);
                cube.Model.OnRollFinished += () => OnCubeRollFinished(cube);
                cube.Roll(Vector2Int.up);
            }

            yield return new WaitForSeconds(delay);
            
        }
        // isRolling = false;
        yield return new WaitForSeconds(rows[0].First().Model.GetRollDuration()*0.8f);
        OnRollFinished?.Invoke();

    }


    public bool IsOccupied(Vector2Int gridPos)
    {
        return grid.ContainsKey(gridPos);
    }

    public bool IsCellReserved(Vector2Int pos)
    {
        bool ret = grid.Values.Any(c =>
            c.Model.IsRolling && c.Model.TargetGridPos == pos
        );

        if (ret)
            Debug.Log("Reserved");

        return ret;
    }

    public bool IsWorldPositionBLocked(Vector3 worldPos, float radius = 0.3f)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.z)
        );
        return (IsOccupied(gridPos) || IsCellReserved(gridPos));
    }


    private void OnCubeRollFinished(CubeController cube)
    {
        cube.Model.OnRollFinished -= () => OnCubeRollFinished(cube);
        grid[cube.Model.GridPos] = cube;
    }


    public bool CubeBlow(CubeController selectedCube)
    {
        if (selectedCube == null)
        {
            Debug.Log("CubeBlow: selectedCube is null");
            return false;
        }

        Debug.Log($"CubeBlow: Selected cube at {selectedCube.Model.GridPos}, height={selectedCube.transform.position.y}, type={selectedCube.Model.ClassicType}");

        Vector2Int pos = selectedCube.Model.GridPos;
        bool isGroundCube = Mathf.Approximately(selectedCube.transform.position.y, 0f);

        // Player can only blow from ground cubes
        if (!isGroundCube)
        {
            Debug.Log("CubeBlow: Can only blow from ground level cubes");
            return false;
        }

        // If the selected ground cube is GREEN, blow all 9 cubes in 3x3 area above
        if (selectedCube.Model.ClassicType == CubeModel.CubeType.Green)
        {
            Debug.Log("CubeBlow: Green ground cube - blowing 9 cubes in 3x3 area");
            BlowCubesAbove(pos);
            return true;
        }

        // Normal blow: find the cube ABOVE the selected ground cube
        CubeController cubeAbove = FindCubeAbove(pos);
        
        if (cubeAbove == null)
        {
            Debug.Log("CubeBlow: No cube above to blow");
            return false;
        }

        Debug.Log($"CubeBlow: Found cube above at height {cubeAbove.transform.position.y}, type={cubeAbove.Model.ClassicType}");
        Debug.Log("CubeBlow: Normal blow - blowing single cube above");
        BlowSingleCube(cubeAbove, selectedCube);

        return true;
    }

    private CubeController FindCubeAbove(Vector2Int pos)
    {
        foreach (var kvp in grid)
        {
            CubeController c = kvp.Value;
            if (c.Model.GridPos == pos && c.transform.position.y > 0f)
            {
                return c;
            }
        }
        return null;
    }

    private void BlowSingleCube(CubeController cubeToBlow, CubeController groundCube)
    {
        CubeModel.CubeType cubeType = cubeToBlow.Model.ClassicType;
        
        Debug.Log($"BlowSingleCube: Melting cube type {cubeType} to ground");
        
        // Start melt animation, then change ground cube type
        StartCoroutine(MeltCubeToGround(cubeToBlow, groundCube, cubeType));
    }

    private IEnumerator MeltCubeToGround(CubeController cubeToMelt, CubeController groundCube, CubeModel.CubeType finalType)
    {
        float duration = 1f;
        float t = 0f;
        Vector3 startPos = cubeToMelt.transform.position;
        Vector3 endPos = groundCube.transform.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            cubeToMelt.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        // Animation complete - destroy the melted cube and change ground cube type
        cubeToMelt.DestroyCube();
        groundCube.ChangeType(finalType);
    }

    private CubeController FindGroundCubeAt(Vector2Int pos)
    {
        // Ground cubes are managed by GroundGenerator, not in our grid
        if (groundGenerator != null)
        {
            return groundGenerator.GetGroundCubeAt(pos);
        }
        return null;
    }

    private IEnumerator MeltToGroundCoroutine(CubeController cube, CubeModel.CubeType finalType)
    {
        float t = 0f;
        Vector3 start = cube.transform.position;
        Vector3 end = new Vector3(start.x, 0f, start.z);

        while (t < 1f)
        {
            t += Time.deltaTime;
            cube.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        cube.transform.position = end;
        cube.ChangeType(finalType);
    }

    private void BlowCubesAbove(Vector2Int groundPos)
    {
        Debug.Log($"BlowCubesAbove: Blowing 3x3 area above {groundPos}");
        
        // First, turn the triggering green ground cube to gray
        CubeController triggeringGroundCube = FindGroundCubeAt(groundPos);
        if (triggeringGroundCube != null && 
            (triggeringGroundCube.Model.ActualType == CubeModel.CubeType.Green || 
             triggeringGroundCube.Model.ClassicType == CubeModel.CubeType.Green))
        {
            Debug.Log($"BlowCubesAbove: Turning green ground cube at {groundPos} to gray");
            triggeringGroundCube.ChangeType(CubeModel.CubeType.Gray);
        }
        
        // Then blow all cubes in 3x3 area above
        for (int x = groundPos.x - 1; x <= groundPos.x + 1; x++)
        {
            for (int z = groundPos.y - 1; z <= groundPos.y + 1; z++)
            {
                Vector2Int checkPos = new Vector2Int(x, z);
                
                // Find ground cube at this position
                CubeController groundCube = FindGroundCubeAt(checkPos);
                if (groundCube == null) continue;
                
                // Find cube above this ground cube
                CubeController cubeAbove = FindCubeAbove(checkPos);
                if (cubeAbove == null) continue;
                
                Debug.Log($"BlowCubesAbove: Blowing cube at {checkPos}");
                BlowSingleCube(cubeAbove, groundCube);
            }
        }
    }

    public void TriggerGreenReaction()
    {
        Debug.Log("TriggerGreenReaction: Finding all green ground cubes");
        
        if (groundGenerator == null)
        {
            Debug.LogError("TriggerGreenReaction: GroundGenerator reference is missing!");
            return;
        }
        
        // Get all green ground cubes from GroundGenerator
        List<CubeController> greenGroundCubes = groundGenerator.GetAllGreenGroundCubes();
        
        Debug.Log($"TriggerGreenReaction: Found {greenGroundCubes.Count} green ground cubes");
        
        // Blow cubes above each green ground cube
        foreach (CubeController greenCube in greenGroundCubes)
        {
            Debug.Log($"TriggerGreenReaction: Blowing cubes above {greenCube.Model.GridPos}");
            BlowCubesAbove(greenCube.Model.GridPos);
        }
    }

    public IEnumerator RiseLevelCubesCoroutine()
    {
        Debug.Log("RiseLevelCubes: Starting rise sequence");
        
        int cubesRising = grid.Values.Count;
        int cubesFinished = 0;
        
        foreach(var cube in grid.Values)
        {
            cube.Rise(() => {
                cubesFinished++;
                // Debug.Log($"Cube finished rising. {cubesFinished}/{cubesRising}");
            });
        }
        
        // Wait until all cubes have finished rising
        while (cubesFinished < cubesRising)
        {
            yield return null;
        }
        
        Debug.Log("RiseLevelCubes: All cubes finished rising!");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
