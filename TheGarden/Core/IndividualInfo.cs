using System;
using System.Drawing;

namespace TheGarden.Core;

public record IndividualInfo
{
    public IndividualInfo(Type type, Color color, string name)
    {
        Name = name;
        Color = color;
        Type = type;
    }

    public readonly string Name;
    public readonly Type Type;
    public readonly Color Color;

    public Individual Create()
    {
        return new Individual {
            Object = Activator.CreateInstance(Type) 
                ?? throw new Exception($"cannot create the type {Type}"),
            Info = this
        };
    }
}