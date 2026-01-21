#ifndef CUBE_EDGES_INCLUDED
#define CUBE_EDGES_INCLUDED

// --------------------------------------------------
// Cube Edge Mask (edges only, scale independent)
// --------------------------------------------------
// positionOS : Object Space position (Shader Graph: Object Position)
// thickness  : Edge thickness (0-100)
// softness   : Edge softness (0-100)
// cubeSize   : Size of cube (default 1)
// out Out    : 0 → no edge, 1 → edge
// --------------------------------------------------

void CubeEdges_float(
    float3 positionOS,
    float thickness,
    float softness,
    float cubeSize,
    out float Out
)
{
    float3 p = abs(positionOS) / (cubeSize * 0.5);

    float t = 1.0 - thickness / 100.0;

    // Binary edge per axis (like Shader Graph Step)
    float dx = 1-step(t, p.x);
    float dy = 1-step(t, p.y);
    float dz = 1-step(t, p.z);

    float sum = dx + dy + dz;

    // Edge = exactly 2 axes near surface
    float edge = step(1.5, sum) * (1.0 - step(2.5, sum));

    // Optional smoothing
    Out = smoothstep(1.0, 0.0, edge);
}



// void CubeEdges_float(
//     float3 positionOS,
//     float thickness,
//     float softness,
//     float cubeSize,
//     out float Out
// )
// {
//     // Normalize position to [-1, 1] space
//     float3 p = abs(positionOS / (cubeSize * 0.5));
//     float thick = thickness/100;
//     float soft = softness/100;

//     // Distance to cube surface per axis
//     float dx = 1 - smoothstep(1.0 - thick - soft, 1.0 - thick, p.x);
//     float dy = 1 - smoothstep(1.0 - thick - soft, 1.0 - thick, p.y);
//     float dz = 1 - smoothstep(1.0 - thick - soft, 1.0 - thick, p.z);

//     // Count how many axes are close to surface
//     float sum = dx + dy + dz;

//     // Keep only edges (exactly 2 axes active)
//     Out = smoothstep(1.9, 2.1, sum);
// }


// --------------------------------------------------
// ApplyCubeEdges
// --------------------------------------------------
// baseColor     : Shader Input
// edgeMask      : CubeEdges output
// edgeColor     : color to put on the edges
// edgeIntensity : intensity of the color of the edges

// Out : 
// --------------------------------------------------

void ApplyCubeEdges_float(
    float3 baseColor,
    float edgeMask,
    float3 edgeColor,
    float edgeIntensity,
    bool enableEdges,
    out float3 Out
)
{
    float mask = edgeMask * (enableEdges ? 1.0 : 0.0);

    //Out = baseColor + edgeColor * edgeIntensity * mask;
    Out = lerp(baseColor, edgeColor, -mask*edgeIntensity);
}

#endif
