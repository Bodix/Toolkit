Due to the number of operations, you need to have shader model 3.0 at least.
So don't forget to add the following line right after CGPROGRAM:
```cg
pragma target 3.0
```

## Possible optimizations
```cg
mod289(x) = fmod(x, 289.0)
taylorInvSqrt(x) = rsqrt(x)
```

```cg
// GLSL: lessThan(x, y) = x < y
// HLSL: 1 - step(y, x) = x < y
s = float4(
    1 - step(0.0, p)
);
p.xyz = p.xyz + (s.xyz * 2 - 1) * s.www;
 
// Which is equivalent to:
p.xyz -= sign(p.xyz) * (p.w < 0);
```

```cg
// float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
// Lex-DRL: afaik, step() in GPU is faster than if(), so:
// step(x, y) = x <= y
int xLessEqual = step(x0.x, x0.y); // x <= y ?
int2 i1 =
    int2(1, 0) * (1 - xLessEqual) // x > y
    + int2(0, 1) * xLessEqual // x <= y
;
float4 x12 = x0.xyxy + C.xxzz;
x12.xy -= i1;
 
// Actually, a simple conditional without branching is faster than that madness :)
float4 x12 = x0.xyxy + C.xxzz;
x12.xy -= (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
```

## [Thread](https://forum.unity.com/threads/2d-3d-4d-optimised-perlin-noise-cg-hlsl-library-cginc.218372/)
