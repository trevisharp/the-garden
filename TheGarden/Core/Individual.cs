namespace TheGarden.Core;

public class Individual
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Energy { get; set; }
    
    public required object Object { get; init; }
    public required IndividualInfo Info { get; init; }

    public string RunGeneration(params object[] parameters)
    {
        return (string)Info.OnGeneration?.Invoke(Object, parameters)!;
    }

    public void Copy()
    {
        if (Info.X is not null)
        {
            X = (int)Info.X.GetMethod?.Invoke(Object, [])!;
        }
        
        if (Info.Y is not null)
        {
            Y = (int)Info.Y.GetMethod?.Invoke(Object, [])!;
        }
        
        if (Info.Energy is not null)
        {
            Energy = (int)Info.Energy.GetMethod?.Invoke(Object, [])!;
        }
    }

    public void Paste()
    {
        Info.X?.SetMethod?.Invoke(Object, [ X ]);
        
        Info.Y?.SetMethod?.Invoke(Object, [ Y ]);
        
        Info.Energy?.SetMethod?.Invoke(Object, [ Energy ]);
    }
}