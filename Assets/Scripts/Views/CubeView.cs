// Assets/Scripts/Views/CubeView.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CubeColorManager))]
public class CubeView : MonoBehaviour
{
    private CubeColorManager colorManager;
    private CubeModel model;

    public void Initialize(CubeModel cubeModel)
    {
        model = cubeModel;
        colorManager = GetComponent<CubeColorManager>();
        
        // Subscribe to model events
        model.OnTypeChanged += HandleTypeChanged;
        model.OnPositionChanged += HandlePositionChanged;
    }

    private void OnDestroy()
    {
        if (model != null)
        {
            model.OnTypeChanged -= HandleTypeChanged;
            model.OnPositionChanged -= HandlePositionChanged;
        }
    }

    // Visual methods
    public void InitColor(CubeModel.CubeType initialType)
    {
        colorManager.Init(ToShaderType(initialType));
    }

    public void InitPosition(Vector2Int gridPos)
    {
        transform.position = new Vector3(gridPos.x, 0, gridPos.y);
    }

    // Animation methods
    public void PlayAppearAnimation()
    {
        StartCoroutine(AppearRoutine());
    }

    public void PlayRiseAnimation(System.Action onComplete = null)
    {
        StartCoroutine(RiseRoutine(onComplete));
    }

    public void PlayMeltAnimation()
    {
        StartCoroutine(MeltRoutine());
    }

    public void PlayRollAnimation(Vector2Int direction, System.Action onComplete)
    {
        StartCoroutine(RollRoutine(direction, onComplete));
    }

    // Animation coroutines
    private IEnumerator AppearRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            yield return null;
        }
    }

    private IEnumerator RiseRoutine(System.Action onComplete = null)
    {
        float t = 0f;
        Vector3 start = transform.position;
        Vector3 end = transform.position + Vector3.up * 1f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        
        // Ensure final position is exact
        transform.position = end;
        
        onComplete?.Invoke();
    }

    private IEnumerator MeltRoutine()
    {
        float t = 0f;
        Vector3 start = transform.position;
        Vector3 end = transform.position + Vector3.down * 1f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    private IEnumerator RollRoutine(Vector2Int dir, System.Action onComplete)
    {
        Quaternion initRotation = transform.localRotation;
        Vector3 worldDir = new Vector3(dir.x, 0, dir.y);
        Vector3 anchor = transform.position + (Vector3.down + worldDir) * 0.5f;
        Vector3 axis = Vector3.Cross(Vector3.up, worldDir);
        float rotated = 0f;

        while (rotated < 90f)
        {
            float step = model.RollSpeed * Time.deltaTime;
            // Clamp the step to not overshoot 90 degrees
            step = Mathf.Min(step, 90f - rotated);
            transform.RotateAround(anchor, axis, step);
            rotated += step;
            yield return null;
        }

        // Force exact final position and rotation
        transform.localRotation = initRotation;
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );

        onComplete?.Invoke();
    }

    // Event handlers
    private void HandleTypeChanged(CubeModel.CubeType newType)
    {
        colorManager.ChangeTo(ToShaderType(newType));
    }

    private void HandlePositionChanged(Vector2Int newPos)
    {
        // Position is updated by animations, but we can add visual feedback here
    }

    // Helper methods
    private CubeShaderType ToShaderType(CubeModel.CubeType type)
    {
        switch (type)
        {
            case CubeModel.CubeType.Gray:
                return CubeShaderType.Gray;
            case CubeModel.CubeType.Green:
            case CubeModel.CubeType.GreenUnder:
                return CubeShaderType.Green;
            case CubeModel.CubeType.Black:
                return CubeShaderType.Black;
            case CubeModel.CubeType.Selected:
                return CubeShaderType.Selected;
            default:
                return CubeShaderType.Gray;
        }
    }
}
