using System;
using System.Drawing;
using TheGarden;

var garden = new Garden();
garden.Add("plant", Color.LightGreen, 80);
garden.Add("cow", Color.White, 30);
garden.Add("wolf", Color.Gray, 6);
garden.Run();

public class Plant
{
    public int Energy { get; set; } = 20;

    public void Act(Gardenkeeper keeper)
    {
        Energy += 10 - 2 * keeper.CountNeighborhood("plant", 1);
        
        if (Energy > 90)
        {
            Energy -= 50;
            keeper.ReproduceOnNeighborhood(3);
        }
        
        if (Energy < 0)
        {
            keeper.KillMe();
        }
    }
}

public class Cow
{
    public int Energy { get; set; } = 80;

    public void Act(Gardenkeeper keeper)
    {
        Energy -= 5;

        if (Energy < 0)
        {
            keeper.KillMe();
        }

        if (Energy > 90)
        {
            Energy -= 30;
            keeper.ReproduceOnNeighborhood(1);
        }

        if (Energy is < 10 or > 40)
            return;
        
        keeper.MoveToNext("plant");
        Energy -= 2;
        
        if (keeper.CountNeighborhood("plant", 1) < 1)
            return;
        
        var killed = keeper.KillNeighborhood("plant", 1);
        if (killed)
            Energy += 60;
    }
}

public class Wolf
{
    public int Energy { get; set; } = 50;

    public void Act(Gardenkeeper keeper)
    {
        Energy--;

        if (Energy < 0)
        {
            keeper.KillMe();
        }

        if (Energy > 90)
        {
            Energy -= 30;
            keeper.ReproduceOnNeighborhood(1);
        }

        if (Energy is < 10 or > 40)
            return;
        
        keeper.MoveToNext("cow", 2, 10, 2);
        Energy -= 2;
        
        if (keeper.CountNeighborhood("cow", 2) < 1)
            return;
        
        var killed = keeper.KillNeighborhood("cow", 2);
        if (killed)
            Energy += 80;
    }
}