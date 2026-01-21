#ifndef CUBE_FACE_DIAGONAL_INCLUDED
#define CUBE_FACE_DIAGONAL_INCLUDED

// --------------------------------------------------
// Cube Face Diagonal Gradient
// --------------------------------------------------
// positionOS : Object Space position
// cubeSize   : Cube size (1 = Unity cube)
// colorA     : Gradient start color
// colorB     : Gradient end color
// contrast   : Gradient contrast
// Out        : Final color
// --------------------------------------------------

void CubeFaceDiagonal_float(
    float3 positionOS,
    float cubeSize,
    float4 colorA,
    float4 colorB,
    float contrast,
    out float3 Out
)
{
    // Normalize to [-1 ; 1]
    float3 p = positionOS / (cubeSize * 0.5);
    float3 ap = abs(p);

    float gradient = 0.0;

    // Detect dominant axis → face selection
    if (ap.x >= ap.y && ap.x >= ap.z)
    {
        // X face → use YZ
        gradient = (p.y + p.z) * 0.5 + 0.5;
    }
    else if (ap.y >= ap.x && ap.y >= ap.z)
    {
        // Y face → use XZ
        gradient = (p.x + p.z) * 0.5 + 0.5;
    }
    else
    {
        // Z face → use XY
        gradient = (p.x + p.y) * 0.5 + 0.5;
    }

    gradient = saturate(gradient);
    gradient = pow(gradient, max(contrast, 0.001));

    Out = lerp(colorA.rgb, colorB.rgb, gradient);
}

#endif
