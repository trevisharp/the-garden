using System;
using System.Drawing;
using TheGarden;

var garden = new Garden();
garden.Add("plant", Color.LightGreen, 80);
garden.Run();

public class Plant
{
    public int Energy { get; set; } = 20;

    public void Act(GardenKeeper gardenKeeper)
    {
        if (Random.Shared.Next(4) < 2)
            Energy += 10;
        
        if (Energy > 90)
        {
            Energy -= 50;
            gardenKeeper.ReproduceOnNeighborhood();
        }
    }
}