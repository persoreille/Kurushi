#ifndef SELECT_CUBE_SHADER
#define SELECT_CUBE_SHADER

#define SHADER_COUNT 8

float GetWeight(int shaderID, int index)
{
    return shaderID == index ? 1.0 : 0.0;
}

void ShaderSelect8_float(
    int shaderID,
    float3 c0,
    float3 c1,
    float3 c2,
    float3 c3,
    float3 c4,
    float3 c5,
    float3 c6,
    float3 c7,
    out float3 ShaderOut
)
{
    ShaderOut =
        c0 * GetWeight(shaderID, 0) +
        c1 * GetWeight(shaderID, 1) +
        c2 * GetWeight(shaderID, 2) +
        c3 * GetWeight(shaderID, 3) +
        c4 * GetWeight(shaderID, 4) +
        c5 * GetWeight(shaderID, 5) +
        c6 * GetWeight(shaderID, 6) +
        c7 * GetWeight(shaderID, 7);
}

void ShaderSelect8Transition_float(
    int fromID,
    int toID,
    float startTime,
    float duration,
    float3 shader0,
    float3 shader1,
    float3 shader2,
    float3 shader3,
    float3 shader4,
    float3 shader5,
    float3 shader6,
    float3 shader7,
    out float3 ShaderOut
    //out float TransitionFinished
)
{
    // Calculate transition progress
    // Using max() to avoid division by zero
    float elapsed = _Time.y - startTime;
    float t = saturate(elapsed / max(duration, 0.0001));

    // Get the color for the "from" shader state
    float3 fromColor = 
        shader0 * GetWeight(fromID, 0) +
        shader1 * GetWeight(fromID, 1) +
        shader2 * GetWeight(fromID, 2) +
        shader3 * GetWeight(fromID, 3) +
        shader4 * GetWeight(fromID, 4) +
        shader5 * GetWeight(fromID, 5) +
        shader6 * GetWeight(fromID, 6) +
        shader7 * GetWeight(fromID, 7);

    // Get the color for the "to" shader state
    float3 toColor =
        shader0 * GetWeight(toID, 0) +
        shader1 * GetWeight(toID, 1) +
        shader2 * GetWeight(toID, 2) +
        shader3 * GetWeight(toID, 3) +
        shader4 * GetWeight(toID, 4) +
        shader5 * GetWeight(toID, 5) +
        shader6 * GetWeight(toID, 6) +
        shader7 * GetWeight(toID, 7);

    // Lerp between the two colors based on transition progress
    ShaderOut = lerp(fromColor, toColor, t);
}



#endif