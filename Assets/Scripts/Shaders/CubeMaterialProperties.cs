using UnityEngine;

public static class CubeShaderProperties
{
    // Liquid
    public static readonly int LiquidScale = Shader.PropertyToID("_LiquidScale");
    public static readonly int LiquidSpeed = Shader.PropertyToID("_LiquidSpeed");
    public static readonly int LiquidColorA = Shader.PropertyToID("_LiquidColorA");
    public static readonly int LiquidColorB = Shader.PropertyToID("_LiquidColorB");

    // Stone
    public static readonly int StoneScale = Shader.PropertyToID("_StoneScale");
    public static readonly int StoneSpeed = Shader.PropertyToID("_StoneSpeed");
    public static readonly int StoneThreshold = Shader.PropertyToID("_StoneThreshold");
    public static readonly int StoneColor = Shader.PropertyToID("_StoneColor");

    // Edges
    public static readonly int EdgeThickness = Shader.PropertyToID("_EdgeThickness_100");
    public static readonly int EdgeSoftness = Shader.PropertyToID("_EdgeSoftness_100");
    public static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
    public static readonly int EdgeIntensity = Shader.PropertyToID("_EdgeIntensity");
    public static readonly int EdgeEnabled = Shader.PropertyToID("_EdgeEnabler");

    // Shader transition
    public static readonly int ShaderFrom = Shader.PropertyToID("_ShaderFrom");
    public static readonly int ShaderTo = Shader.PropertyToID("_ShaderTo");
    public static readonly int TransitionStart = Shader.PropertyToID("_ShaderTransitionStart");
    public static readonly int TransitionDuration = Shader.PropertyToID("_ShaderTransitionDuration");
}
