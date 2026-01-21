using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GroundGenerator groundGenerator;
    // [SerializeField] private LevelStructure levelStructure;
    [SerializeField] private CubeLevelManager cubeLevelManager;
    

    [Header("Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Settings")]
    [SerializeField] private float spawnHeight = 1f;

    private GameObject playerInstance;
    private PlayerController playerController;

    public GameObject CreateCharacter(LevelStructure levelStructure, Transform parent = null)
    {
        
        if(playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in Character script");
            return null;
        }
        Vector3 spawnPosition = new Vector3(
            levelStructure.width / 2 -0.5f,
            spawnHeight,
            1
        );

        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, parent);
        playerController = playerInstance.GetComponent<PlayerController>();

        if(playerController == null)
        {
            Debug.LogError("Player prefab must have PlayerCOntroller component");
            return null;
        }

        playerController.Initialize(groundGenerator, cubeLevelManager);

        Debug.Log($"Player created at {spawnPosition}");
        
        return playerInstance;
        
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
