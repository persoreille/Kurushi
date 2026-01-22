using UnityEngine;
using System;
using UnityEditor.PackageManager;

public class PressureManager : MonoBehaviour
{
    [Header("Pressure Settings")]
    [SerializeField] private float maxPressureTime = 5f;
    [SerializeField] private CubeLevelManager cubeLevelManager;

    private float timer;
    private bool paused = true;

    public Action OnPressureTimeout;

    
    void Start()
    {   
        // Subscribers
        cubeLevelManager.OnRollStarted += () => {
            Pause();
            ResetPressure();
        };
        cubeLevelManager.OnRollFinished += () => {
            ResetPressure();
            Play();
        };
        OnPressureTimeout += () => {
            Pause();
            ResetPressure();
        };

        // ResetPressure();
        // Pause();

    }

    private void WaitForSeconds(float v)
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        if (paused) return;

        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            //timer = 0f;
            paused = true;
            OnPressureTimeout?.Invoke();
        }
    }

    public void ResetPressure()
    {
        timer = maxPressureTime;
        // paused = _paused;
    }

    public float GetNormalizedPressure()
    {
        return timer / maxPressureTime;
    }

    public void Pause() { paused = true; }
    public void Play() { paused = false; }

    
}
