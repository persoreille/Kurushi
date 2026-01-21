#ifndef STONE_FLUID_INCLUDED
#define STONE_FLUID_INCLUDED

// ---------------- HASH ----------------
float Hash3(float3 p)
{
    return frac(sin(dot(p, float3(127.1,311.7,74.7))) * 43758.5453);
}

// ---------------- 3D NOISE ----------------
float Noise3D(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f);

    return lerp(
        lerp(
            lerp(Hash3(i + float3(0,0,0)), Hash3(i + float3(1,0,0)), u.x),
            lerp(Hash3(i + float3(0,1,0)), Hash3(i + float3(1,1,0)), u.x),
            u.y
        ),
        lerp(
            lerp(Hash3(i + float3(0,0,1)), Hash3(i + float3(1,0,1)), u.x),
            lerp(Hash3(i + float3(0,1,1)), Hash3(i + float3(1,1,1)), u.x),
            u.y
        ),
        u.z
    );
}

// ---------------- STRUCTURE ----------------
void StoneMask_float(
    float3 positionWS,
    float scale,
    float speed,
    float threshold,
    float time,
    out float Out
)
{
    float3 p = positionWS * scale + time * speed;
    float n = Noise3D(p);

    Out = smoothstep(threshold - 0.02, threshold + 0.02, n);
}

// ---------------- LIQUID COLOR ----------------
void LiquidColor_float(
    float3 positionWS,
    float scale,
    float speed,
    float4 colorA,
    float4 colorB,
    float time,
    out float3 Out
)
{
    float x = sin(sin(time/2)*(positionWS.x%5)/5)*2;
    float y = cos(cos(time/3)*(positionWS.y%3)/3)*1.5;
    float z = sin(cos(time/2)*(positionWS.z%5)/5)*3;
    //float3 p = positionWS * scale + float3(x, sin(time/2) * speed, z);
    float3 p = positionWS * scale + float3(x, y, z);
    float n = Noise3D(p);

    Out =  lerp(colorA, colorB, n);
}

// ------------------- Utility function --------------------
void ConvColor4to3_float(float4 color4, out float3 Out)
{
    float3 color3;
    color3.x = color4.x;
    color3.y = color4.y;
    color3.z = color4.z;
    Out = color3;
}

#endif
