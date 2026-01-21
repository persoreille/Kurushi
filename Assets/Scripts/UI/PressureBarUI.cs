using UnityEngine;
using UnityEngine.UI;

public class PressureBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PressureManager pressureManager;
    [SerializeField] private Image fillImage;

    void Update()
    {
        if (pressureManager == null || fillImage == null)
            return;

        float normalizedPressure = pressureManager.GetNormalizedPressure();
        
        // Update fill amount only - preserves your gradient design
        fillImage.fillAmount = normalizedPressure;
    }
}
