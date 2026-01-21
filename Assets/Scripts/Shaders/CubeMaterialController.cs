using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CubeMaterialController : MonoBehaviour
{
    [SerializeField] private Material baseMaterial;
    public Material runtimeMaterial;

    void Awake()
    {
        if(baseMaterial == null)
        {
            Debug.LogError("BaseMaterial NOT assigned on " + gameObject.name);
            enabled = false;
            return;
        }
        
        runtimeMaterial = Instantiate(baseMaterial);
        GetComponent<Renderer>().material = runtimeMaterial;
    }

    public void SetEdges(bool enabled, float thickness, float softness, float intensity)
    {
        runtimeMaterial.SetInt(CubeShaderProperties.EdgeEnabled, enabled ? 1 : 0);
        runtimeMaterial.SetFloat(CubeShaderProperties.EdgeThickness, thickness);
        runtimeMaterial.SetFloat(CubeShaderProperties.EdgeSoftness, softness);
        runtimeMaterial.SetFloat(CubeShaderProperties.EdgeIntensity, intensity);
    }

    public void StartShaderTransition(int from, int to, float duration)
    {
        runtimeMaterial.SetInt(CubeShaderProperties.ShaderFrom, from);
        runtimeMaterial.SetInt(CubeShaderProperties.ShaderTo, to);
        runtimeMaterial.SetFloat(CubeShaderProperties.TransitionDuration, duration);
        runtimeMaterial.SetFloat(CubeShaderProperties.TransitionStart, Time.time);
    }

    public void ForceSetShader(int shaderIndex)
    {
        runtimeMaterial.SetInt(CubeShaderProperties.ShaderFrom, shaderIndex);
        runtimeMaterial.SetInt(CubeShaderProperties.ShaderTo, shaderIndex);
        runtimeMaterial.SetFloat(CubeShaderProperties.TransitionDuration, 0.001f);
        runtimeMaterial.SetFloat(CubeShaderProperties.TransitionStart, Time.time - 1f);
    }
}
