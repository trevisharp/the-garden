using System;
using System.Drawing;
using TheGarden;

var garden = new Garden();
garden.Add("plant", Color.LightGreen, 80);
garden.Run();

public class Plant
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Energy { get; set; } = 20;

    public string OnFrame(
        string onLeft, string onRight,
        string onTop, string onBot
    )
    {
        if (Random.Shared.Next(4) < 2)
            Energy += 10;
        
        if (Energy > 90)
        {
            Energy -= 50;
            return "REPRODUCE";
        }

        return "WAIT";
    }
}