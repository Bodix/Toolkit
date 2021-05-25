fixed invLerp(fixed a, fixed b, fixed value)
{
    return (value - a) / (b - a);
}

fixed remap(fixed inputA, fixed inputB, fixed outputA, fixed outputB, fixed value)
{
    float weight = invLerp(inputA, inputB, value);

    return lerp(outputA, outputB, weight);
}