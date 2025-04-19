using System;
using System.Drawing;
using System.Reflection;

namespace TheGarden.Core;

public record IndividualInfo
{
    public IndividualInfo(Type type, Color color)
    {
        Color = color;
        IndividualType = type;
        OnGeneration = null!;
    }

    public readonly Type IndividualType;
    public readonly Color Color;
    public readonly MethodInfo OnGeneration;

    public Individual Create(int x, int y)
    {
        return new Individual {
            Info = this,
            X = x,
            Y = y
        };
    }
}