using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeFactory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab;


    private static CubeFactory instance;
    public static CubeFactory Instance => instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Create cube controller
    public CubeController CreateCube(Vector2Int position, CubeModel.CubeType Type, Transform parent = null)
    {
        // Instanciate the cube prefab
        GameObject cubeObj = Instantiate(cubePrefab, parent);

        // Set world position
        Vector3 worldPos = new Vector3(position.x, 0, position.y);
        cubeObj.transform.position = worldPos;
        
        // Get the controller component
        CubeController controller = cubeObj.GetComponent<CubeController>();
        
        if (controller == null)
        {
            Debug.LogError($"CubeFactory: Cube prefab is missing CubeController component!");
            Destroy(cubeObj);
            return null;
        }
        
        // Initialize the cube with MVC pattern
        controller.Initialize(Type, position);
        
        return controller;
    }

    /// <summary>
    /// Creates multiple cubes from a level structure
    /// </summary>
    /// <param name="levelStructure">Level data</param>
    /// <param name="parent">Parent transform</param>
    /// <returns>List of created "CubeController"</returns>
    public List<CubeController> CreateCubesFromLevel(LevelStructure levelStructure, Transform parent = null)
    {
        List<CubeController> cubes = new List<CubeController>();
        
        for (int x = 0; x < levelStructure.width; x++)
        {
            for (int z = 0; z < levelStructure.depth; z++)
            {
                CubeModel.CubeType type = levelStructure.cells[x, z];
                
                // Skip empty cells if needed
                if (type == CubeModel.CubeType.Gray) // or whatever represents empty
                {
                    continue;
                }
                
                Vector2Int position = new Vector2Int(x, z);
                CubeController cube = CreateCube(position, type, parent);
                
                if (cube != null)
                {
                    cubes.Add(cube);
                }
            }
        }
        
        return cubes;
    }

    /// <summary>
    /// Creates a cube at a specific world position
    /// </summary>
    public CubeController CreateCubeAtWorldPosition(Vector3 worldPos, CubeModel.CubeType type, Transform parent = null)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.z)
        );
        
        return CreateCube(gridPos, type, parent);
    }
}