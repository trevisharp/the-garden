namespace TheGarden;

using Core;

public class GardenKeeper(Garden garden, Individual current)
{
    readonly Garden garden = garden;
    readonly Individual individual = current;

    public int GetMyXPosition()
        => individual.X;
    
    public int GetMyYPosition()
        => individual.Y;
    
    public int SetMyXPosition(int x)
        => individual.X = x;
    
    public int SetMyYPosition(int y)
        => individual.Y = y;
    
    public void Move(int dx, int dy)
    {
        individual.X += dx;
        individual.Y += dy;
    }

    public void KillMe()
    {
        garden.Kill(individual);
    }

    public void ReproduceOn5x5Neighborhood()
    {
        garden.AddApprox(
            individual.Info,
            individual.X,
            individual.Y
        );
    }
}