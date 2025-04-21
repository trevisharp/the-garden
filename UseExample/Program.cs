using System;
using System.Drawing;
using TheGarden;

var garden = new Garden();
garden.Add("plant", Color.LightGreen, 80);
garden.Add("cow", Color.White, 30);
garden.Add("wolf", Color.DarkRed, 8);
garden.Run();

public class Plant
{
    public float Energy { get; set; } = Random.Shared.Next(100);

    public void Act(Gardenkeeper keeper)
    {
        var sumlight = 1 - 0.2f * keeper.CountNeighborhood("plant", 1);
        Energy += sumlight * Random.Shared.NextSingle();
        
        if (Energy > 60)
        {
            Energy -= 40;
            keeper.ReproduceOnNeighborhood(5);
        }
        
        if (Energy < 0)
        {
            keeper.KillMe();
        }
    }
}

public class Cow
{
    public float Energy { get; set; } = 80;

    public void Act(Gardenkeeper keeper)
    {
        Energy--;

        if (Energy < 0)
        {
            keeper.KillMe();
            keeper.CreateNeighborhood("plant", 3);
            keeper.CreateNeighborhood("plant", 3);
            keeper.CreateNeighborhood("plant", 3);
            return;
        }

        if (Energy < 60 && keeper.CountNeighborhood("plant", 1) > 0)
        {
            keeper.KillNeighborhood("plant", 1);
            Energy += 30;
            return;
        }

        if (Energy > 80)
        {
            Energy -= 30;
            keeper.ReproduceOnNeighborhood(1);
            return;
        }

        if (Energy > 40)
        {
            if (Random.Shared.Next(2) == 0)
                return;
            
            keeper.Move(
                Random.Shared.Next(3) - 1,
                Random.Shared.Next(3) - 1
            );
            return;
        }
        
        keeper.MoveToNext("plant");
    }
}

public class Wolf
{
    public int Age { get; set; } = 0;
    public float Energy { get; set; } = 50;

    public void Act(Gardenkeeper keeper)
    {
        Age++;
        Energy--;

        if (Energy < 0 || Age == 400)
        {
            keeper.KillMe();
            keeper.CreateNeighborhood("plant", 3);
        }

        if (Age == 200)
        {
            keeper.ReproduceOnNeighborhood(1);
        }

        if (Energy > 60)
            return;
        
        keeper.MoveToNext("cow", 1, 20);
        
        if (keeper.CountNeighborhood("cow", 1) < 1)
            return;
        
        keeper.KillNeighborhood("cow", 1);
        Energy += 40;
    }
}