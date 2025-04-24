using System;
using System.Drawing;

namespace TheGarden.Core;

public record IndividualInfo(Type Type, Color DefaultColor, string Name)
{
    public Individual Create()
    {
        return new Individual {
            Object = Activator.CreateInstance(Type) 
                ?? throw new Exception($"cannot create the type {Type}"),
            Info = this,
            Color = DefaultColor
        };
    }
}