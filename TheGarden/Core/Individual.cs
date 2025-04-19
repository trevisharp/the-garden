namespace TheGarden.Core;

public class Individual
{
    public required IndividualInfo Info { get; init; }
    public required int X { get; set; }
    public required int Y { get; set; }
}