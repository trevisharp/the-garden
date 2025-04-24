using System;
using System.Drawing;

using TheGarden;

var garden = new Garden();
garden.AddFull("cell", Color.Black);
garden.Run();

public class Cell
{
    public void Act(Gardenkeeper gardenkeeper)
    {
        
    }
}