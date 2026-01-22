using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSequenceController : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds2 = new WaitForSeconds(2f);
    [Header("References")]
    [SerializeField] private GroundGenerator groundGenerator;
    [SerializeField] private CubeLevelManager cubeLevelManager;
    [SerializeField] private PressureManager pressureManager;
    [SerializeField] private Character characterFactory;  // ← Add this reference
    //[SerializeField] private PlayerController playerController;
    

    [Header("Timings")]
    [SerializeField] private float delayBetweenPhases = 0.5f;

    private LevelStructure levelStructure;
    private int levelPart = 0;
    private int levelNumber = 1;


    private String getLevelName(int _levelNumber)
    {
        // String number = String.Format()
        return $"Level_{String.Format("{0:00}", _levelNumber)}";
        // String.Format("{0:00000}", 15);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundGenerator.OnSpawnGroundSequenceFinished += OnSpawnGroundSequenceFinished;
        levelStructure = new LevelStructure();
        
        // Subscribe to level part finished event
        cubeLevelManager.OnLevelPartFinished += () =>
        {
            // Use member variables from this class
            String levelName = getLevelName(levelNumber);
            int nextPart = levelStructure.partIndex + 1;
            NextLevelPart(levelName, nextPart);
        };
        
        IDebug.Log($"levelName = {getLevelName(levelNumber)}");
        LaunchLevel(getLevelName(levelNumber));
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void LaunchLevel(String levelFile)
    {
        //StartCoroutine(
        if (StartLevel(levelFile) < 0)
        {
            IDebug.Log("Impossible de démarrer le level");
        }
        ;
    }


    private int StartLevel(String levelFile)
    {
        levelStructure = LevelLoader.Load(levelFile, levelPart+1);
        if(levelStructure == null)
            return -1;
        IDebug.Log($"LevelNumber: {levelFile} / LevelPart: {levelStructure.partIndex}");
        IDebug.Log($"Width: {levelStructure.width} / Depth: {levelStructure.depth}");

        StartCoroutine(groundGenerator.SpawnSequence(levelStructure.width, levelStructure.playfieldDepth));
        
        characterFactory.CreateCharacter(levelStructure, transform);
        return 0;
    }

    private void StartLevelPart(String levelFile, int part = 1)
    {
        levelStructure = LevelLoader.Load(levelFile, part);
        cubeLevelManager.CreateLevelCubes(levelStructure);
        StartCoroutine(cubeLevelManager.RiseLevelCubesCoroutine());
        
        // Timer before start
        new WaitForSeconds(2f);

        pressureManager.ResetPressure();
        pressureManager.Play();
    }


    void NextLevelPart(String levelFile, int part)
    {
        levelStructure = LevelLoader.Load(levelFile, part);
        if(levelStructure == null)
        {
            // TODO: Go to next level
        }

        cubeLevelManager.CreateLevelCubes(levelStructure);
        StartCoroutine(cubeLevelManager.RiseLevelCubesCoroutine());

        // TImer before beginning
        new WaitForSeconds(2f);
        pressureManager.ResetPressure();
        pressureManager.Play();
    }

    void OnSpawnGroundSequenceFinished()
    {
        StartLevelPart(getLevelName(levelNumber), 1);
    }

}
