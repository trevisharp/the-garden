using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Radiance;
using static Radiance.Utils;

namespace TheGarden;

using Core;

public class Garden
{
    const int defaultSize = 80;
    readonly List<Individual>[] board;
    readonly List<IndividualInfo> infos;
    float camdx = 0;
    float camdy = 0;
    float camzoom = 20;

    public Garden()
    {
        infos = [];
        board = new List<Individual>[defaultSize * defaultSize];
        for (int i = 0; i < board.Length; i++)
            board[i] = [];
    }

    public void Add(string typeName, Color color, int initialPopulation)
    {
        var type = typeName.AsType();
        var info = new IndividualInfo(type, color, typeName);
        infos.Add(info);

        for (int i = 0; i < initialPopulation; i++)
            AddOnRandomPlance(info);
    }

    public void Run()
    {
        var clk = new Clock();

        var fieldRender = render((vec4 clr, vec2 pos, val size) =>
        {
            zoom(size);
            move(pos);

            color = clr;
            fill();

            color = (.75f, .75f, .75f, 1);
            draw(2f);
        });
        fieldRender = fieldRender(Polygons.Square);
        
        float cx = -1, cy = -1;
        float holdcx = -1, holdcy = -1;
        bool mouseDown = false, fstDown = true;
        Window.OnMouseDown += b => 
        {
            fstDown = true;
            mouseDown |= b == MouseButton.Left;
        };
        Window.OnMouseUp += b => mouseDown &= b != MouseButton.Left;
        Window.OnMouseWhell += whell =>
        {
            var newZoom = float.Clamp(camzoom + whell, 10, 100);

            camdx = (camdx - cx) * (newZoom / camzoom) + cx;
            camdy = (camdy - cy) * (newZoom / camzoom) + cy;

            camzoom = newZoom;
        };
        Window.OnMouseMove += pos => 
        {
            (cx, cy) = pos;
            if (!mouseDown)
                return;

            if (fstDown)
            {
                (holdcx, holdcy) = pos;
                fstDown = false;
                return;
            }

            camdx += pos.x - holdcx;
            camdy += pos.y - holdcy;
            (holdcx, holdcy) = pos;
        };

        Window.OnKeyDown += (input, modifier) =>
        {
            if (input == Input.Space)
            {
                clk.ToogleFreeze();
            }
        };

        Window.OnFrame += () =>
        {
            if (clk.Time < 0.1f)
                return;
            
            RunGeneration();
            clk.Reset();
        };

        Window.OnRender += () =>
        {
            for (int y = 0; y < defaultSize; y++)
            {
                for (int x = 0; x < defaultSize; x++)
                {
                    var list = board[defaultSize * y + x];
                    if (list.Count == 0)
                    {
                        fieldRender(
                            vec(0f, .25f, 0f, 1f), 
                            camdx + camzoom * x,
                            camdy + camzoom * y,
                            camzoom
                        );
                    }
                    else
                    {
                        var individual = list[0];
                        fieldRender(
                            individual.Info.Color.R / 255f,
                            individual.Info.Color.G / 255f,
                            individual.Info.Color.B / 255f,
                            1f, 
                            camdx + camzoom * x,
                            camdy + camzoom * y,
                            camzoom
                        );
                    }
                }
            }
        };

        Window.CloseOn(Input.Escape);
        Window.Open();
    }

    internal void AddOnRegion(string typeName, int x, int y, int radius)
    {
        var info = infos.FirstOrDefault(i => i.Name == typeName);
        if (info is null)
            return;
        
        AddOnRegion(info, x, y, radius);
    }

    internal void Move(Individual individual, int dx, int dy)
    {
        var x = individual.X;
        var y = individual.Y;

        var tx = int.Clamp(x + dx, 0, defaultSize - 1);
        var ty = int.Clamp(y + dy, 0, defaultSize - 1);

        var initialList = board[x + defaultSize * y];
        initialList.Remove(individual);

        var targetList = board[tx + defaultSize * ty];
        targetList.Add(individual);

        individual.X = tx;
        individual.Y = ty;
    }

    internal void Kill(Individual individual)
    {
        board[individual.X + individual.Y * defaultSize].Remove(individual);
    }

    internal bool KillNeighborhood(string name, Individual individual, int radius)
    {
        var target = GetBestNeighborhood(name, individual, radius);
        if (target is null)
            return false;
        
        Kill(target);
        return true;
    }

    internal void AddOnRandomPlance(IndividualInfo info)
    {
        int x = Random.Shared.Next(defaultSize);
        int y = Random.Shared.Next(defaultSize);
        
        Add(info, x, y);
    }

    internal void Add(IndividualInfo info, int x, int y)
    {
        var individual = info.Create();

        individual.X = x;
        individual.Y = y;

        board[defaultSize * y + x].Add(individual);
    }

    internal void AddOnRegion(IndividualInfo info, int x, int y, int radius)
    {
        var randomField = GetRegion(x, y, radius)
            .OrderBy(x => Random.Shared.Next())
            .FirstOrDefault();

        Add(info, randomField.x, randomField.y);
    }

    internal Individual? GetBestNeighborhood(string name, Individual individual, int radius)
    {
        var query =
            from ind in GetNeighborhood(individual, radius)
            where ind.Info.Name == name
            let dx = ind.X - individual.X
            let dy = ind.Y - individual.Y
            let distance = dx * dx + dy * dy
            orderby distance ascending
            select ind;
        
        return query.FirstOrDefault();
    }

    internal int Count(string name, Individual individual, int radius)
    {
        return GetNeighborhood(individual, radius)
            .Count(t => t.Info.Name == name);
    }

    private IEnumerable<Individual> GetNeighborhood(Individual individual, int radius)
    {
        return GetRegion(individual.X, individual.Y, radius)
            .SelectMany(t => t.individuals)
            .Where(ind => ind != individual);
    }

    private IEnumerable<(int x, int y, List<Individual> individuals)> GetRegion(int x, int y, int radius)
    {
        int jmin = int.Max(0, y - radius),
            jmax = int.Min(defaultSize, y + radius + 1),
            imin = int.Max(0, x - radius),
            imax = int.Min(defaultSize, x + radius + 1);

        for (int j = jmin; j < jmax; j++)
        {
            for (int i = imin; i < imax; i++)
            {
                yield return (i, j, board[j * defaultSize + i]);
            }
        }
    }

    void RunGeneration()
    {
        var individuals = board
            .SelectMany(x => x)
            .ToArray();
        foreach (var individual in individuals)
        {
            var keeper = new Gardenkeeper(this, individual);
            individual.RunGeneration(keeper);
        }
    }
}