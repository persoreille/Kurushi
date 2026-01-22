using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSequenceController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private CubeLevelManager cubeLevelManager;
    [SerializeField] private PressureManager pressureManager;
    [SerializeField] private Character characterFactory;  // ← Add this reference
    [SerializeField] private PlayerController playerController;
    

    [Header("Timings")]
    [SerializeField] private float delayBetweenPhases = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        // playerController.Initialize(groundGenerator, cubeLevelManager);
        StartCoroutine(PlaySequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlaySequence()
    {
        LevelStructure level = LevelLoader.Load("Level_01", 1);

        Debug.Log("Width:"+level.width + " / Depth:"+level.depth);
        
        // Create the ground for the level
        yield return StartCoroutine(groundGenerator.SpawnSequence(level.width, level.playfieldDepth));

        // yield return new WaitForSeconds(delayBetweenPhases);

        // Create the cubes inside the ground
        cubeLevelManager.CreateLevelCubes(level);

        yield return new WaitForSeconds(delayBetweenPhases);

        // Rise the playing cubes from the ground - WAIT for them to finish rising
        yield return StartCoroutine(cubeLevelManager.RiseLevelCubesCoroutine());

        yield return new WaitForSeconds(delayBetweenPhases);
        
        // Now start rolling - cubes are fully risen

        yield return new WaitForSeconds(2f);

        // Create the player character
        characterFactory.CreateCharacter(level, transform);
        
        // Starts timer and rolling
        pressureManager.ResetPressure(false);
        

    
    }

}
