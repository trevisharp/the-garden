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
        OnGeneration = "OnFrame".AsMethod(type);
        X = "X".AsProperty(type);
        Y = "Y".AsProperty(type);
        Energy = "Energy".AsProperty(type);
    }

    public readonly Type IndividualType;
    public readonly Color Color;
    public readonly MethodInfo? OnGeneration;
    public readonly PropertyInfo? X;
    public readonly PropertyInfo? Y;
    public readonly PropertyInfo? Energy;

    public Individual Create()
    {
        return new Individual {
            Object = Activator.CreateInstance(IndividualType) 
                ?? throw new Exception($"cannot create the type {IndividualType}"),
            Info = this
        };
    }
}