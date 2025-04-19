using System;
using System.Drawing;
using System.Collections.Generic;

using Radiance;
using static Radiance.Utils;

namespace TheGarden;

using Core;

public class Garden
{
    int nextId = 1;
    const int defaultSize = 80;
    readonly int[] board = new int[defaultSize * defaultSize];
    readonly Dictionary<int, Individual> individuals = [];
    float camdx = 0;
    float camdy = 0;
    float camzoom = 20;

    public void Add(string typeName, Color color, int initialPopulation)
    {
        var type = typeName.AsType();
        var info = new IndividualInfo(type, color);
        for (int i = 0; i < initialPopulation; i++)
            Add(info);
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
                    var id = board[defaultSize * y + x];
                    if (id == 0)
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
                        var individual = individuals[id];
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

    void RunGeneration()
    {
        for (int j = 0; j < defaultSize; j++)
        {
            for (int i = 0; i < defaultSize; i++)
            {
                int index = defaultSize * j + i;
                var id = board[index];
                if (id == 0)
                    continue;
                
                var individual = individuals[id];
                individual.Paste();
                var behaviour = individual.RunGeneration([ null, null, null, null ]);
                individual.Copy();

                if (behaviour == "REPRODUCE")
                {
                    AddApprox(individual.Info, individual.X, individual.Y);
                }
                else if (behaviour == "DIE")
                {
                    individuals.Remove(id);
                    board[index] = 0;
                }
            }
        }
    }

    void Add(IndividualInfo info)
    {
        int x = Random.Shared.Next(defaultSize);
        int y = Random.Shared.Next(defaultSize);

        for (int k = 0; board[defaultSize * y + x] != 0 && k < board.Length; k++)
        {
            x = Random.Shared.Next(defaultSize);
            y = Random.Shared.Next(defaultSize);
        }
        
        Add(info, x, y);
    }

    void Add(IndividualInfo info, int x, int y)
    {
        var id = nextId++;
        var individual = info.Create();
        individual.Copy();

        individual.X = x;
        individual.Y = y;

        individuals.Add(id, individual);
        board[defaultSize * y + x] = id;
    }

    void AddApprox(IndividualInfo info, int x, int y)
    {
        for (int k = 0; k < 121; k++)
        {
            int dx = Random.Shared.Next(11) - 5;
            int dy = Random.Shared.Next(11) - 5;

            var tx = dx + x;
            var ty = dy + y;
            if (tx < 0 || ty < 0 || tx >= defaultSize || ty >= defaultSize)
                continue;

            if (board[defaultSize * ty + tx] != 0)
                continue;

            x += dx;
            y += dy;
            break;
        }

        var id = nextId++;
        var individual = info.Create();
        individual.Copy();

        individual.X = x;
        individual.Y = y;

        individuals.Add(id, individual);
        board[defaultSize * y + x] = id;
    }
}