// https://gist.github.com/Elringus/d21c8b0f87616ede9014
// https://elringus.me/blend-modes-in-unity/

// Created by Bogdan Nikolayev.
fixed3 Average(fixed3 a, fixed3 b)
{
    return (a + b) / 2;
}

fixed3 Darken(fixed3 a, fixed3 b)
{
    return min(a, b);
}

fixed3 Multiply(fixed3 a, fixed3 b)
{
    return a * b;
}

fixed3 ColorBurn(fixed3 a, fixed3 b)
{
    return saturate(1.0 - (1.0 - a) / b);
}

fixed3 LinearBurn(fixed3 a, fixed3 b)
{
    return a + b - 1.0;
}

// TODO: Implement GrabPass.
// fixed3 DarkerColor(fixed3 a, fixed3 b)
// {
//     return G(a) < G(b) ? a : b;
// }

fixed3 Lighten(fixed3 a, fixed3 b)
{
    return max(a, b);
}

fixed3 Screen(fixed3 a, fixed3 b)
{
    return 1.0 - (1.0 - a) * (1.0 - b);
}

fixed3 ColorDodge(fixed3 a, fixed3 b)
{
    return saturate(a / (1.0 - b));
}

fixed3 LinearDodge(fixed3 a, fixed3 b)
{
    return a + b;
}

// TODO: Implement GrabPass.
// fixed3 LighterColor(fixed3 a, fixed3 b)
// {
//     return G(a) > G(b) ? a : b;
// }

fixed3 Overlay(fixed3 a, fixed3 b)
{
    return a > .5 ? 1.0 - 2.0 * (1.0 - a) * (1.0 - b) : 2.0 * a * b;
}

fixed3 SoftLight(fixed3 a, fixed3 b)
{
    return (1.0 - a) * a * b + a * (1.0 - (1.0 - a) * (1.0 - b));
}

fixed3 HardLight(fixed3 a, fixed3 b)
{
    return b > .5 ? 1.0 - (1.0 - a) * (1.0 - 2.0 * (b - .5)) : a * (2.0 * b);
}

fixed3 VividLight(fixed3 a, fixed3 b)
{
    return saturate(b > .5 ? a / (1.0 - (b - .5) * 2.0) : 1.0 - (1.0 - a) / (b * 2.0));
}

fixed3 LinearLight(fixed3 a, fixed3 b)
{
    return b > .5 ? a + 2.0 * (b - .5) : a + 2.0 * b - 1.0;
}

fixed3 PinLight(fixed3 a, fixed3 b)
{
    return b > .5 ? max(a, 2.0 * (b - .5)) : min(a, 2.0 * b);
}

fixed3 HardMix(fixed3 a, fixed3 b)
{
    return (b > 1.0 - a) ? 1.0 : .0;
}

fixed3 Difference(fixed3 a, fixed3 b)
{
    return abs(a - b);
}

fixed3 Exclusion(fixed3 a, fixed3 b)
{
    return a + b - 2.0 * a * b;
}

fixed3 Subtract(fixed3 a, fixed3 b)
{
    return a - b;
}

fixed3 Divide(fixed3 a, fixed3 b)
{
    return saturate(a / b);
}
