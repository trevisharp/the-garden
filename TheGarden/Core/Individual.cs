namespace TheGarden.Core;

public class Individual
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public required object Object { get; init; }
    public required IndividualInfo Info { get; init; }

    public void RunGeneration(GardenKeeper gardenKeeper)
    {
        var actMethod = "Act".AsMethod(Info.Type);
        if (actMethod is null)
            return;
        
        actMethod?.Invoke(Object, [ gardenKeeper ]);
    }
}