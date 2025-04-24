namespace TheGarden;

using System.Drawing;
using Core;

public class Gardenkeeper(Garden garden, Individual current)
{
    readonly Garden garden = garden;
    readonly Individual individual = current;

    public int GetMyXPosition()
        => individual.X;
    
    public int GetMyYPosition()
        => individual.Y;
    
    public void ChangeColor(Color color)
        => individual.Color = color;
    
    public void Move(int dx, int dy)
    {
        garden.Move(individual, dx, dy);
    }

    public void KillMe()
    {
        garden.Kill(individual);
    }

    public void CreateEntityOnNeighborhood(string entity, int vision = 5)
    {
        garden.AddOnRegion(
            entity,
            individual.X,
            individual.Y,
            vision
        );
    }

    public bool KillEntityOnNeighborhood(string entity, int vision = 5)
    {
        return garden.KillNeighborhood(
            entity,
            individual,
            vision
        );
    }

    public void ReproduceOnNeighborhood(int size = 5)
    {
        garden.AddOnRegion(
            individual.Info,
            individual.X,
            individual.Y,
            size
        );
    }

    public int CountNeighborhood(string entity, int size = 5)
    {
        return garden.Count(
            entity,
            individual,
            size
        );
    }

    public void MoveToNext(string entity, int speed = 1, int vision = 5, int minDistance = 1)
    {
        var target = garden.GetBestNeighborhood(
            entity,
            individual,
            vision
        );
        if (target is null)
            return;
        
        var x = target.X;
        var y = target.Y;
        var dx = x - individual.X;
        var dy = y - individual.Y;
        int distance = int.Abs(dx) + int.Abs(dy);
        
        for (int i = 0; i < speed; i++)
        {
            if (distance <= minDistance)
                break;
            distance--;

            if (dx is > 0)
            {
                Move(1, 0);
                dx--;
            }
            else if (dx is < 0)
            {
                Move(-1, 0);
                dx++;
            }
            else if (dy is > 0)
            {
                Move(0, 1);
                dy--;
            }
            else if (dy is < 0)
            {
                Move(0, -1);
                dy++;
            }
        }
    }
}