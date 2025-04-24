using System;
using System.Drawing;

using TheGarden;

var garden = new Garden();
garden.AddFull("deadcell", Color.Red);
garden.AddFull("deadcell", Color.Red);
garden.Run();

public class Cell
{
    public void Act(Gardenkeeper gardenkeeper)
    {
        gardenkeeper.Move(
            Random.Shared.Next(3) - 1,
            Random.Shared.Next(3) - 1
        );
    }
}