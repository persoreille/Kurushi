using UnityEngine;
using System;

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
        cubeLevelManager.OnRollFinished += () => ResetPressure();
        OnPressureTimeout += () => ResetPressure();
        ResetPressure();
        Pause();
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
        paused = false;
    }

    public float GetNormalizedPressure()
    {
        return timer / maxPressureTime;
    }

    public void Pause() { paused = true; }
    public void Play() { paused = false; }

    
}
